using AutoMapper;
using BirthdayTracker.Shared;
using BirthdayTracker.Shared.Models.Request;
using BirthdayTracker.Shared.Models.Response;

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
            CreateMap<EmployeeRequest, Employee>();
        }

        private void MapResponse()
        {
            CreateMap<Employee, EmployeeResponse>();
        }


    }
}
