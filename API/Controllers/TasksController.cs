using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Extensions;
using API.Helpers;
using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/tasks")]
    public class TasksController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        public TasksController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IMapper mapper)
        {
            _mapper = mapper;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ResponseDto<Pagination<TaskDto>>>> GetTasks([FromQuery]TasksSpecificationParams tasksSpecificationParams)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            var spec = new TasksWithPaginationSpecification(tasksSpecificationParams);

            var countSpec = new TasksWithFilterForCountSpecification(spec.Criteria);

            var totalItems = await _unitOfWork.Repository<Core.Entities.Task>().CountAsync(countSpec);

            var tasks = await _unitOfWork.Repository<Core.Entities.Task>().ListAsyncWithSpec(spec);

            if (tasks.Count == 0) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            var tasksDto = _mapper.Map<IReadOnlyList<Core.Entities.Task>, IReadOnlyList<TaskDto>>(tasks);

            return new ResponseDto<Pagination<TaskDto>>
            {
                Success = true,
                Data = new Pagination<TaskDto>
                (tasksSpecificationParams.PageIndex, tasksSpecificationParams.PageSize, totalItems, tasksDto),
                Error = new ApiErrorResponse()
            };
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<TaskToReturnDto>>> GetTask([FromRoute] int id)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            var task = await _unitOfWork.Repository<Core.Entities.Task>().GetEntityByIdAsync(id);

            if (task == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            var taskDto = _mapper.Map<Core.Entities.Task, TaskDto>(task);

            return new ResponseDto<TaskToReturnDto>
            {
                Success = true,
                Data = new TaskToReturnDto() {Task = taskDto},
                Error = new ApiErrorResponse()
            };
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ResponseDto<TaskToReturnDto>>> CreateTask([FromBody] CreateOrUpdateTaskDto createTaskDto)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            var task = _mapper.Map<CreateOrUpdateTaskDto, Core.Entities.Task>(createTaskDto);

            if (!ValidateTask(task)) return BadRequest(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(400)
            });

            _unitOfWork.Repository<Core.Entities.Task>().Add(task);

            var result = await _unitOfWork.Complete();

            if (result <= 0) return BadRequest(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(400)
            });

            var taskDto = _mapper.Map<Core.Entities.Task, TaskDto>(task);

            return new ResponseDto<TaskToReturnDto>
            {
                Success = true,
                Data = new TaskToReturnDto() {Task = taskDto},
                Error = new ApiErrorResponse()
            };
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDto<TaskToReturnDto>>> UpdateTask([FromBody] CreateOrUpdateTaskDto updateTaskDto, [FromRoute] int id)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            var task = await _unitOfWork.Repository<Core.Entities.Task>().GetEntityByIdAsync(id);

            if (task == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            task.Name = updateTaskDto.Name;
            task.Description = updateTaskDto.Description;
            task.Deadline = updateTaskDto.Deadline;
            task.Status = updateTaskDto.Status;
            task.WhenToComplete = updateTaskDto.WhenToComplete;
            task.AppointmentId = updateTaskDto.AppointmentId;

            if (!ValidateTask(task)) return BadRequest(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(400)
            });

            _unitOfWork.Repository<Core.Entities.Task>().Update(task);

            var result = await _unitOfWork.Complete();

            if (result <= 0) return BadRequest(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(400)
            });

            var taskDto = _mapper.Map<Core.Entities.Task, TaskDto>(task);

            return new ResponseDto<TaskToReturnDto>
            {
                Success = true,
                Data = new TaskToReturnDto() {Task = taskDto},
                Error = new ApiErrorResponse()
            };
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDto<TaskDto>>> DeleteTask([FromRoute] int id)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(HttpContext.User);

            if (user == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            var task = await _unitOfWork.Repository<Core.Entities.Task>().GetEntityByIdAsync(id);

            if (task == null) return NotFound(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(404)
            });

            _unitOfWork.Repository<Core.Entities.Task>().Delete(task);

            var result = await _unitOfWork.Complete();

            if (result <= 0) return BadRequest(new ResponseDto<string>
            {
                Success = false,
                Data = null,
                Error = new ApiErrorResponse(400)
            });

            return StatusCode(204);
        }

        private bool ValidateTask(Core.Entities.Task task)
        {
            return !Enum.IsDefined(typeof(Core.Enums.TaskStatus), task.Status) || !Enum.IsDefined(typeof(Core.Enums.WhenToComplete), task.WhenToComplete) ?
                false : true;
        }

    }
}