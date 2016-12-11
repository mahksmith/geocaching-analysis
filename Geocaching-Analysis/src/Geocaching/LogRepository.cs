using System;
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
            add.Parameters.AddWithValue("ID", log.ID);
            add.Parameters.AddWithValue("GeocacheID", log.GeocacheID);
            add.Parameters.AddWithValue("Date", log.Date);
            add.Parameters.AddWithValue("Type", log.Type);
            add.Parameters.AddWithValue("Author", log.Author);
            add.Parameters.AddWithValue("Text", log.Text);
            add.Parameters.AddWithValue("TextEncoded", log.TextEncoded);

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

            update.Parameters.AddWithValue("GeocacheID", log.GeocacheID);
            update.Parameters.AddWithValue("Date", log.Date);
            update.Parameters.AddWithValue("Type", log.Type);
            update.Parameters.AddWithValue("Author", log.Author);
            update.Parameters.AddWithValue("Text", log.Text);
            update.Parameters.AddWithValue("TextEncoded", log.TextEncoded);
            update.Parameters.AddWithValue("ID", log.ID);
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
