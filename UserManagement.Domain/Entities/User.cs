using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }

        public User() 
        { 
            Id = Guid.NewGuid();

            CreateAt = DateTime.UtcNow;

            IsActive = true;

            UserRoles = new HashSet<UserRole>();
        }
    }
}
