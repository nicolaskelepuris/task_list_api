using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Entities.Identity;

namespace API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<RegisterOrUpdateVesselDto, Vessel>();
            CreateMap<Vessel, VesselDto>();
            CreateMap<Task, TaskDto>();
            CreateMap<CreateOrUpdateTaskDto, Task>();
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(d => d.VesselName, o => o.MapFrom(s => s.Vessel.Name));
            CreateMap<CreateOrUpdateAppointmentDto, Appointment>();
            CreateMap<AppUser, UserDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.UserName));
            CreateMap<CreateOrUpdateUserDto, AppUser>()
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.Name));
        }
    }
}