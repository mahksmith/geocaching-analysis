using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Geocaching
{
    class LogRepository : IUoWRepository<Log>
    {
        private SqlConnection _connection;
        private SqlTransaction _transaction;

        public LogRepository(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public void Add(Log log)
        {
            SqlCommand add = CreateCommand();
            add.CommandText = "INSERT INTO Logs(ID, GeocacheID, Date, Type, Author, Text, TextEncoded)" +
                "VALUES(@ID, @GeocacheID, @Date, @Type, @Author, @Text, @TextEncoded);";

            add.Parameters.AddRange(parameterList(log));
            add.ExecuteNonQuery();
        }

        public IQueryable<Log> All()
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public void Delete(Log entity)
        {
            throw new NotImplementedException();
        }

        public SqlParameter[] parameterList(Log log)
        {
            List<SqlParameter> parameters = new List<SqlParameter>(7)
            {
                new SqlParameter("GeocacheID", log.GeocacheID),
                new SqlParameter("Date", log.Date),
                new SqlParameter("Type", log.Type),
                new SqlParameter("Author", log.Author),
                new SqlParameter("Text", log.Text),
                new SqlParameter("TextEncoded", log.TextEncoded),
                new SqlParameter("ID", log.ID)
            };
            return parameters.ToArray();
        }

        public void Update(Log log)
        {
            SqlCommand update = CreateCommand();
            update.CommandText = "UPDATE Logs SET " +
                "GeocacheID = @GeocacheID, " +
                "Date = @Date, " +
                "Type = @Type, " +
                "Author = @Author, " +
                "Text = @Text, " +
                "TextEncoded = @TextEncoded " +
                "WHERE ID = @ID";

            update.Parameters.AddRange(parameterList(log));
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
