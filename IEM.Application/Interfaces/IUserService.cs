using IEM.Application.Models.Users;
using IEM.Domain.Entities;

namespace IEM.Application.Interfaces
{
    public interface IUserService
    {
        public ValueTask<IEnumerable<UserBaseModel>> GetAllUsers();
        ValueTask<User> CreateUserAsync(UserCreateModel model);
        ValueTask<UserSingleModel> GetSingleUserAysnc(Guid userId);
        ValueTask<bool> CheckUserExistsAsync(string Email);
    }
}
