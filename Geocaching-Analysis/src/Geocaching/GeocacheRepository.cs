using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Geocaching
{
    class GeocacheRepository : IUoWRepository<Geocache>
    {
        private SqlConnection _connection;
        private SqlTransaction _transaction;


        public GeocacheRepository(SqlConnection connection, SqlTransaction transaction)
        {
            this._connection = connection;
            this._transaction = transaction;
        }

        public void Add(Geocache geocache)
        {
            SqlCommand add = CreateCommand();

            add.CommandText = "INSERT INTO Geocaches(Name, CacheID, Country, Description, Difficulty, LongDescription, Owner, ShortDescription, Size, State, StatusArchived, StatusAvailable, Symbol, SymbolType, Terrain, Time, Type, URL, URLName, Latitude, Longitude, Altitude, LastChanged, GeocacheID)" +
                "VALUES(@Name, @CacheID, @Country, @Description, @Difficulty, @LongDescription, @Owner, @ShortDescription, @Size, @State, @StatusArchived, @StatusAvailable, @Symbol, @SymbolType, @Terrain, @Time, @Type, @URL, @URLName, @Latitude, @Longitude, @Altitude, @LastChanged, @GeocacheID)";

            add.Parameters.AddRange(parameterList(geocache));
            add.ExecuteNonQuery();
        }

        public IQueryable<Geocache> All()
        {
            throw new NotImplementedException();
        }

        public void Delete(Geocache geocache)
        {
            throw new NotImplementedException();
        }

        public void Update(Geocache geocache)
        {
            SqlCommand update = CreateCommand();
            update.CommandText = "UPDATE Geocaches SET " +
                "Name = @Name," +
                "CacheID = @CacheID, " +
                "Country = @Country," +
                "Description = @Description," +
                "Difficulty = @Difficulty," +
                "LongDescription = @LongDescription, " +
                "Owner = @Owner, " +
                "ShortDescription = @ShortDescription, " +
                "Size = @Size, " +
                "State = @State, " +
                "StatusArchived = @StatusArchived, " +
                "StatusAvailable = @StatusAvailable, " +
                "Symbol = @Symbol, " +
                "SymbolType = @SymbolType, " +
                "Terrain = @Terrain, " +
                "Time = @Time, " +
                "Type = @Type, " +
                "URL = @URL, " +
                "URLName = @URLName, " +
                "Latitude = @Latitude, " +
                "Longitude = @Longitude, " +
                "Altitude = @Altitude, " +
                "LastChanged = @LastChanged " +
                "WHERE GeocacheID = @GeocacheID";

            update.Parameters.AddRange(parameterList(geocache));
            update.ExecuteNonQuery();
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        private SqlCommand CreateCommand()
        {
            SqlCommand command = _connection.CreateCommand();
            command.Transaction = _transaction;
            return command;
        }

        void IUoWRepository<Geocache>.Commit()
        {
            throw new NotImplementedException();
        }

        IQueryable<Geocache> IRepository<Geocache>.All()
        {
            throw new NotImplementedException();
        }

        void IRepository<Geocache>.Add(Geocache entity)
        {
            throw new NotImplementedException();
        }

        void IRepository<Geocache>.Update(Geocache entity)
        {
            throw new NotImplementedException();
        }

        void IRepository<Geocache>.Delete(Geocache entity)
        {
            throw new NotImplementedException();
        }

        public SqlParameter[] parameterList(Geocache geocache)
        {
            List<SqlParameter> parameters = new List<SqlParameter>(24);

            parameters.Add(new SqlParameter("Name", geocache.Name));
            parameters.Add(new SqlParameter("CacheID", geocache.CacheID));
            parameters.Add(new SqlParameter("Country", geocache.Country));
            parameters.Add(new SqlParameter("Description", geocache.Description));
            parameters.Add(new SqlParameter("Difficulty", geocache.Difficulty));
            parameters.Add(new SqlParameter("LongDescription", geocache.LongDescription));
            parameters.Add(new SqlParameter("Owner", geocache.Owner));
            parameters.Add(new SqlParameter("ShortDescription", geocache.ShortDescription));
            parameters.Add(new SqlParameter("Size", geocache.Size));
            parameters.Add(new SqlParameter("State", geocache.State));
            parameters.Add(new SqlParameter("StatusArchived", geocache.StatusArchived));
            parameters.Add(new SqlParameter("StatusAvailable", geocache.StatusAvailable));
            parameters.Add(new SqlParameter("Symbol", geocache.Symbol));
            parameters.Add(new SqlParameter("SymbolType", geocache.SymbolType));
            parameters.Add(new SqlParameter("Terrain", geocache.Terrain));
            parameters.Add(new SqlParameter("Time", geocache.Time));
            parameters.Add(new SqlParameter("Type", geocache.Type));
            parameters.Add(new SqlParameter("URL", geocache.URL));
            parameters.Add(new SqlParameter("URLName", geocache.URLName));
            parameters.Add(new SqlParameter("Latitude", geocache.Latitude));
            parameters.Add(new SqlParameter("Longitude", geocache.Longitude));
            parameters.Add(new SqlParameter("Altitude", geocache.Altitude));
            parameters.Add(new SqlParameter("LastChanged", geocache.LastChanged));
            parameters.Add(new SqlParameter("GeocacheID", geocache.GeocacheID));

            return parameters.ToArray();
        }
    }
}