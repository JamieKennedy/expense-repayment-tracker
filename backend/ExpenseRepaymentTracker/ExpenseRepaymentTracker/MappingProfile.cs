using AutoMapper;
using Common.DataTransferObjects.User;
using Entities.Models;

namespace ExpenseRepaymentTracker
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User
            CreateMap<UserRegistrationDto, User>();
        }
    }
}