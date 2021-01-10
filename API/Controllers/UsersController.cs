using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Extensions;
using API.Helpers;
using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/users")]
    public class UsersController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public UsersController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userManager = userManager;

        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ResponseDto<UsersDto>>> GetUsers()
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            if (!user.isAdmin) return StatusCode(403, new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(403)
            });

            var usersList = await _userManager.Users.ToListAsync();

            var userDtoList = _mapper.Map<IReadOnlyList<AppUser>, IReadOnlyList<UserDto>>(usersList);

            return new ResponseDto<UsersDto>
            {
                Success = true,
                Data = new UsersDto { Users = userDtoList },
                Error = new ApiErrorResponse()
            };
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<UserToReturnDto>>> GetUser([Required][FromRoute] string id)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            if (!user.isAdmin) return StatusCode(403, new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(403)
            });

            var userToReturn = await _userManager.FindByIdAsync(id);

            var userDto = _mapper.Map<AppUser, UserDto>(userToReturn);

            return new ResponseDto<UserToReturnDto>
            {
                Success = true,
                Data = new UserToReturnDto { User = userDto },
                Error = new ApiErrorResponse()
            };
        }

        [Authorize]
        [HttpGet("current")]
        public async Task<ActionResult<ResponseDto<UserResponseDto>>> GetCurrentUser()
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            return new ResponseDto<UserResponseDto>
            {
                Success = true,
                Data = new UserResponseDto
                {
                    User = _mapper.Map<AppUser, UserDto>(user),
                    Token = new TokenDto { Token = _tokenService.CreateToken(user) },
                },
                Error = new ApiErrorResponse()
            };
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDto<UserToReturnDto>>> UpdateUser([FromBody] CreateOrUpdateUserDto updateUserDto, [FromRoute] string id)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            if (!user.isAdmin) return StatusCode(403, new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(403)
            });

            var userToBeUpdated = await _userManager.FindByIdAsync(id);

            userToBeUpdated.UserName = updateUserDto.Name;

            var updateResult = await _userManager.UpdateAsync(userToBeUpdated);

            if (!updateResult.Succeeded) return BadRequest(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(400)
            });

            if (userToBeUpdated.Email != updateUserDto.Email)
            {
                var changeEmailToken = await _userManager.GenerateChangeEmailTokenAsync(userToBeUpdated, updateUserDto.Email);

                var result = await _userManager.ChangeEmailAsync(userToBeUpdated, updateUserDto.Email, changeEmailToken);

                if (!result.Succeeded) return BadRequest(new ResponseDto<string>
                {
                    Success = false,
                    Data = null,
                    Error = new ApiErrorResponse(400)
                });
            }

            return new ResponseDto<UserToReturnDto>
            {
                Success = true,
                Data = new UserToReturnDto { User = _mapper.Map<AppUser, UserDto>(userToBeUpdated) },
                Error = new ApiErrorResponse()
            };
        }

        [Authorize]
        [HttpPost()]
        public async Task<ActionResult<ResponseDto<UserToReturnDto>>> RegisterUser([FromBody] CreateOrUpdateUserDto createUserDto)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            if (!user.isAdmin) return StatusCode(403, new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(403)
            });

            var userToBeCreated = _mapper.Map<CreateOrUpdateUserDto, AppUser>(createUserDto);

            var result = await _userManager.CreateAsync(userToBeCreated, createUserDto.Password);

            if (!result.Succeeded) return BadRequest(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(400)
            });

            return new ResponseDto<UserToReturnDto>
            {
                Success = true,
                Data = new UserToReturnDto { User = _mapper.Map<AppUser, UserDto>(userToBeCreated) },
                Error = new ApiErrorResponse()
            };
        }

        [Authorize]
        [HttpPut("password")]
        public async Task<ActionResult<ResponseDto<UserToReturnDto>>> UpdatePassword([FromBody] UpdatePasswordDto updatePasswordDto)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            var result = await _userManager.ChangePasswordAsync(user, updatePasswordDto.CurrentPassword, updatePasswordDto.NewPassword);

            if (!result.Succeeded) return BadRequest(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(400)
            });

            return new ResponseDto<UserToReturnDto>
            {
                Success = true,
                Data = new UserToReturnDto { User = _mapper.Map<AppUser, UserDto>(user) },
                Error = new ApiErrorResponse()
            };
        }

        [Authorize]
        [HttpPost("{id}/password_reset")]
        public async Task<ActionResult<ResponseDto<PasswordDto>>> ResetPassword([FromRoute] string id)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            if (!user.isAdmin) return StatusCode(403, new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(403)
            });

            if (user.Id == id) return BadRequest(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(400)
            });

            var userToResetPassword = await _userManager.FindByIdAsync(id);

            var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(userToResetPassword);

            var randomPassword = RandomPasswordGenerator.GeneratePassword(5);

            var result = await _userManager.ResetPasswordAsync(userToResetPassword, resetPasswordToken, randomPassword);

            if (!result.Succeeded) return BadRequest(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(400)
            });

            return new ResponseDto<PasswordDto>
            {
                Success = true,
                Data = new PasswordDto { Password = randomPassword },
                Error = new ApiErrorResponse()
            };
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser([FromRoute] string id)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            if (!user.isAdmin) return StatusCode(403, new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(403)
            });

            if (user.Id == id) return BadRequest(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(400)
            });

            var userToBeDeleted = await _userManager.FindByIdAsync(id);

            var result = await _userManager.DeleteAsync(userToBeDeleted);

            if (!result.Succeeded) return BadRequest(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(400)
            });

            return StatusCode(204, new ResponseDto<string>
            {
                Success = true,
                Data = null,
                Error = new ApiErrorResponse()
            });
        }       
        
    }
}