using AutoMapper;
using Castle.Core.Logging;
using IEM.Application.Interfaces;
using IEM.Application.Models.Commons;
using IEM.Application.Models.Users;
using IEM.Domain.Core.Repositories;
using IEM.Domain.Entities;
using IEM.WebAPI.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Moq;


namespace IEM.UnitTest.UserTest
{
    public class UserControllerTest
    {

        private readonly Mock<IUserService> _userService;
        private readonly IMapper _mapper;
        public UserControllerTest()
        {
            _userService = new Mock<IUserService>();
            var myProfile = new TestAutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            _mapper = new Mapper(configuration);
        }

        [Fact]
        public async void ShouldGetUsers()
        {
            //Arrange
            var users = GetUsersData();
            _userService.Setup(x => x.GetAllUsers())
                .ReturnsAsync(users);

            var userController = new UsersController(_userService.Object, _mapper);

            //Act
            var usersResult = await userController.GetAllUsers();

            //Assert
            Assert.Equal(200 ,usersResult.StatusCode);
            Assert.IsType<ApiResponseModel<IEnumerable<UserBaseModel>>>(usersResult);
            Assert.Equal(users.Count(), ((IApiResponseModel<IEnumerable<UserBaseModel>>)usersResult).Data.Count());
        }

        private List<User> GetUsersData()
        {
            var users = new List<User>()
            {
                new User
                {
                    Id = 1,
                    Name = "Test1",
                    Mobile = "123456789",
                    Email = "test1@iem.com"
                },
                new User
                {
                    Id = 2,
                    Name = "Test2",
                    Mobile = "123456789",
                    Email = "test2@iem.com"
                }
            };

            return users;
        }
    }
}
