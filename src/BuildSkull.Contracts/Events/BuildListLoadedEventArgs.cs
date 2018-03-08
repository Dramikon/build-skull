using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuildSkull.Contracts
{
    public class BuildListLoadedEventArgs : EventArgs
    {
        public List<BuildDefinitionItem> BuildList { get; set; }
    }
}
