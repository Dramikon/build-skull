using BuildSkull.Contracts;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BuildSkull.TFS
{
    public class TFSBuildFacade : IBuildFacade
    {
        public RepositoryUser CurrentUser { get; set; }
        private RepositoryInfo CurrentServer { get; set; }

        private List<TFSBuildDefinitionItem> BuildList { get; set; }
        private TfsTeamProjectCollection TeamProjectCollection { get; set; }
        private ProjectInfo[] SelectedProjects { get; set; }
        private VersionControlServer TeamVersionControl { get; set; }

        private IBuildServer BuildServer = null;
        private Timer _backgroundTimer = null;
        private bool _isBackgroundProcessRunning = false;
        
        #region Interface Implementation

        public void LogIn(RepositoryUser user, RepositoryInfo server)
        {
            //if (user == null) { throw new ArgumentNullException("user"); }
            //if (server == null) { throw new ArgumentNullException("server"); }
            //if (server.ServerUrl == null) { throw new ArgumentNullException("server.ServerUrl"); }
            //if (string.IsNullOrEmpty(server.ProjectName)) { throw new ArgumentNullException("server.ProjectName"); }
            
            CurrentUser = user;
            CurrentServer = server;

            ThreadPool.QueueUserWorkItem(LogInAsync);
        }

        private void LogInAsync(object state)
        {
            // The  user is allowed to select only one project
            var tfsPicker = new TeamProjectPicker(TeamProjectPickerMode.SingleProject, false);
            if (tfsPicker.SelectedTeamProjectCollection == null)
            {
                var tResult = tfsPicker.ShowDialog();
            }
            
            //TfsConfigurationServer confServer = new TfsConfigurationServer(CurrentServer.ServerUrl);

            //ReadOnlyCollection<CatalogNode> collectionNodes = confServer.CatalogNode.QueryChildren(
            //    new[] { CatalogResourceTypes.ProjectCollection }, false, CatalogQueryOptions.None);

            //if (collectionNodes == null || collectionNodes.Count() == 0) { throw new ApplicationException("Project list in TFS repo is empty"); }

            //// it returns collection, but it should be only one node for project. and we need first
            //var collectionNode = collectionNodes[0];

            //Guid collectionId = new Guid(collectionNode.Resource.Properties["InstanceId"]);

            //get current project from TFS repo
            //TeamProjectCollection = confServer.GetTeamProjectCollection(collectionId);

            TeamProjectCollection = tfsPicker.SelectedTeamProjectCollection;
            SelectedProjects = tfsPicker.SelectedProjects;
            TeamProjectCollection.EnsureAuthenticated();
            BuildServer = TeamProjectCollection.GetService<IBuildServer>();

            TeamVersionControl = TeamProjectCollection.GetService<VersionControlServer>();
            if (TeamVersionControl != null)
            {
                //CurrentUser.Name = TeamVersionControl.AuthorizedUser;     //AuthorizedUser = 'valentin_kononov@epam.com'
                CurrentUser.Name = TeamVersionControl.AuthenticatedUser;    //AuthenticatedUser = 'Valentin Kononov'
            }

            if (LogInCompleted != null)
            {
                LogInCompleted(this, new LogInCompletedEventArgs() { IsSucceeded = true, User = CurrentUser });
            }

            _backgroundTimer = new Timer(MonitorBuilds, null, new TimeSpan(0, 0, 10), new TimeSpan(0, 0, 10));
        }

        public void LogOut()
        {
            //throw new NotImplementedException();
        }

        public void LoadBuildList()
        {
            ThreadPool.QueueUserWorkItem(LoadBuildListAsync);
        }

        private void LoadBuildListAsync(object state)
        {
            BuildList = GetBuildList();

            if (BuildListLoaded != null)
            {
                BuildListLoaded(this, new BuildListLoadedEventArgs() { BuildList = this.BuildList.Cast<BuildDefinitionItem>().ToList() });
            }

            MonitorBuilds(null);
        }

        public List<TFSBuildDefinitionItem> GetBuildList()
        {
            _isBackgroundProcessRunning = true;
            var curPrj = SelectedProjects.FirstOrDefault();
            if (curPrj == null) { throw new ApplicationException("No one TFS project has been selected"); }
            var buildDefs = BuildServer.QueryBuildDefinitions(curPrj.Name);

            //convert flat build list to our contracts
            var result = new List<TFSBuildDefinitionItem>(buildDefs.Length);
            foreach (var item in buildDefs)
            {
                var newDef = new TFSBuildDefinitionItem()
                {
                    Name = item.Name,
                    HierarchyName = item.Name.Split('.'),
                    Id = item.Id,
                    Description = item.Description,
                    TFSBuildDefInstance = item,
                    LastRun = new TFSBuildRunItem() { BuildStatus = BuildStatusType.None }
                };

                result.Add(newDef);
            }
            _isBackgroundProcessRunning = false;
            return result;
        }

        private void MonitorBuilds(object state)
        {
            if (!_isBackgroundProcessRunning)
            {
                _isBackgroundProcessRunning = true;

                List<BuildDefinitionItem> crashedBuilds = new List<BuildDefinitionItem>();

                foreach (var buildItem in BuildList)
                {
                    BuildRunItem newLastRun = null;

                    // get last run for particular build
                    //var details = buildItem.TFSBuildDefInstance.QueryBuilds();
                    
                    IBuildDetailSpec spec = BuildServer.CreateBuildDetailSpec("*");
                    spec.InformationTypes = null; // for speed improvement
                    //spec.MinFinishTime = DateTime.Now.AddDays(-21); //to get only builds of last 3 weeks
                    spec.MaxBuildsPerDefinition = 1; //get only one build per build definintion
                    spec.QueryOrder = BuildQueryOrder.FinishTimeDescending; //get the latest build only
                    spec.QueryOptions = QueryOptions.All;
                    spec.DefinitionSpec.Name = buildItem.Name;

                    var details = BuildServer.QueryBuilds(spec).Builds;

                    if (details != null)
                    {
                        newLastRun = GetBuildRunInfo(details);

                        //check that status changed
                        if (newLastRun != null && buildItem.LastRun != null
                            && (
                            newLastRun.BuildNumber != buildItem.LastRun.BuildNumber
                            ||
                            newLastRun.BuildStatus != buildItem.LastRun.BuildStatus
                            )
                         )
                        {
                            buildItem.LastRun = newLastRun;

                            crashedBuilds.Add(buildItem);
                        }
                    }
                }

                if (BuildStatusChanged != null)
                {
                    BuildStatusChanged(this, new BuildStatusChangedEventArgs() { BuildDefinitions = crashedBuilds.ToArray() });
                }

                _isBackgroundProcessRunning = false;
            }
        }

        private static BuildRunItem GetBuildRunInfo(IBuildDetail[] details)
        {
            BuildRunItem newLastRun = null;
            var lastRunTFS = details.FirstOrDefault();

            if (lastRunTFS != null)
            {
                newLastRun = new TFSBuildRunItem()
                {
                    BuildNumber = lastRunTFS.BuildNumber,
                    StartTime = lastRunTFS.StartTime,
                    FinishTime = lastRunTFS.FinishTime,
                    BuildStatus = (BuildStatusType)((int)lastRunTFS.Status),                                    //TODO: implement proper conversion
                    CompilationStatus = (BuildCompilationStatusType)((int)lastRunTFS.CompilationStatus),        //TODO: implement proper conversion
                    TriggeredBy = new RepositoryUser() { Name = lastRunTFS.RequestedBy },
                    LastChangedBy = new RepositoryUser() { Name = lastRunTFS.LastChangedBy },
                    CommittedBy = new RepositoryUser() { Name = lastRunTFS.RequestedFor },
                    Revision = lastRunTFS.SourceGetVersion,
                    TFSBuildLastDetails = lastRunTFS
                };

                //int changeId = InformationNodeConverters.GetChangesetId(lastRunTFS.Information);
                //if (changeId > 0)
                //{
                //    //check who last changed build
                //    Changeset verItem = TeamVersionControl.GetChangeset(changeId);
                //    newLastRun.CommittedBy = new RepositoryUser() { Name = verItem.Committer };
                //}
            }
            return newLastRun;
        }

        #endregion

        #region Events

        public event EventHandler<BuildListLoadedEventArgs> BuildListLoaded;
        public event EventHandler<BuildStatusChangedEventArgs> BuildStatusChanged;
        public event EventHandler<LogInCompletedEventArgs> LogInCompleted;

        #endregion


        public bool IsAuthenticated
        {
            get { return true; }
        }


        public void RunBuild(string id)
        {
            if (BuildList == null) { throw new ApplicationException("builds collection is not initialized"); }

            var buildToRun = BuildList.Find(b => b.Id == id);
            if (buildToRun != null)
            {
                var queuedBuild = BuildServer.QueueBuild(buildToRun.TFSBuildDefInstance);

                if (BuildStatusChanged != null)
                {
                    //need to get fresh build info

                    var details = buildToRun.TFSBuildDefInstance.QueryBuilds();
                    if (details != null && details.Length == 0)
                    {
                        // build is still in queue
                        var newLastRun = GetBuildRunInfo(details);
                        buildToRun.LastRun = newLastRun;

                        BuildStatusChanged(this, new BuildStatusChangedEventArgs() { BuildDefinitions = new BuildDefinitionItem[] { buildToRun } });
                    }
                }
            }
        }

        public void StopBuild(string id)
        {
            if (BuildList == null) { throw new ApplicationException("builds collection is not initialized"); }
            var buildToStop = BuildList.Find(b => b.Id == id);

            if (buildToStop != null)
            {
                BuildServer.StopBuilds(new IBuildDetail[]{ ((TFSBuildRunItem)buildToStop.LastRun).TFSBuildLastDetails } );

                if (BuildStatusChanged != null)
                {
                    buildToStop.LastRun.BuildStatus = BuildStatusType.Stopped;
                    BuildStatusChanged(this, new BuildStatusChangedEventArgs() { BuildDefinitions = new BuildDefinitionItem[] { buildToStop } });
                }
            }
        }
    }
}
