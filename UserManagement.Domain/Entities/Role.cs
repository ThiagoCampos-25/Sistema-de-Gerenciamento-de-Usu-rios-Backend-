using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
        public Role()
        {
            Id = Guid.NewGuid();
            UserRoles = new HashSet<UserRole>();
        }
    }
}
