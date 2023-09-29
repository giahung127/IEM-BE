using AutoMapper;
using Castle.Core.Logging;
using IEM.Application.Interfaces;
using IEM.Application.Models.Commons;
using IEM.Application.Models.Users;
using IEM.Application.Services;
using IEM.Domain.Core.Repositories;
using IEM.Domain.Entities;
using IEM.WebAPI.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
                .ReturnsAsync(_mapper.Map<IEnumerable<UserBaseModel>>(users));

            var userController = new UsersController(_userService.Object);

            //Act
            var usersResult = await userController.GetAllUsers();

            //Assert
            Assert.Equal(200 ,usersResult.StatusCode);
            Assert.IsType<ApiResponseModel<IEnumerable<UserBaseModel>>>(usersResult);
            Assert.Equal(users.Count(), ((IApiResponseModel<IEnumerable<UserBaseModel>>)usersResult).Data.Count());
        }

        [Fact]
        public async void ShouldGetAllUsers()
        {
            //Arrange
            var users = GetUsersData();
            var serviceCollection = new ServiceCollection();
            var unitOfWork = new Mock<IUnitOfWork>();
            var repository = new Mock<IUserRepository>();
            var loggerFac = new LoggerFactory();

            repository.Setup(x => x.ToListAsync())
                .ReturnsAsync(users);
            unitOfWork.Setup(x => x.Users)
                .Returns(repository.Object);

            var logger = new Mock<Logger<UserService>>();
            serviceCollection.AddScoped<IUnitOfWork>(provider => unitOfWork.Object);
            serviceCollection.AddScoped<IMapper>(provider => _mapper);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var userService = new UserService(serviceProvider, loggerFac.CreateLogger<UserService>());

            // Act
            var result = await userService.GetAllUsers();

            //
            Assert.Equal(users.Count(), result.Count());
        }

        private List<User> GetUsersData()
        {
            var users = new List<User>()
            {
                new User
                {
                    UserId = new Guid(),
                    Name = "Test1",
                    Mobile = "123456789",
                    Email = "test1@iem.com",
                    Password = string.Empty,
                },
                new User
                {
                    UserId = new Guid(),
                    Name = "Test2",
                    Mobile = "123456789",
                    Email = "test2@iem.com",
                    Password = string.Empty,
                }
            };

            return users;
        }
    }
}
