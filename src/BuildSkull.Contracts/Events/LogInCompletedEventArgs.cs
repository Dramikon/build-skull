using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSkull.Contracts
{
    public class LogInCompletedEventArgs : EventArgs
    {
        public bool IsSucceeded { get; set; }
        public RepositoryUser User { get; set; }
    }
}
