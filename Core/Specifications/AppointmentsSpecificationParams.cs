using Core.Enums;

namespace Core.Specifications
{
    public class AppointmentsSpecificationParams
    {
        public int? VesselId { get; set; }
        public AppointmentType? Type { get; set; }
        public Port? Port { get; set; }        
        public bool Done { get; set; } = false;
        public bool Cancelled { get; set; } = false;
    }
}