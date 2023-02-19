namespace Endure.Models.Yolo;

public class YoloPrediction
{
    /// <summary>
    /// The bounding rectangle.
    /// <para>left, top, right, bottom</para>
    /// </summary>
    public RectF Rectangle { get; set; }

    /// <summary>
    /// The Box category.
    /// </summary>
    public YoloLabel Label { get; set; }

    /// <summary>
    /// Confidence level.
    /// </summary>
    public float Score { get; set; }

    public YoloPrediction(YoloLabel label) => Label = label;

    public YoloPrediction(YoloLabel label, float confidence) : this(label) => Score = confidence;
}