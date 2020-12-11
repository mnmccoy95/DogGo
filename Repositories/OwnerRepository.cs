using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class OwnerRepository : IOwnerRepository
    { 
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public OwnerRepository(IConfiguration config)
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

        public List<Owner> GetAllOwners()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            SELECT Owner.Id, Owner.Name, Owner.NeighborhoodId, Owner.Address, Owner.Phone, n.Name AS NeighborhoodName
                            FROM Owner
                            JOIN Neighborhood n ON Owner.NeighborhoodId = n.Id";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Owner> owners = new List<Owner>();
                    while (reader.Read())
                    {
                        Owner owner = new Owner
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            Phone = reader.GetString(reader.GetOrdinal("Phone")),
                            Neighborhood = new Neighborhood()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                                Name = reader.GetString(reader.GetOrdinal("NeighborhoodName"))
                            }
                        };

                        owners.Add(owner);
                    }

                    reader.Close();

                    return owners;
                }
            }
        }

        public OwnerDogs GetOwnerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    List<Dog> dogs = new List<Dog>();
                    cmd.CommandText = @"
                            SELECT Owner.Id, Owner.Name, Owner.NeighborhoodId, Owner.Address, Owner.Phone, n.Name AS NeighborhoodName, d.Name as DogName
                            FROM Owner
                            JOIN Neighborhood n ON Owner.NeighborhoodId = n.Id
                            JOIN Dog d ON d.OwnerId = @id2
                            WHERE Owner.Id = @id2
                        ";

                    cmd.Parameters.AddWithValue("@id2", id);

                    SqlDataReader reader2 = cmd.ExecuteReader();
                    while (reader2.Read())
                    {
                        Dog dog = new Dog
                        {
                            Name = reader2.GetString(reader2.GetOrdinal("DogName"))
                        };
                        dogs.Add(dog);
                    };
                    reader2.Close();

                    cmd.CommandText = @"
                            SELECT Owner.Id, Owner.Name, Owner.NeighborhoodId, Owner.Address, Owner.Phone, n.Name AS NeighborhoodName, d.Name as DogName
                            FROM Owner
                            JOIN Neighborhood n ON Owner.NeighborhoodId = n.Id
                            JOIN Dog d ON d.OwnerId = @id
                            WHERE Owner.Id = @id
                        ";

                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Owner ownerWanted = new Owner
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            Phone = reader.GetString(reader.GetOrdinal("Phone")),
                            Neighborhood = new Neighborhood()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                                Name = reader.GetString(reader.GetOrdinal("NeighborhoodName"))
                            }
                        };
                       
                        OwnerDogs ownerDogs = new OwnerDogs
                        {
                            dogList = dogs,
                            owner = ownerWanted
                        };

                        reader.Close();
                        return ownerDogs;
                    }
                    else
                    {
                        reader.Close();
                        return null;
                    }
                }
            }
        }
    }
}
