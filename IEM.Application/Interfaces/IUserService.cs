using IEM.Application.Models.Users;
using IEM.Domain.Entities;

namespace IEM.Application.Interfaces
{
    public interface IUserService
    {
        public ValueTask<IEnumerable<UserBaseModel>> GetAllUsers();
    }
}
