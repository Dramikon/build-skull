using BuildSkull.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSkull.ViewModel
{
    public class BuildItemBaseVM : ViewModelBase
    {
        public string Id { get; set; }

        public string Name { get; set; }

        private BuildStatusType _BuildStatus;
        public BuildStatusType BuildStatus
        {
            get { return _BuildStatus; }
            set
            {
                if (value != _BuildStatus)
                {
                    _BuildStatus = value;
                    NotifyPropertyChanged("BuildStatus");
                }
            }
        }
    }
}
