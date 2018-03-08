using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuildSkull.Contracts
{
    public class BuildStatusChangedEventArgs : EventArgs
    {
        public BuildDefinitionItem[] BuildDefinitions { get; set; }
    }
}
