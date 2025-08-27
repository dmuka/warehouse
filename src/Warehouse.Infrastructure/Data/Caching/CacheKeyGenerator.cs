using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Core;

namespace Warehouse.Infrastructure.Data.Caching;

public class CacheKeyGenerator : ICacheKeyGenerator
{
    public string ForEntity<TEntity>(TypedId id) where TEntity : Entity
        => $"{typeof(TEntity).Name.ToLowerInvariant()}:id:{id.Value}";

    public string ForQuery<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : Entity
    {
        var expressionString = condition.ToString();
        var hash = ComputeHash(expressionString);
        
        return $"{typeof(TEntity).Name.ToLowerInvariant()}:query:{hash}";
    }

    public string ForRawSql<TEntity>(string sql, List<object>? parameters) where TEntity : Entity
    {
        var sqlHash = ComputeHash(sql);
        
        var paramsHash = parameters != null 
            ? ComputeHash(string.Join("|", parameters.Select(p => p.ToString() ?? "null"))) 
            : "null";
        
        return $"{typeof(TEntity).Name.ToLowerInvariant()}:sql:{sqlHash}:params:{paramsHash}";
    }

    public string ForMethod<TEntity>(string methodName, params (string ParamName, object ParamValue)[] parameters) 
        where TEntity : Entity
    {
        var paramString = string.Join(":", parameters.Select(p => $"{p.ParamName}:{p.ParamValue}"));
        
        return $"{typeof(TEntity).Name.ToLowerInvariant()}:{methodName}:{paramString}";
    }

    private static string ComputeHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));

        return Convert.ToHexStringLower(bytes)[..16];
    }
}