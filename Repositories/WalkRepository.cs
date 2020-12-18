using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DogGo.Repositories
{
    public class WalkRepository : IWalkRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public WalkRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        public void AddWalk(Walk walk)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO Walks (Date, Duration, WalkerId, DogId, Accepted, Complete)
                    OUTPUT INSERTED.ID
                    VALUES (@date, 0, @WalkerId, @DogId, 0, 0);
                ";

                    cmd.Parameters.AddWithValue("@date", walk.Date);
                    cmd.Parameters.AddWithValue("@WalkerId", walk.walker.Id);
                    cmd.Parameters.AddWithValue("@DogId", walk.DogId);

                    int id = (int)cmd.ExecuteScalar();

                    walk.Id = id;
                }
            }
        }
        public List<Walk> GetWalksByOwner(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT s.Id, s.Date, s.Duration, s.WalkerId, s.DogId, o.Name, w.Name AS walkerName, s.Accepted, d.Name AS dogName
                FROM Walks s
                JOIN Walker w ON w.Id = s.WalkerId
                JOIN Dog d ON d.Id = s.DogId
                JOIN Owner o ON o.Id = d.OwnerId
                WHERE d.OwnerId = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walk> walks = new List<Walk>();

                    while (reader.Read())
                    {
                        Walk walk = new Walk()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Duration = reader.GetInt32(reader.GetOrdinal("Duration")) / 60,
                            walker = new Walker()
                            {
                                Name = reader.GetString(reader.GetOrdinal("walkerName"))
                            },
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                            Dog = new Dog()
                            {
                                Name = reader.GetString(reader.GetOrdinal("dogName"))
                            },
                            owner = new Owner()
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            },
                            Accepted = reader.GetBoolean(reader.GetOrdinal("Accepted"))
                        };

                        walks.Add(walk);
                    }
                    reader.Close();
                    return walks;
                }
            }
        }
        public List<Walk> GetWalksByDog(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT s.Id, s.Date, s.Duration, s.WalkerId, s.DogId, o.Name, w.Name AS walkerName, s.Accepted, d.Name AS dogName
                FROM Walks s
                JOIN Walker w ON w.Id = s.WalkerId
                JOIN Dog d ON d.Id = s.DogId
                JOIN Owner o ON o.Id = d.OwnerId
                WHERE d.Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walk> walks = new List<Walk>();

                    while (reader.Read())
                    {
                        Walk walk = new Walk()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Duration = reader.GetInt32(reader.GetOrdinal("Duration")) / 60,
                            walker = new Walker()
                            {
                                Name = reader.GetString(reader.GetOrdinal("walkerName"))
                            },
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                            Dog = new Dog()
                            {
                                Name = reader.GetString(reader.GetOrdinal("dogName"))
                            },
                            owner = new Owner()
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            },
                            Accepted = reader.GetBoolean(reader.GetOrdinal("Accepted"))
                        };

                        walks.Add(walk);
                    }
                    reader.Close();
                    return walks;
                }
            }
        }
        public void ConfirmWalk(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            UPDATE Walks
                            SET 
                                Accepted = 1
                            WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public Walk GetWalkById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Date, Duration, Complete, WalkerId
                        FROM Walks
                        WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Walk walk = new Walk()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                            Completed = reader.GetBoolean(reader.GetOrdinal("Complete")),
                            walker = new Walker()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("WalkerId"))
                            }
                        };

                        reader.Close();
                        return walk;
                    }

                    reader.Close();
                    return null;
                }
            }
        }
        public void CompleteWalk(Walk walk)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            UPDATE Walks
                            SET 
                                Complete = 1,
                                Duration = @duration
                            WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@id", walk.Id);
                    cmd.Parameters.AddWithValue("@duration", walk.Duration);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
