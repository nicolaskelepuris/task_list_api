using System;
using System.Linq;
using System.Linq.Expressions;
using Core.Entities;
using Core.Enums;

namespace Core.Specifications
{
    public class AppointmentsSpecification : BaseSpecification<Appointment>
    {
        public AppointmentsSpecification(AppointmentsSpecificationParams appointmentsSpecParams)
        {
            Criteria = CreateCriteria(appointmentsSpecParams);

            AddOrderBy(appointment => appointment.EstimatedTimeOfArrival);

            AddOrderBy(appointment => appointment.EstimatedTimeOfBerthing);

            AddOrderBy(appointment => appointment.Status);

            AddInclude(appointment => appointment.Vessel);

            AddInclude(appointment => appointment.Tasks);
        }

        private static Expression<Func<Appointment, bool>> CreateCriteria(AppointmentsSpecificationParams appointmentsSpecParams)
        {

            if (appointmentsSpecParams.VesselId == null && appointmentsSpecParams.Type == null && appointmentsSpecParams.Port == null
                && !appointmentsSpecParams.Done && !appointmentsSpecParams.Cancelled)
            {
                return null;
            }
            else
            {
                return appointment => (appointmentsSpecParams.Done ? appointment.Status == AppointmentStatus.Done : true) 
                    && (appointmentsSpecParams.Cancelled ? appointment.Status == AppointmentStatus.Cancelled : true)
                    && (appointmentsSpecParams.VesselId != null ? (appointment.VesselId == appointmentsSpecParams.VesselId) : true)
                    && (appointmentsSpecParams.Type != null ? (appointment.Type == appointmentsSpecParams.Type) : true)
                    && (appointmentsSpecParams.Port != null ? (appointment.Port == appointmentsSpecParams.Port) : true);
            }
        }
    }
}