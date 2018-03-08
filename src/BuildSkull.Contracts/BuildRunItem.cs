using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSkull.Contracts
{
    public class BuildRunItem : ICloneable
    {
        public string BuildNumber { get; set; }
        
        /// <summary>
        /// Status of this build run
        /// </summary>
        public BuildStatusType BuildStatus { get; set; }

        public BuildCompilationStatusType CompilationStatus { get; set; }

        public RepositoryUser TriggeredBy { get; set; }
        public RepositoryUser LastChangedBy { get; set; }
        public string Revision { get; set; }
        public RepositoryUser CommittedBy { get; set; }
        

        /// <summary>
        /// Date of current build run (when triggered)
        /// </summary>
        public DateTime? StartTime { get; set; }

        public DateTime? FinishTime { get; set; }

        public BuildRunItem Clone()
        {
            return new BuildRunItem() 
            {
                BuildStatus = this.BuildStatus,
                CompilationStatus = this.CompilationStatus,
                FinishTime = this.FinishTime, 
                LastChangedBy = this.LastChangedBy, 
                StartTime = this.StartTime, 
                TriggeredBy = this.TriggeredBy,
                Revision = this.Revision,
                CommittedBy = this.CommittedBy
            };
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
