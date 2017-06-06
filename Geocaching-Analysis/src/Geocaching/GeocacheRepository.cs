using System;
using System.Data.SqlClient;
using System.Diagnostics;
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
            Add(geocache, 0);
        }

        private void Add(Geocache geocache, int attempt = 0, SqlCommand add = null)
        {
            if (add == null) {
                add = CreateCommand();

                add.CommandText = "INSERT INTO Geocaches(Name, CacheID, Country, Description, Difficulty, LongDescription, Owner, ShortDescription, Size, State, StatusArchived, StatusAvailable, Symbol, SymbolType, Terrain, Time, Type, URL, URLName, Latitude, Longitude, Altitude, LastChanged, GeocacheID)" +
                    "VALUES(@Name, @CacheID, @Country, @Description, @Difficulty, @LongDescription, @Owner, @ShortDescription, @Size, @State, @StatusArchived, @StatusAvailable, @Symbol, @SymbolType, @Terrain, @Time, @Type, @URL, @URLName, @Latitude, @Longitude, @Altitude, @LastChanged, @GeocacheID)";

                add.Parameters.AddWithValue("@Name", geocache.Name);
                add.Parameters.AddWithValue("@CacheID", geocache.CacheID);
                add.Parameters.AddWithValue("@Country", geocache.Country);
                add.Parameters.AddWithValue("@Description", geocache.Description);
                add.Parameters.AddWithValue("@Difficulty", geocache.Difficulty);
                add.Parameters.AddWithValue("@LongDescription", geocache.LongDescription);
                add.Parameters.AddWithValue("@Owner", geocache.Owner);
                add.Parameters.AddWithValue("@ShortDescription", geocache.ShortDescription);
                add.Parameters.AddWithValue("@Size", geocache.Size);
                add.Parameters.AddWithValue("@State", geocache.State);
                add.Parameters.AddWithValue("@StatusArchived", geocache.StatusArchived);
                add.Parameters.AddWithValue("@StatusAvailable", geocache.StatusAvailable);
                add.Parameters.AddWithValue("@Symbol", geocache.Symbol);
                add.Parameters.AddWithValue("@SymbolType", geocache.SymbolType);
                add.Parameters.AddWithValue("@Terrain", geocache.Terrain);
                add.Parameters.AddWithValue("@Time", geocache.Time);
                add.Parameters.AddWithValue("@Type", geocache.Type);
                add.Parameters.AddWithValue("@URL", geocache.URL);
                add.Parameters.AddWithValue("@URLName", geocache.URLName);
                add.Parameters.AddWithValue("@Latitude", geocache.Latitude);
                add.Parameters.AddWithValue("@Longitude", geocache.Longitude);
                add.Parameters.AddWithValue("@Altitude", geocache.Altitude);
                add.Parameters.AddWithValue("@LastChanged", geocache.LastChanged);
                add.Parameters.AddWithValue("@GeocacheID", geocache.GeocacheID);
            }
            
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
                        Console.WriteLine($"Attempting to Add {geocache.CacheID}, attempt {attempt++}");
                        //System.Threading.Thread.Sleep(new Random().Next(5000));
                        Add(geocache, attempt, add);
                    }

                    //TODO document what these numbers refer to...
                    else if (e.Number == 2627 || e.Number == 2601)
                    {
                        Console.WriteLine($"Geocache: Duplicate Key {geocache.CacheID}");
                        Update(geocache);
                        ok = true;          // Required, will loop infinitely..
                    }
                    else
                        throw;
                }
            }
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
            Update(geocache, 0);
        }

        private void Update(Geocache geocache, int attempt = 0, SqlCommand update = null)
        {
            if (update == null)
            {
                update = CreateCommand();
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
                update.Parameters.AddWithValue("@Name", geocache.Name);
                update.Parameters.AddWithValue("@CacheID", geocache.CacheID);
                update.Parameters.AddWithValue("@Country", geocache.Country);
                update.Parameters.AddWithValue("@Description", geocache.Description);
                update.Parameters.AddWithValue("@Difficulty", geocache.Difficulty);
                update.Parameters.AddWithValue("@LongDescription", geocache.LongDescription);
                update.Parameters.AddWithValue("@Owner", geocache.Owner);
                update.Parameters.AddWithValue("@ShortDescription", geocache.ShortDescription);
                update.Parameters.AddWithValue("@Size", geocache.Size);
                update.Parameters.AddWithValue("@State", geocache.State);
                update.Parameters.AddWithValue("@StatusArchived", geocache.StatusArchived);
                update.Parameters.AddWithValue("@StatusAvailable", geocache.StatusAvailable);
                update.Parameters.AddWithValue("@Symbol", geocache.Symbol);
                update.Parameters.AddWithValue("@SymbolType", geocache.SymbolType);
                update.Parameters.AddWithValue("@Terrain", geocache.Terrain);
                update.Parameters.AddWithValue("@Time", geocache.Time);
                update.Parameters.AddWithValue("@Type", geocache.Type);
                update.Parameters.AddWithValue("@URL", geocache.URL);
                update.Parameters.AddWithValue("@URLName", geocache.URLName);
                update.Parameters.AddWithValue("@Latitude", geocache.Latitude);
                update.Parameters.AddWithValue("@Longitude", geocache.Longitude);
                update.Parameters.AddWithValue("@Altitude", geocache.Altitude);
                update.Parameters.AddWithValue("@LastChanged", geocache.LastChanged);
                update.Parameters.AddWithValue("@GeocacheID", geocache.GeocacheID);
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
                        Console.WriteLine($"Attempting to Update {geocache.CacheID}, attempt {attempt++}");
                        //System.Threading.Thread.Sleep(new Random().Next(5000));
                        Update(geocache, attempt, update);
                    }
                    else
                        throw;
                }
            }
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
            List<SqlParameter> parameters = new List<SqlParameter>(24)
            {
                new SqlParameter("Name", geocache.Name),
                new SqlParameter("CacheID", geocache.CacheID),
                new SqlParameter("Country", geocache.Country),
                new SqlParameter("Description", geocache.Description),
                new SqlParameter("Difficulty", geocache.Difficulty),
                new SqlParameter("LongDescription", geocache.LongDescription),
                new SqlParameter("Owner", geocache.Owner),
                new SqlParameter("ShortDescription", geocache.ShortDescription),
                new SqlParameter("Size", geocache.Size),
                new SqlParameter("State", geocache.State),
                new SqlParameter("StatusArchived", geocache.StatusArchived),
                new SqlParameter("StatusAvailable", geocache.StatusAvailable),
                new SqlParameter("Symbol", geocache.Symbol),
                new SqlParameter("SymbolType", geocache.SymbolType),
                new SqlParameter("Terrain", geocache.Terrain),
                new SqlParameter("Time", geocache.Time),
                new SqlParameter("Type", geocache.Type),
                new SqlParameter("URL", geocache.URL),
                new SqlParameter("URLName", geocache.URLName),
                new SqlParameter("Latitude", geocache.Latitude),
                new SqlParameter("Longitude", geocache.Longitude),
                new SqlParameter("Altitude", geocache.Altitude),
                new SqlParameter("LastChanged", geocache.LastChanged),
                new SqlParameter("GeocacheID", geocache.GeocacheID)
            };
            return parameters.ToArray();
        }
    }
}