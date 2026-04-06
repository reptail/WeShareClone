using System.Data;
using Dapper;

namespace WeShareClone.DataAccess;

/// <summary>
/// Dapper type handler that maps <see cref="DateOnly"/> to and from a SQL <c>DATE</c> column.
/// Register once at startup via <see cref="SqlMapper.AddTypeHandler{T}"/>.
/// </summary>
public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = DbType.Date;
        parameter.Value  = value.ToDateTime(TimeOnly.MinValue);
    }

    public override DateOnly Parse(object value)
    {
        return DateOnly.FromDateTime(Convert.ToDateTime(value));
    }
}
