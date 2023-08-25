using IEM.Domain.Entities;

namespace IEM.Application.Interfaces
{
    public interface IUserService
    {
        public ValueTask<IEnumerable<User>> GetAllUsers();
    }
}
