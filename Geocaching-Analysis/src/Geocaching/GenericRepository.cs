using System;
using System.Data.SqlClient;
using System.Linq;

namespace Geocaching
{
    abstract class GenericRepository<T> : IRepository<T>
    {
        public abstract T Add(T entity);

        public abstract IQueryable<T> All();

        public abstract void Delete(T entity);

        public abstract void Update(T entity);

        public void Retry(Func<T> repositoryAddMethod, Func<T> repositoryUpdateMethod, int currentRetry, int maxRetries, SqlCommand sqlCommand)
        {
            //var retryCount = 0;
            var ok = false;

            while (ok != true && currentRetry < maxRetries)
            {
                try
                {
                    sqlCommand.ExecuteNonQuery();
                    ok = true;
                }
                catch (SqlException e)
                {
                    if (e.Number == 1205)
                    { //Deadlock
                        System.Threading.Thread.Sleep(new Random().Next(1000));
                        repositoryAddMethod();
                        ok = true;
                    }
                    else if (e.Number == 2627 || e.Number == 2601)
                    {
                        repositoryUpdateMethod();
                    }
                    else
                        throw;
                }
            }

            return;
        }
    }
}
