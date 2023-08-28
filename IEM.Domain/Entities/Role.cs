﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEM.Domain.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string? RoleName { get; set; }
        public int RoleCode { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
