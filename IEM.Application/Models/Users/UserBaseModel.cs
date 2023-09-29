using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEM.Application.Models.Users
{
    public class UserBaseModel
    {
        public Guid UserId { get; set; }
        public string? Email { get; set; }
    }
}
