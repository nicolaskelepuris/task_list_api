using System;
using Core.Enums;

namespace Core.Entities
{
    public class Task : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public TaskStatus Status { get; set; }
        public WhenToComplete WhenToComplete { get; set; }
        public int AppointmentId { get; set; }
    }
}