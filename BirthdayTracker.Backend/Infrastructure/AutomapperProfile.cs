using AutoMapper;
using BirthdayTracker.Shared.Entities;
using BirthdayTracker.Shared.Models.Request;
using BirthdayTracker.Shared.Models.Response;
using BirthdayTracker.Shared.Requests;

namespace BirthdayTracker.Backend.Infrastructure
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            MapRequest();
            MapResponse();
        }

        private void MapRequest()
        {
            CreateMap<AddUserRequest, AppUser>();
            CreateMap<UpdateUserRequest, AppUser>();
            CreateMap<CompanyOwnerRequest, AppUser>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => string.Join('_', src.Name, src.Surname)))
                .ForMember(dest => dest.NormalizedUserName, opt => opt.MapFrom(src => string.Join('_', src.Name, src.Surname).ToUpper()))
                .ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(src => src.Email.ToUpper()));
        }

        private void MapResponse()
        {
            CreateMap<Employee, EmployeeResponse>();
        }
    }
}
