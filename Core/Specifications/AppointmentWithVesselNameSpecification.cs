using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
    public class AppointmentWithVesselNameSpecification : BaseSpecification<Appointment>
    {
        public AppointmentWithVesselNameSpecification(int id)
        {
            Criteria = appointment => appointment.Id == id;

            AddInclude(appointment => appointment.Vessel);
            AddInclude(appointment => appointment.Tasks);
        }
    }
}