using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Geocaching
{
    internal class PocketQueryRepository : GenericRepository<PocketQuery>, IUoWRepository<PocketQuery>
    {
        private SqlConnection _connection;
        private SqlTransaction _transaction;   
        
        public PocketQueryRepository(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }
        

        public override PocketQuery Add(PocketQuery pq)
        {
            SqlCommand command = CreateCommand();
            command.CommandText = "INSERT INTO PocketQueries(Name, DateGenerated, EntryCount, FileSize, Url)" +
                "VALUES (@Name, @DateGenerated, @EntryCount, @FileSize, @Url);";

            command.Parameters.AddRange(parameterList(pq));
            command.ExecuteNonQuery();

            return pq;
        }

        public override IQueryable<PocketQuery> All()
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public override void Delete(PocketQuery pq)
        {
            throw new NotImplementedException();
        }

        public SqlParameter[] parameterList(PocketQuery pq)
        {
            List<SqlParameter> parameters = new List<SqlParameter>(5)
            {
                new SqlParameter("Name", pq.Name),
                new SqlParameter("DateGenerated", pq.DateGenerated),
                new SqlParameter("EntryCount", pq.EntryCount),
                new SqlParameter("FileSize", pq.FileSize),
                new SqlParameter("Url", pq.Url)
            };
            return parameters.ToArray();
        }

        public override void Update(PocketQuery pq)
        {
            SqlCommand update = CreateCommand();
            update.CommandText = "UPDATE PocketQueries SET " +
                "DateGenerated = @DateGenerated," +
                "EntryCount = @EntryCount," + 
                "FileSize = @FileSize," + 
                "Url = @Url " +
                "WHERE Name = @Name";

            update.Parameters.AddRange(parameterList(pq));
            update.ExecuteNonQuery();
        }

        private SqlCommand CreateCommand()
        {
            SqlCommand command = _connection.CreateCommand();
            command.Transaction = _transaction;
            return command;
        }
    }
}