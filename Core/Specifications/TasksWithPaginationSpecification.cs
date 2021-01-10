using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
    public class TasksWithPaginationSpecification : BaseSpecification<Task>
    {
        public TasksWithPaginationSpecification(TasksSpecificationParams tasksSpecificationParams)
        {
            Criteria = CreateCriteria(tasksSpecificationParams);

            AddOrderBy(task => task.Status);

            AddOrderBy(task => task.Deadline);

            ApplyPaging(tasksSpecificationParams.PageSize * (tasksSpecificationParams.PageIndex - 1), tasksSpecificationParams.PageSize);
        }

        private static Expression<Func<Task, bool>> CreateCriteria(TasksSpecificationParams tasksSpecificationParams)
        {

            if (tasksSpecificationParams.AppointmentId == null)
            {
                return null;
            }
            else
            {
                return task => task.AppointmentId == tasksSpecificationParams.AppointmentId;
            }
        }
    }
}