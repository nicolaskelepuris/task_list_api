using System.Collections.Generic;

namespace API.Dtos
{
    public class AppointmentsDto
    {
        public IReadOnlyList<AppointmentDto> Appointments { get; set; }
    }
}