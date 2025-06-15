﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string Username { get; set; }   

        public string Email { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public ICollection<string> Roles { get; set; }

    }
}
