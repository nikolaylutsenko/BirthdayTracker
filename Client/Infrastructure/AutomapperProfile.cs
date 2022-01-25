using AutoMapper;
using BirthdayTracker.Shared.Entities;
using BirthdayTracker.Shared.Models.Request;
using BirthdayTracker.Shared.Models.Response;

namespace BirthdayTracker.Server.Infrastructure
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
            CreateMap<AddEmployeeRequest, Employee>().ReverseMap();
        }

        private void MapResponse()
        {
            CreateMap<Employee, EmployeeResponse>().ReverseMap();
        }

        
    }
}
