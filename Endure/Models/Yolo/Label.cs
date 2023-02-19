namespace Endure.Models.Yolo;

public class YoloLabel
{
    /// <summary>
    /// The prediction box id of label.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The prediction box label name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The prediction box color.
    /// </summary>
    public Color Color { get; set; }

    public YoloLabel() => Color = new Color(255, 255, 0);
}