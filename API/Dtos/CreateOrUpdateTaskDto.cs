using System;
using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace API.Dtos
{
    public class CreateOrUpdateTaskDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime Deadline { get; set; }
        [Required]
        public TaskStatus Status { get; set; }
        [Required]
        public WhenToComplete WhenToComplete { get; set; }
        [Required]
        public int AppointmentId { get; set; }
    }
}