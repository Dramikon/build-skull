using BuildSkull.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BuildSkull
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class BuildSkullControl : UserControl
    {
        private bool _isAllExpanded = true;

        public BuildSkullControl()
        {
            InitializeComponent();
            App.Dispatcher = Dispatcher;
            Loaded += OnLoaded;

            var vm = new BuildSkullVM();
            vm.CurrentBuildFacade = new TFS.TFSBuildFacade();
            vm.Initialize();
            DataContext = vm;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format(System.Globalization.CultureInfo.CurrentUICulture, "We are inside {0}.button1_Click()", this.ToString()),
                            "Build Desk");

        }

        private void OnExpandCollapseClick(object sender, RoutedEventArgs e)
        {
            if (ItemsTreeView.Items.Count > 0)
            {
                if (_isAllExpanded)
                {
                    ExpandAll(ItemsTreeView, false);
                    _isAllExpanded = false;
                }
                else
                {
                    ExpandAll(ItemsTreeView, true);
                    _isAllExpanded = true;
                }
            }
        }

        private void ExpandAll(ItemsControl items, bool expand)
        {
            foreach (object obj in items.Items)
            {
                ItemsControl childControl = items.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
                if (childControl != null)
                {
                    ExpandAll(childControl, expand);
                }
                TreeViewItem item = childControl as TreeViewItem;
                if (item != null)
                {
                    item.Dispatcher.InvokeAsync(() => item.IsExpanded = expand);
                }
            }
        }
    }
}