using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Geocaching
{
    class LogRepository : GenericRepository<Log>, IUoWRepository<Log>
    {
        private SqlConnection _connection;
        private SqlTransaction _transaction;

        public LogRepository(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public override Log Add(Log log)
        {
            return Add(log, 0);
        }

        public Log Add(Log log, int attempt = 0, SqlCommand add = null)
        {
            if (add == null)
            {
                add = CreateCommand();
                add.CommandText = "INSERT INTO Logs(ID, GeocacheID, Date, Type, Author, Text, TextEncoded)" +
                    "VALUES(@ID, @GeocacheID, @Date, @Type, @Author, @Text, @TextEncoded);";
                add.Parameters.AddWithValue("@ID", log.ID);
                add.Parameters.AddWithValue("@GeocacheID", log.GeocacheID);
                add.Parameters.AddWithValue("@Date", log.Date);
                add.Parameters.AddWithValue("@Type", log.Type);
                add.Parameters.AddWithValue("@Author", log.Author);
                add.Parameters.AddWithValue("@Text", log.Text);
                add.Parameters.AddWithValue("@TextEncoded", log.TextEncoded);
            }

            //TODO: we should have already checked for existance in database previously, however, we still might need to run update if a deadlock occurs on Add.
            base.Retry(() => Add(log, ++attempt, add), () => Update(log), ++attempt, 3, add);

            return log;
        }

        public override IQueryable<Log> All()
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public override void Delete(Log entity)
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

        public override void Update(Log log)
        {
            Update(log, 0);
        }

        public Log Update(Log log, int attempt = 0, SqlCommand update = null)
        {
            if (update == null)
            {
                update = CreateCommand();
                update.CommandText = 
                    "UPDATE Logs SET " +
                    "GeocacheID = @GeocacheID, " +
                    "Date = @Date, " +
                    "Type = @Type, " +
                    "Author = @Author, " +
                    "Text = @Text, " +
                    "TextEncoded = @TextEncoded " +
                    "WHERE ID = @ID";

                update.Parameters.AddWithValue("@GeocacheID", log.GeocacheID);
                update.Parameters.AddWithValue("@Date", log.Date);
                update.Parameters.AddWithValue("@Type", log.Type);
                update.Parameters.AddWithValue("@Author", log.Author);
                update.Parameters.AddWithValue("@Text", log.Text);
                update.Parameters.AddWithValue("@TextEncoded", log.TextEncoded);
                update.Parameters.AddWithValue("@ID", log.ID);
                update.CommandTimeout = 0;
            }
            base.Retry(() => Update(log, ++attempt, update), () => Update(log), ++attempt, 3, update);

            return log;
        }

        private SqlCommand CreateCommand()
        {
            SqlCommand command = _connection.CreateCommand();
            command.Transaction = _transaction;
            return command;
        }
    }
}
