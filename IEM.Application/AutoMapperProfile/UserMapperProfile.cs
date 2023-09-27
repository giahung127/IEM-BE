using AutoMapper;
using IEM.Application.Models.Auth;
using IEM.Application.Models.Users;
using IEM.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEM.Application.AutoMapperProfile
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile(IConfiguration configuration) 
        {
            CreateMap<User, UserBaseModel>();
            CreateMap<UserRegistrationModel, UserCreateModel>();
            CreateMap<UserCreateModel, User>();
        }
    }
}
