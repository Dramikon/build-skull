using BuildSkull.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSkull.ViewModel
{
    public class BuildCatalogItemVM : BuildItemBaseVM
    {
        public BuildCatalogItemVM()
        {
            Children = new ObservableCollection<BuildItemBaseVM>();
            Children.CollectionChanged += OnChildrenCollectionChanged;
        }

        private void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var child in Children)
            {
                child.PropertyChanged += OnChildPropertyChanged;
            }
        }

        private void OnChildPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "BuildStatus")
            {
                bool isAnyFailed = Children.Any(c => c.BuildStatus == Contracts.BuildStatusType.Failed);
                this.BuildStatus = isAnyFailed ? BuildStatusType.Failed : BuildStatusType.Succeeded;
            }
        }

        public ObservableCollection<BuildItemBaseVM> Children { get; private set; }
    }
}
