using Endure.Models.Yolo;
using Microsoft.Maui.Graphics.Skia;
using Microsoft.ML.OnnxRuntime.Tensors;
using SkiaSharp;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace Endure.Services.Yolo;

public class ImageTensor
{
    public SKBitmap Bitmap { get; }

    public DenseTensor<float> Tensor { get; }

    public ImageTensor(IImage image, YoloModel model)
    {
        Bitmap = ResizeImage(NormalizeImage(image).PlatformRepresentation, (model.Width, model.Width));
        Tensor = new DenseTensor<float>(new[] { 1, 3, model.Height, model.Width });

        var pixels = Bitmap.Pixels;

        Parallel.For(0, Bitmap.Height, y =>
        {
            Parallel.For(0, Bitmap.Width, x =>
            {
                var color = pixels[x + y * Bitmap.Width];
                Tensor[0, 0, y, x] = color.Red;
                Tensor[0, 1, y, x] = color.Green;
                Tensor[0, 2, y, x] = color.Blue;
            });
        });
    }

    private SKBitmap ResizeImage(SKBitmap bitmap, (int Width, int Height) size)
    {
        if (bitmap.Width != size.Width || bitmap.Height != size.Height)
            return Bitmap.Resize(new SKImageInfo(size.Width, size.Height), SKFilterQuality.None);
        return Bitmap;
    }

    private static SkiaImage NormalizeImage(IImage image)
    {
        if (image is SkiaImage skiaImage) return skiaImage;

        return SkiaImage.FromStream(image.AsStream()) as SkiaImage
               ?? throw new InvalidCastException("Invalid Image instance");
    }
}