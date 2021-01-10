using System;
using System.Collections.Generic;
using Core.Enums;

namespace Core.Entities
{
    public class Appointment : BaseEntity
    {
        public AppointmentType Type { get; set; }
        public int VesselId { get; set; }
        public Vessel Vessel { get; set; }
        public string DuvNumber { get; set; }
        public string ScheduleNumber { get; set; }
        public string VoyageNumber { get; set; }
        public string NextPorts { get; set; }
        public OperationType OperationType { get; set; }
        public string Cargo { get; set; }
        public Port Port { get; set; }
        public bool HasCrewChange { get; set; }
        public int OnSigners { get; set; }
        public int OffSigners { get; set; }
        public List<Task> Tasks { get; set; }
        public DateTime EstimatedTimeOfArrivalOnFirstBrazillianPort { get; set; }
        public DateTime EstimatedTimeOfArrival { get; set; }
        public DateTime EstimatedTimeOfBerthing { get; set; }        
        public DateTime EstimatedTimeOfSailing { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime Berthing { get; set; }
        public DateTime Sailing { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}