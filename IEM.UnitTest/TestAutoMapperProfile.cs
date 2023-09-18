using AutoMapper;
using IEM.Application.Models.Users;
using IEM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEM.UnitTest
{
    public class TestAutoMapperProfile : Profile
    {
        public TestAutoMapperProfile() 
        {
            CreateMap<User, UserBaseModel>();
        }
    }
}
