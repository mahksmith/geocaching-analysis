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

            while (ok != true && currentRetry <= maxRetries)
            {
                try
                {
                    sqlCommand.ExecuteNonQuery();
                    ok = true;
                }
                catch (SqlException e)
                {
                    switch (e.Number)
                    {
                        case 1205:
                            // Deadlock
                            System.Threading.Thread.Sleep(new Random().Next(60 * 1000));
                            repositoryAddMethod();
                            ok = true;
                            break;

                        case 2601:
                        case 2627:
                            // Duplicate Keys
                            repositoryUpdateMethod();
                            ok = true;
                            break;

                        case -2:
                            // Query Timeout
                            var sleep = new Random().Next(60 * 1000);
                            Console.WriteLine("Sleeping Query %1", sleep);
                            System.Threading.Thread.Sleep(new Random().Next(60 * 1000));
                            currentRetry++;
                            break;
                    }

                    if (currentRetry == maxRetries)
                    {
                        throw;
                    }
                }
            }

            return;
        }
    }
}
