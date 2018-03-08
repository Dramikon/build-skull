using BuildSkull.Contracts;
using Microsoft.TeamFoundation.Build.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSkull.TFS
{
    public class TFSBuildRunItem : BuildRunItem
    {
        public IBuildDetail TFSBuildLastDetails { get; set; }
    }
}
