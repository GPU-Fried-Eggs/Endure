using Endure.Models.Yolo;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Collections.Concurrent;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace Endure.Services.Yolo;

public class YoloScorer<T> : IDisposable where T : YoloModel
{
    private readonly InferenceSession m_inferenceSession;

    private readonly T m_model;

    /// <summary>
    /// Creates new instance of YoloScorer with weights path and options.
    /// </summary>
    public YoloScorer(string weights, SessionOptions? opts = null)
    {
        m_inferenceSession = new InferenceSession(File.ReadAllBytes(weights), opts ?? new SessionOptions());
        m_model = Activator.CreateInstance<T>();
    }

    /// <summary>
    /// Creates new instance of YoloScorer with weights stream and options.
    /// </summary>
    public YoloScorer(Stream weights, SessionOptions? opts = null)
    {
        using var reader = new BinaryReader(weights);
        m_inferenceSession = new InferenceSession(reader.ReadBytes((int)weights.Length), opts ?? new SessionOptions());
        m_model = Activator.CreateInstance<T>();
    }

    /// <summary>
    /// Creates new instance of YoloScorer with weights bytes and options.
    /// </summary>
    public YoloScorer(byte[] weights, SessionOptions? opts = null)
    {
        m_inferenceSession = new InferenceSession(weights, opts ?? new SessionOptions());
        m_model = Activator.CreateInstance<T>();
    }

    ~YoloScorer() => Dispose(false);

    /// <summary>
    /// Runs object detection.
    /// </summary>
    public List<YoloPrediction> Predict(IImage image)
        => Suppress(ParseOutput(Inference(image), (image.Width, image.Height))); // size always downwards

    /// <summary>
    /// Disposes YoloScorer instance.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Runs inference session.
    /// </summary>
    private DenseTensor<float>[] Inference(IImage image)
    {
        var inputs = new List<NamedOnnxValue> // add image as onnx input
        {
            NamedOnnxValue.CreateFromTensor("images", new ImageTensor(image, m_model).Tensor)
        };

        IDisposableReadOnlyCollection<DisposableNamedOnnxValue> result = m_inferenceSession.Run(inputs); // run inference

        return m_model.Outputs.Select(item => (DenseTensor<float>)result.First(x => x.Name == item).Value).ToArray();
    }

    /// <summary>
    /// Parses net outputs (sigmoid or detect layer) to predictions.
    /// </summary>
    private List<YoloPrediction> ParseOutput(DenseTensor<float>[] output, (float Width, float Height) size)
        => m_model.UseDetect ? ParseDetect(output[0], size) : ParseSigmoid(output, size);

        /// <summary>
    /// Parses net output (detect) to predictions.
    /// </summary>
    private List<YoloPrediction> ParseDetect(DenseTensor<float> output, (float Width, float Height) size)
    {
        var result = new ConcurrentBag<YoloPrediction>();

        var (xGain, yGain) = (m_model.Width / size.Width, m_model.Height / size.Height); // x, y gains

        var gain = Math.Min(xGain, yGain); // gain = resized / original

        var (xPad, yPad) = ((m_model.Width - size.Width * gain) / 2, (m_model.Height - size.Height * gain) / 2); // left, right pads

        Parallel.For(0, (int)output.Length / m_model.Dimensions, i =>
        {
            if (output[0, i, 4] <= m_model.Confidence) return; // skip low obj_conf results

            Parallel.For(5, m_model.Dimensions, j => output[0, i, j] *= output[0, i, 4]);

            Parallel.For(5, m_model.Dimensions, k =>
            {
                if (output[0, i, k] <= m_model.MulConfidence) return; // skip low mul_conf results

                var xMin = (output[0, i, 0] - output[0, i, 2] / 2 - xPad) / gain; // unpad bbox tlx to original
                var yMin = (output[0, i, 1] - output[0, i, 3] / 2 - yPad) / gain; // unpad bbox tly to original
                var xMax = (output[0, i, 0] + output[0, i, 2] / 2 - xPad) / gain; // unpad bbox brx to original
                var yMax = (output[0, i, 1] + output[0, i, 3] / 2 - yPad) / gain; // unpad bbox bry to original

                xMin = Clamp(xMin, 0, size.Width - 0); // clip bbox tlx to boundaries
                yMin = Clamp(yMin, 0, size.Height - 0); // clip bbox tly to boundaries
                xMax = Clamp(xMax, 0, size.Width - 1); // clip bbox brx to boundaries
                yMax = Clamp(yMax, 0, size.Height - 1); // clip bbox bry to boundaries

                var label = m_model.Labels[k - 5];

                var prediction = new YoloPrediction(label, output[0, i, k])
                {
                    Rectangle = new RectF(xMin, yMin, xMax - xMin, yMax - yMin)
                };

                result.Add(prediction);
            });
        });

        return result.ToList();
    }

