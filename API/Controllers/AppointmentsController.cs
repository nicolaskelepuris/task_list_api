using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Extensions;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Entities.Identity;
using Core.Enums;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/appointments")]
    public class AppointmentsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        public AppointmentsController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IMapper mapper)
        {
            _mapper = mapper;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ResponseDto<AppointmentsDto>>> GetAppointments([FromQuery] AppointmentsSpecificationParams specParams)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            var spec = new AppointmentsSpecification(specParams);

            var appointments = await _unitOfWork.Repository<Appointment>().ListAsyncWithSpec(spec);

            foreach (var item in appointments)
            {
                item.Tasks = item.Tasks.OrderBy(task => task.Status).ToList();
            }

            if (appointments.Count == 0) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            var appointmentsDto = _mapper.Map<IReadOnlyList<Appointment>, IReadOnlyList<AppointmentDto>>(appointments);

            return new ResponseDto<AppointmentsDto>
            {
                Success = true,
                Data = new AppointmentsDto() { Appointments = appointmentsDto },
                Error = new ApiErrorResponse()
            };
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<AppointmentToReturnDto>>> GetAppointment([FromRoute] int id)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            var spec = new AppointmentWithVesselNameSpecification(id);

            var appointment = await _unitOfWork.Repository<Appointment>().GetEntityAsyncWithSpec(spec);

            appointment.Tasks = appointment.Tasks.OrderBy(task => task.Status).ToList();

            if (appointment == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            var appointmentDto = _mapper.Map<Appointment, AppointmentDto>(appointment);

            return new ResponseDto<AppointmentToReturnDto>
            {
                Success = true,
                Data = new AppointmentToReturnDto() { Appointment = appointmentDto },
                Error = new ApiErrorResponse()
            };
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ResponseDto<AppointmentToReturnDto>>> CreateAppointment([FromBody] CreateOrUpdateAppointmentDto createAppointmentDto)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            var appointment = _mapper.Map<CreateOrUpdateAppointmentDto, Appointment>(createAppointmentDto);

            if (!ValidateAppointment(appointment)) return BadRequest(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(400)
            });

            _unitOfWork.Repository<Appointment>().Add(appointment);

            var result = await _unitOfWork.Complete();

            if (result <= 0) return BadRequest(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(400)
            });

            var appointmentDto = _mapper.Map<Appointment, AppointmentDto>(appointment);

            return new ResponseDto<AppointmentToReturnDto>
            {
                Success = true,
                Data = new AppointmentToReturnDto() { Appointment = appointmentDto },
                Error = new ApiErrorResponse()
            };
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDto<AppointmentToReturnDto>>> UpdateAppointment([FromBody] CreateOrUpdateAppointmentDto updateAppointmentDto, [FromRoute] int id)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            var appointment = await _unitOfWork.Repository<Appointment>().GetEntityByIdAsync(id);

            if (appointment == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            _mapper.Map<CreateOrUpdateAppointmentDto, Appointment>(updateAppointmentDto, appointment);

            if (!ValidateAppointment(appointment)) return BadRequest(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(400)
            });

            _unitOfWork.Repository<Appointment>().Update(appointment);

            var result = await _unitOfWork.Complete();

            if (result <= 0) return BadRequest(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(400)
            });

            var spec = new AppointmentWithVesselNameSpecification(id);

            appointment = await _unitOfWork.Repository<Appointment>().GetEntityAsyncWithSpec(spec);            

            appointment.Tasks = appointment.Tasks.OrderBy(task => task.Status).ToList();

            var appointmentDto = _mapper.Map<Appointment, AppointmentDto>(appointment);

            return new ResponseDto<AppointmentToReturnDto>
            {
                Success = true,
                Data = new AppointmentToReturnDto() { Appointment = appointmentDto },
                Error = new ApiErrorResponse()
            };
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDto<AppointmentToReturnDto>>> DeleteAppointment([FromRoute] int id)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            var appointment = await _unitOfWork.Repository<Appointment>().GetEntityByIdAsync(id);

            if (appointment == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            _unitOfWork.Repository<Appointment>().Delete(appointment);

            var result = await _unitOfWork.Complete();

            if (result <= 0) return BadRequest(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(400)
            });

            return StatusCode(204);
        }

        private bool ValidateAppointment(Appointment appointment)
        {
            return !Enum.IsDefined(typeof(AppointmentType), appointment.Type) || !Enum.IsDefined(typeof(OperationType), appointment.OperationType)
                || !Enum.IsDefined(typeof(Port), appointment.Port) ?
                false : true;
        }


    }
}