using BuildSkull.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BuildSkull.Converters
{
    public class BuildItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BuildDefinitionTemplate { get; set; }
        public HierarchicalDataTemplate BuildCatalogTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is BuildDefinitionItemVM)
            {
                return BuildDefinitionTemplate;
            }
            else if (item is BuildCatalogItemVM)
            {
                //if (BuildCatalogTemplate != null)
                //{
                //    BuildCatalogTemplate.ItemTemplate = BuildDefinitionTemplate;
                //}
                return BuildCatalogTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
