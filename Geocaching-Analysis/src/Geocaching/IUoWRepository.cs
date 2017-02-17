using System.Data.SqlClient;

namespace Geocaching
{
    interface IUoWRepository<T>: IRepository<T>
    {
        void Commit();
        SqlParameter[] parameterList(T t);
    }
}
