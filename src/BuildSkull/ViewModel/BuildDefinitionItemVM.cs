using BuildSkull.Contracts;
using BuildSkull.ViewModel.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSkull.ViewModel
{
    public class BuildDefinitionItemVM : BuildItemBaseVM
    {
        private IBuildFacade CurrentBuildFacade { get; set; }

        public BuildDefinitionItemVM(IBuildFacade buildFacade)
        {
            CurrentBuildFacade = buildFacade;
            RunCommand = new RelayCommand(ExecuteRunCommand, CanExecuteRunCommand);
            StopCommand = new RelayCommand(ExecuteStopCommand, CanExecuteStopCommand);
        }

        private BuildDefinitionItem _Model;
        public BuildDefinitionItem Model
        {
            get { return _Model; }
            set
            {
                if (value != _Model)
                {
                    _Model = value;
                    NotifyPropertyChanged("Model");
                    RunCommand.RaiseCanExecuteChanged();
                    StopCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public RelayCommand RunCommand { get; set; }

        public RelayCommand StopCommand { get; set; }

        private void ExecuteRunCommand()
        {
            if (CanExecuteRunCommand())
            {
                CurrentBuildFacade.RunBuild(Model.Id);
            }
        }

        private bool CanExecuteRunCommand()
        {
            bool canExecute = Model != null && (Model.LastRun == null || Model.LastRun.BuildStatus != BuildStatusType.InProgress);
            return canExecute;
        }

        private void ExecuteStopCommand()
        {
            if (CanExecuteStopCommand())
            {
                CurrentBuildFacade.StopBuild(Model.Id);
            }
        }

        private bool CanExecuteStopCommand()
        {
            bool canExecute = Model != null && Model.LastRun != null && Model.LastRun.BuildStatus == BuildStatusType.InProgress;
            return canExecute;
        }
    }
}
