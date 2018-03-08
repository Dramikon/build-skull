using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSkull.Contracts
{
    public class BuildDefinitionItem : ICloneable
    {
        /// <summary>
        /// Unique Id for build definition
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Name for build definition
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name for build definition
        /// </summary>
        public string[] HierarchyName { get; set; }

        /// <summary>
        /// Description for build definition
        /// </summary>
        public string Description { get; set; }        

        public BuildRunItem LastRun { get; set; }

        public BuildDefinitionItem Clone()
        {
            var result = new BuildDefinitionItem() 
            {
                Id = this.Id,
                Name = this.Name,
                HierarchyName = this.HierarchyName, 
                Description = this.Description
            };

            if(this.LastRun != null)
            {
                result.LastRun = this.LastRun.Clone();
            }
            return result;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
