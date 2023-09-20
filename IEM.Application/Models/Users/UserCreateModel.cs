using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEM.Application.Models.Users
{
    public class UserCreateModel
    {
        public string? Name { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Password { get; set; }
    }
}
