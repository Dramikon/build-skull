using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuildSkull.Contracts
{
    public class RepositoryUser
    {
        public string Name { get; set; }
        public string Password { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is RepositoryUser)
            {
                bool result = Equals(obj as RepositoryUser);
                return result;
            }
            return false;
        }

        public bool Equals(RepositoryUser user)
        {
            if (user == null)
            {
                return false;
            }
            bool result = this.Name == user.Name;
            return result;
        }
    }
}
