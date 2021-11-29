using AutoMapper;
using WebApi.Entities;
using WebApi.Models;

namespace WebApi.Profiles
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeDto>().ForMember(dest => dest.FullName, act => act.MapFrom(src => src.Name));
            CreateMap<EmployeeAddDto, Employee>().ForMember(dest => dest.Name, act => act.MapFrom(src => src.FullName));
            CreateMap<EmployeeUpdateDto, Employee>().ForMember(dest => dest.Name, act => act.MapFrom(src => src.FullName));
            CreateMap<Employee, EmployeeUpdateDto>().ForMember(dest => dest.FullName, act => act.MapFrom(src => src.Name));
        }
    }
}
