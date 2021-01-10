using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
    public class VesselsSpecification : BaseSpecification<Vessel>
    {
        public VesselsSpecification(VesselsSpecificationParams vesselsSpecificationParams)
        {
            Criteria = CreateCriteria(vesselsSpecificationParams);
        }

        private static Expression<Func<Vessel, bool>> CreateCriteria(VesselsSpecificationParams vesselsSpecificationParams)
        {

            if (string.IsNullOrEmpty(vesselsSpecificationParams.NameSearch))
            {
                return null;
            }
            else
            {
                return vessel => vessel.Name.ToLower().Contains(vesselsSpecificationParams.NameSearch);
            }
        }
    }
}