    /// <summary>
    /// Parses net outputs (sigmoid) to predictions.
    /// </summary>
    private List<YoloPrediction> ParseSigmoid(DenseTensor<float>[] output, (float Width, float Height) size)
    {
        var result = new ConcurrentBag<YoloPrediction>();

        var (xGain, yGain) = (m_model.Width / size.Width, m_model.Height / size.Height); // x, y gains

        var gain = Math.Min(xGain, yGain); // gain = resized / original

        var (xPad, yPad) = ((m_model.Width - size.Width * gain) / 2, (m_model.Height - size.Height * gain) / 2); // left, right pads

        Parallel.For(0, output.Length, i => // iterate model outputs
        {
            var shapes = m_model.Shapes[i]; // shapes per output

            Parallel.For(0, m_model.Anchors[0].Length, a => // iterate anchors
            {
                Parallel.For(0, shapes, y => // iterate shapes (rows)
                {
                    Parallel.For(0, shapes, x => // iterate shapes (columns)
                    {
                        var offset = (shapes * shapes * a + shapes * y + x) * m_model.Dimensions;

                        var buffer = output[i].Skip(offset).Take(m_model.Dimensions).Select(Sigmoid).ToArray();

                        if (buffer[4] <= m_model.Confidence) return; // skip low obj_conf results

                        var scores = buffer.Skip(5).Select(b => b * buffer[4]).ToList(); // mul_conf = obj_conf * cls_conf

                        var mulConfidence = scores.Max(); // max confidence score

                        if (mulConfidence <= m_model.MulConfidence) return; // skip low mul_conf results

                        var rawX = (buffer[0] * 2 - 0.5f + x) * m_model.Strides[i]; // predicted bbox x (center)
                        var rawY = (buffer[1] * 2 - 0.5f + y) * m_model.Strides[i]; // predicted bbox y (center)

                        var rawW = (float)Math.Pow(buffer[2] * 2, 2) * m_model.Anchors[i][a][0]; // predicted bbox w
                        var rawH = (float)Math.Pow(buffer[3] * 2, 2) * m_model.Anchors[i][a][1]; // predicted bbox h

                        var minmax = Rect2MinMax(new[] { rawX, rawY, rawW, rawH });

                        var xMin = Clamp((minmax[0].x - xPad) / gain, 0, size.Width - 0); // unpad, clip tlx
                        var yMin = Clamp((minmax[0].y - yPad) / gain, 0, size.Height - 0); // unpad, clip tly
                        var xMax = Clamp((minmax[1].x - xPad) / gain, 0, size.Width - 1); // unpad, clip brx
                        var yMax = Clamp((minmax[1].y - yPad) / gain, 0, size.Height - 1); // unpad, clip bry

                        var label = m_model.Labels[scores.IndexOf(mulConfidence)];

                        var prediction = new YoloPrediction(label, mulConfidence)
                        {
                            Rectangle = new RectF(xMin, yMin, xMax - xMin, yMax - yMin)
                        };

                        result.Add(prediction);
                    });
                });
            });
        });

        return result.ToList();
    }

    /// <summary>
    /// Removes overlapped duplicates (nms).
    /// </summary>
    private List<YoloPrediction> Suppress(List<YoloPrediction> items)
    {
        var result = new List<YoloPrediction>(items);

        foreach (var item in items) // iterate every prediction
        {
            foreach (var current in result.ToList().Where(current => current != item)) // make a copy for each iteration
            {
                var (rect1, rect2) = (item.Rectangle, current.Rectangle);

                var intersection = RectF.Intersect(rect1, rect2);

                var intArea = intersection.Width * intersection.Height; // intersection area
                var unionArea = rect1.Width * rect1.Height + rect2.Width * rect2.Height - intArea; // union area
                var overlap = intArea / unionArea; // overlap ratio

                if (overlap >= m_model.Overlap && item.Score >= current.Score)
                    result.Remove(current);
            }
        }

        return result;
    }

    /// <summary>
    /// Outputs value between 0 and 1.
    /// </summary>
    private static float Sigmoid(float value) => 1 / (1 + (float)Math.Exp(-value));

    /// <summary>
    /// Converts rect bbox format to min max xy.
    /// </summary>
    private static (float x, float y)[] Rect2MinMax(float[] source) => new[]
    {
        (source[0] - source[2] / 2f, source[1] - source[3] / 2f), // xy
        (source[0] + source[2] / 2f, source[1] + source[3] / 2f)  // xy
    };

    /// <summary>
    /// Returns value clamped to the inclusive range of min and max.
    /// </summary>
    private static float Clamp(float value, float min, float max) => value < min ? min : value > max ? max : value; 

    /// <summary>
    /// Dispose managed.
    /// </summary>
    private void Dispose(bool disposing)
    {
        if (disposing)
            m_inferenceSession.Dispose();
    }
}
