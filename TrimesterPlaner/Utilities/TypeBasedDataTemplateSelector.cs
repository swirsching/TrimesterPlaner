using System.Windows;
using System.Windows.Controls;

namespace TrimesterPlaner.Utilities
{
    public class TypeBasedDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is not ContentPresenter presenter)
            {
                return base.SelectTemplate(item, container);
            }

            if (presenter.TemplatedParent is not ContentControl control)
            {
                return base.SelectTemplate(item, container);
            }

            Type? type = item?.GetType();
            if (type is null)
            {
                return base.SelectTemplate(item, container);
            }

            foreach (DataTemplate template in Templates)
            {
                if ((Type)template.DataType == type)
                {
                    return template;
                }
            }

            return base.SelectTemplate(item, container);
        }

        public List<DataTemplate> Templates { get; set; } = [];
    }
}