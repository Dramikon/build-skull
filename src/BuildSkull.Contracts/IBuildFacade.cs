using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSkull.Contracts
{
    /// <summary>
    /// Interface to communicate with build repository, continuous integration to get information about build statuses
    /// </summary>
    public interface IBuildFacade
    {
        void LogIn(RepositoryUser user, RepositoryInfo server);
        void LogOut();

        bool IsAuthenticated { get; }
        RepositoryUser CurrentUser { get; }

        void LoadBuildList();

        void RunBuild(string id);
        void StopBuild(string id);

        event EventHandler<BuildListLoadedEventArgs> BuildListLoaded;
        event EventHandler<BuildStatusChangedEventArgs> BuildStatusChanged;
        event EventHandler<LogInCompletedEventArgs> LogInCompleted;
    }
}
