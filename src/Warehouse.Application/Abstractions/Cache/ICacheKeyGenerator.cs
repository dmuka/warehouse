using System.Linq.Expressions;
using Warehouse.Core;

namespace Warehouse.Application.Abstractions.Cache;

public interface ICacheKeyGenerator
{
    string ForEntity<TEntity>(TypedId id) where TEntity : Entity;
    string ForQuery<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : Entity;
    string ForRawSql<TEntity>(string sql, List<object>? parameters) where TEntity : Entity;
    string ForMethod<TEntity>(string methodName, params (string ParamName, object ParamValue)[] parameters) 
        where TEntity : Entity;
}