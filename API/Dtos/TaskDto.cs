using System;
using Core.Entities;
using Core.Enums;

namespace API.Dtos
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public TaskStatus Status { get; set; }
        public WhenToComplete WhenToComplete { get; set; }
        public int AppointmentId { get; set; }
    }
}