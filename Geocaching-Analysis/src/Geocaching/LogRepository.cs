using System;
using System.Data.SqlClient;
using System.Diagnostics;
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
            Add(log, 0);
        }

        public void Add(Log log, int attempt = 0, SqlCommand add = null)
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
            bool ok = false;
            while (!ok)
            {
                try
                {
                    add.ExecuteNonQuery();
                    ok = true;
                }
                catch (SqlException e)
                {
                    if (e.Number == 1205) //Deadlock
                    {
                        Console.WriteLine($"Attempting to Add {log.GeocacheID}, attempt {attempt++}");
                        //System.Threading.Thread.Sleep(new Random().Next(5000));
                        Add(log, attempt, add);
                    }

                    //TODO document what these numbers mean..
                    else if (e.Number == 2627 || e.Number == 2601)
                    {
                        Console.WriteLine($"Log: Duplicate Key: {log.ID}");
                        Update(log);
                        ok = true;          // Required, will loop infinitely..
                    }
                    else
                        throw;
                }
            }
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
            Update(log, 0);
        }

        public void Update(Log log, int attempt = 0, SqlCommand update = null)
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

            bool ok = false;
            while (!ok)
            {
                try
                {
                    update.ExecuteNonQuery();
                    ok = true;
                }
                catch (SqlException e)
                {
                    if (e.Number == 1205) //Deadlock
                    {
                        Console.WriteLine($"Attempting to Update {log.GeocacheID}, attempt {attempt++}");
                        //System.Threading.Thread.Sleep(new Random().Next(5000));
                        Update(log, attempt, update);
                    }
                    else
                        throw;
                }
            }
        }

        private SqlCommand CreateCommand()
        {
            SqlCommand command = _connection.CreateCommand();
            command.Transaction = _transaction;
            return command;
        }

        private void handleDeadLockException()
        {

        }
    }
}
