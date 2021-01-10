using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
    public class TasksWithFilterForCountSpecification : BaseSpecification<Task>
    {
        public TasksWithFilterForCountSpecification(Expression<Func<Task, bool>> criteria) : base(criteria)
        {
        }
    }
}