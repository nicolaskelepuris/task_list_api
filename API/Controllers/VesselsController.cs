using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Extensions;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Entities.Identity;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/vessels")]
    public class VesselsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        public VesselsController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IMapper mapper)
        {
            _mapper = mapper;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ResponseDto<VesselsDto>>> GetVessels([FromQuery]VesselsSpecificationParams vesselsSpecificationParams)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            var spec = new VesselsSpecification(vesselsSpecificationParams);

            var vessels = await _unitOfWork.Repository<Vessel>().ListAsyncWithSpec(spec);

            if (vessels.Count == 0) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            var vesselsDto = _mapper.Map<IReadOnlyList<Vessel>, IReadOnlyList<VesselDto>>(vessels);

            return new ResponseDto<VesselsDto>
            {
                Success = true,
                Data = new VesselsDto {Vessels = vesselsDto},
                Error = new ApiErrorResponse()
            };
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<VesselToReturnDto>>> GetVessel([FromRoute] int id)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            var vessel = await _unitOfWork.Repository<Vessel>().GetEntityByIdAsync(id);

            if (vessel == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            var vesselDto = _mapper.Map<Vessel, VesselDto>(vessel);

            return new ResponseDto<VesselToReturnDto>
            {
                Success = true,
                Data = new VesselToReturnDto {Vessel = vesselDto},
                Error = new ApiErrorResponse()
            };
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ResponseDto<VesselToReturnDto>>> RegisterVessel([FromBody] RegisterOrUpdateVesselDto registerVesselDto)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            var vessel = _mapper.Map<RegisterOrUpdateVesselDto, Vessel>(registerVesselDto);

            _unitOfWork.Repository<Vessel>().Add(vessel);

            var result = await _unitOfWork.Complete();

            if (result <= 0) return BadRequest(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(400)
            });

            var vesselDto = _mapper.Map<Vessel, VesselDto>(vessel);

            return new ResponseDto<VesselToReturnDto>
            {
                Success = true,
                Data = new VesselToReturnDto {Vessel = vesselDto},
                Error = new ApiErrorResponse()
            };
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDto<VesselToReturnDto>>> UpdateVessel([FromBody] RegisterOrUpdateVesselDto updateVesselDto, [FromRoute] int id)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            var vessel = await _unitOfWork.Repository<Vessel>().GetEntityByIdAsync(id);

            if (vessel == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            vessel.Name = updateVesselDto.Name;
            vessel.Imo = updateVesselDto.Imo;
            vessel.Flag = updateVesselDto.Flag;
            vessel.Deadweight = updateVesselDto.Deadweight;
            vessel.LengthOverall = updateVesselDto.LengthOverall;
            vessel.Beam = updateVesselDto.Beam;
            vessel.Depth = updateVesselDto.Depth;

            _unitOfWork.Repository<Vessel>().Update(vessel);

            var result = await _unitOfWork.Complete();

            if (result <= 0) return BadRequest(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(400)
            });

            var vesselDto = _mapper.Map<Vessel, VesselDto>(vessel);

            return new ResponseDto<VesselToReturnDto>
            {
                Success = true,
                Data = new VesselToReturnDto {Vessel = vesselDto},
                Error = new ApiErrorResponse()
            };
        }
    }
}