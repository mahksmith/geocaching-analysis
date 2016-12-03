﻿using System;
using System.Data.SqlClient;
using System.Linq;

namespace Geocaching
{
    internal class PocketQueryRepository : IUoWRepository<PocketQuery>
    {
        private SqlConnection _connection;
        private SqlTransaction _transaction;   
        
        public PocketQueryRepository(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }
        

        public void Add(PocketQuery pq)
        {
            SqlCommand command = CreateCommand();
            command.CommandText = "INSERT INTO PocketQueries(Name, DateGenerated, EntryCount, FileSize, Url)" +
                "VALUES (@Name, @DateGenerated, @EntryCount, @FileSize, @Url);";
            command.Parameters.AddWithValue("Name", pq.Name);
            command.Parameters.AddWithValue("DateGenerated", pq.DateGenerated);
            command.Parameters.AddWithValue("EntryCount", pq.EntryCount);
            command.Parameters.AddWithValue("FileSize", pq.FileSize);
            command.Parameters.AddWithValue("Url", pq.Url);
            command.ExecuteNonQuery();
        }

        public IQueryable<PocketQuery> All()
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public void Delete(PocketQuery pq)
        {
            throw new NotImplementedException();
        }

        public void Update(PocketQuery pq)
        {
            SqlCommand update = CreateCommand();
            update.CommandText = "UPDATE PocketQueries SET " +
                "DateGenerated = @DateGenerated," +
                "EntryCount = @EntryCount," + 
                "FileSize = @FileSize," + 
                "Url = @Url " +
                "WHERE Name = @Name";

            update.Parameters.AddWithValue("DateGenerated", pq.DateGenerated);
            update.Parameters.AddWithValue("EntryCount", pq.EntryCount);
            update.Parameters.AddWithValue("FileSize", pq.FileSize);
            update.Parameters.AddWithValue("Url", pq.Url);
            update.Parameters.AddWithValue("Name", pq.Name);
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