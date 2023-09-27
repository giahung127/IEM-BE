using IEM.Domain.Core.Repositories;
using IEM.Domain.Entities;
using IEM.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEM.Infrastructure.Repositories
{
    internal class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDatabaseContext dbContext) : base(dbContext)
        {
        }
    }
}
