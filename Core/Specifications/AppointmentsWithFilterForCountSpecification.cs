using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
    public class AppointmentsWithFilterForCountSpecification : BaseSpecification<Appointment>
    {
        public AppointmentsWithFilterForCountSpecification(Expression<Func<Appointment, bool>> criteria) : base(criteria)
        {
        }
    }
}