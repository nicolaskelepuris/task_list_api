using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
    public class VesselsWithFilterForCountSpecification : BaseSpecification<Vessel>
    {
        public VesselsWithFilterForCountSpecification(Expression<Func<Vessel, bool>> criteria) : base(criteria)
        {
        }
    }
}