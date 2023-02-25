namespace Endure.Controls;

class DetailDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate? FullDetailTemplate { get; set; }

    public DataTemplate? LessDetailTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        return (Constants.Desktop ? FullDetailTemplate : LessDetailTemplate) ?? new DataTemplate();
    }
}
