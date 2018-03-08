using BuildSkull.Contracts;
using BuildSkull.ViewModel;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BuildSkull.View
{
    public class BuildCrashInfo
    {
        public RepositoryUser User { get; set; }
        public BuildDefinitionItemVM BuildDefinition { get; set; }
        public BuildRunItem BuildRun { get; set; }
    }

    /// <summary>
    /// Interaction logic for BuildCrashedDialog.xaml
    /// </summary>
    public partial class BuildCrashedDialog : DialogWindow
    {
        public BuildCrashedDialog()
        {
            InitializeComponent();
            Builds = new ObservableCollection<BuildCrashInfo>();
            Loaded += OnLoaded;
        }

        public ObservableCollection<BuildCrashInfo> Builds { get; set; }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Root.InvalidateMeasure();
            //this.Height = BuildsItemsControl.ActualHeight
        }

        public static void ShowDialog(BuildCrashInfo[] builds)
        {
            App.Dispatcher.InvokeAsync(() =>
            {
                var view = new BuildCrashedDialog();
                if (builds != null)
                {
                    foreach (var build in builds)
                    {
                        view.Builds.Add(build);
                    }
                }
                view.DataContext = view;
                view.Show();
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
