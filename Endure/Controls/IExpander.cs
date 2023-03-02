namespace Endure.Controls;

/// <summary>
/// Expander expand direction.
/// </summary>
public enum ExpandDirection
{
    /// <summary>
    /// Expander expands down
    /// </summary>
    Down,

    /// <summary>
    /// Expander expands up
    /// </summary>
    Up
}

public interface IExpander : IContentView
{
    /// <summary>
    /// Expander header.
    /// </summary>
    public IView? Header { get; }

    /// <summary>
    /// Gets or sets expand direction.
    /// </summary>
    public ExpandDirection Direction { get; }

    /// <summary>
    /// Gets or sets Expander collapsible state.
    /// </summary>
    public bool IsExpanded { get; set; }

    /// <summary>
    /// Action when <see cref="IsExpanded"/> changes
    /// </summary>
    void ExpandedChanged(bool isExpanded);
}