using System;
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

            add.Parameters.AddWithValue("Name", geocache.Name);
            add.Parameters.AddWithValue("CacheID", geocache.CacheID);
            add.Parameters.AddWithValue("Country", geocache.Country);
            add.Parameters.AddWithValue("Description", geocache.Description);
            add.Parameters.AddWithValue("Difficulty", geocache.Difficulty);
            add.Parameters.AddWithValue("LongDescription", geocache.LongDescription);
            add.Parameters.AddWithValue("Owner", geocache.Owner);
            add.Parameters.AddWithValue("ShortDescription", geocache.ShortDescription);
            add.Parameters.AddWithValue("Size", geocache.Size);
            add.Parameters.AddWithValue("State", geocache.State);
            add.Parameters.AddWithValue("StatusArchived", geocache.StatusArchived);
            add.Parameters.AddWithValue("StatusAvailable", geocache.StatusAvailable);
            add.Parameters.AddWithValue("Symbol", geocache.Symbol); 
            add.Parameters.AddWithValue("SymbolType", geocache.SymbolType);
            add.Parameters.AddWithValue("Terrain", geocache.Terrain);
            add.Parameters.AddWithValue("Time", geocache.Time);
            add.Parameters.AddWithValue("Type", geocache.Type);
            add.Parameters.AddWithValue("URL", geocache.URL);
            add.Parameters.AddWithValue("URLName", geocache.URLName);
            add.Parameters.AddWithValue("Latitude", geocache.Latitude);
            add.Parameters.AddWithValue("Longitude", geocache.Longitude);
            add.Parameters.AddWithValue("Altitude", geocache.Altitude);
            add.Parameters.AddWithValue("LastChanged", geocache.LastChanged);
            add.Parameters.AddWithValue("GeocacheID", geocache.GeocacheID);

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
            update.Parameters.AddWithValue("Name", geocache.Name);
            update.Parameters.AddWithValue("CacheID", geocache.CacheID);
            update.Parameters.AddWithValue("Country", geocache.Country);
            update.Parameters.AddWithValue("Description", geocache.Description);
            update.Parameters.AddWithValue("Difficulty", geocache.Difficulty);
            update.Parameters.AddWithValue("LongDescription", geocache.LongDescription);
            update.Parameters.AddWithValue("Owner", geocache.Owner);
            update.Parameters.AddWithValue("ShortDescription", geocache.ShortDescription);
            update.Parameters.AddWithValue("Size", geocache.Size);
            update.Parameters.AddWithValue("State", geocache.State);
            update.Parameters.AddWithValue("StatusArchived", geocache.StatusArchived);
            update.Parameters.AddWithValue("StatusAvailable", geocache.StatusAvailable);
            update.Parameters.AddWithValue("Symbol", geocache.Symbol);
            update.Parameters.AddWithValue("SymbolType", geocache.SymbolType);
            update.Parameters.AddWithValue("Terrain", geocache.Terrain);
            update.Parameters.AddWithValue("Time", geocache.Time);
            update.Parameters.AddWithValue("Type", geocache.Type);
            update.Parameters.AddWithValue("URL", geocache.URL);
            update.Parameters.AddWithValue("URLName", geocache.URLName);
            update.Parameters.AddWithValue("Latitude", geocache.Latitude);
            update.Parameters.AddWithValue("Longitude", geocache.Longitude);
            update.Parameters.AddWithValue("Altitude", geocache.Altitude);
            update.Parameters.AddWithValue("LastChanged", geocache.LastChanged);
            update.Parameters.AddWithValue("GeocacheID", geocache.GeocacheID);
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
    }
}
