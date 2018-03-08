using BuildSkull.Contracts;
using BuildSkull.View;
using BuildSkull.ViewModel.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BuildSkull.ViewModel
{
    public class BuildSkullVM : ViewModelBase
    {
        public BuildSkullVM()
        {
            RefreshItemsCommand = new RelayCommand(ExecuteRefreshItemsCommand, CanExecuteRefreshItemsCommand);
        }

        public IBuildFacade CurrentBuildFacade { get; set; }

        public ObservableCollection<BuildItemBaseVM> Items { get; private set; }

        public RelayCommand RefreshItemsCommand { get; set; }

        public void Initialize()
        {
            Items = new ObservableCollection<BuildItemBaseVM>();
            if (CurrentBuildFacade != null)
            {
                CurrentBuildFacade.LogInCompleted -= OnBuildFacadeLogInCompleted;
                CurrentBuildFacade.BuildListLoaded -= OnBuildFacadeBuildListLoaded;
                CurrentBuildFacade.BuildStatusChanged -= OnBuildFacadeBuildStatusLoaded;
                CurrentBuildFacade.LogInCompleted += OnBuildFacadeLogInCompleted;
                CurrentBuildFacade.BuildListLoaded += OnBuildFacadeBuildListLoaded;
                CurrentBuildFacade.BuildStatusChanged += OnBuildFacadeBuildStatusLoaded;

                CurrentBuildFacade.LogIn(
                    new RepositoryUser() { Name = "", Password = "" },
                    //new RepositoryInfo() { ProjectName = "VSBuildAddin", ServerUrl = new Uri("https://restcode.visualstudio.com") });
                    new RepositoryInfo() { ProjectName = "", ServerUrl = null });
            }
        }

        private void OnBuildFacadeLogInCompleted(object sender, LogInCompletedEventArgs e)
        {
            if (e.IsSucceeded)
            {
                LoadBuildList();
            }
            else
            {
                MessageBox.Show("User Not Authorized!");
            }
        }

        private void LoadBuildList()
        {
            if (CurrentBuildFacade != null)
            {
                CurrentBuildFacade.LoadBuildList();
            }
        }

        private void OnBuildFacadeBuildListLoaded(object sender, BuildListLoadedEventArgs e)
        {
            var newBuilds = GetBuildTree(e.BuildList);
            App.Dispatcher.Invoke(() =>
            {
                Items.Clear();
                foreach (var newBuild in newBuilds)
                {
                    Items.Add(newBuild);
                }
            });
            RefreshItemsCommand.RaiseCanExecuteChanged();
        }

        private void OnBuildFacadeBuildStatusLoaded(object sender, BuildStatusChangedEventArgs e)
        {
            var crashedBuilds = new List<BuildCrashInfo>();
            foreach (var build in e.BuildDefinitions)
            {
                var buildDefinitionVm = SearchBuildById(Items, build.Id);
                if (buildDefinitionVm != null)
                {
                    buildDefinitionVm.Model = build.Clone();
                    if (build.LastRun != null)
                    {
                        buildDefinitionVm.BuildStatus = build.LastRun.BuildStatus;
                    }

                    var buildLastRun = build.LastRun;
                    if (buildLastRun != null)
                    {
                        if ((buildLastRun.BuildStatus == BuildStatusType.Failed)
                            && CurrentBuildFacade.CurrentUser.Equals(buildLastRun.CommittedBy))
                        {
                            var crashedBuildInfo = new BuildCrashInfo()
                            {
                                User = CurrentBuildFacade.CurrentUser,
                                BuildDefinition = buildDefinitionVm,
                                BuildRun = buildLastRun,
                            };
                            crashedBuilds.Add(crashedBuildInfo);
                        }
                    }
                }
            }

            if (crashedBuilds.Count > 0)
            {
                BuildCrashedDialog.ShowDialog(crashedBuilds.ToArray());
            }
        }

        public BuildDefinitionItemVM SearchBuildById(IEnumerable<BuildItemBaseVM> tree, string id)
        {
            BuildDefinitionItemVM result = null;
            foreach (var buildItem in tree)
            {
                if (buildItem.Id == id)
                {
                    result = buildItem as BuildDefinitionItemVM;
                }
                else if (buildItem is BuildCatalogItemVM)
                {
                    var catalog = buildItem as BuildCatalogItemVM;
                    result = SearchBuildById(catalog.Children, id);
                }
                if (result != null) { break; }
            }
            return result;
        }

        public BuildItemBaseVM[] GetBuildTree(IEnumerable<BuildDefinitionItem> builds)
        {
            var result = GetBuildTreeReq(builds, 0);
            return result;
        }

        public BuildItemBaseVM[] GetBuildTreeReq(IEnumerable<BuildDefinitionItem> builds, int level)
        {
            var newItemsTree = new List<BuildItemBaseVM>();

            string[] levelNames = (
                from build in builds
                select build.HierarchyName[level])
                .Distinct()
                .ToArray();

            foreach (var levelName in levelNames)
            {
                var catalogBuilds = builds
                    .Where(b => b.HierarchyName[level] == levelName && b.HierarchyName.Length > level + 1)
                    .ToArray();

                var definitionBuilds = builds
                    .Where(b => b.HierarchyName[level] == levelName && b.HierarchyName.Length == level + 1)
                    .ToArray();

                if (catalogBuilds.Length > 0)
                {
                    var newBuildCatalog = new BuildCatalogItemVM()
                    {
                        Name = levelName,
                    };
                    newItemsTree.Add(newBuildCatalog);
                    var children = GetBuildTreeReq(catalogBuilds, level + 1);

                    foreach (var child in children)
                    {
                        newBuildCatalog.Children.Add(child);
                    }
                }
                
                //add single builds
                foreach (var definitionBuild in definitionBuilds)
                {
                    var newDefinitionBuild = new BuildDefinitionItemVM(CurrentBuildFacade)
                    {
                        Id = definitionBuild.Id,
                        Name = definitionBuild.HierarchyName[level],
                        Model = definitionBuild,
                        
                    };
                    newItemsTree.Add(newDefinitionBuild);
                }
            }

            return newItemsTree.ToArray();
        }

        public void ExecuteRefreshItemsCommand()
        {
            if (CanExecuteRefreshItemsCommand())
            {
                LoadBuildList();
            }
        }

        public bool CanExecuteRefreshItemsCommand()
        {
            bool canExecute = CurrentBuildFacade != null && CurrentBuildFacade.IsAuthenticated;
            return canExecute;
        }
    }
}
