using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Warehouse.Core;

public abstract class Dto : Entity
{
    [Key] 
    public new Guid Id { get; set; }
    public abstract Entity ToEntity();
    public abstract Dto ToDto(Entity entity);

    public class EntityToDtoMapper<TEntity, TEntityDto>
    {
        public static Expression<Func<TEntityDto, bool>> ConvertCondition(Expression<Func<TEntity, bool>> condition)
        {
            // Define a parameter for the DTO
            var dtoParameter = Expression.Parameter(typeof(TEntityDto), "dto");

            // Map the properties from TEntity to TEntityDto
            // This example assumes a simple mapping where property names are the same
            var body = new ReplaceParameterVisitor(condition.Parameters[0], dtoParameter).Visit(condition.Body);

            // Create a new lambda expression for the DTO
            return Expression.Lambda<Func<TEntityDto, bool>>(body, dtoParameter);
        }
    }

    public class ReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _newParameter : base.VisitParameter(node);
        }
    }
}