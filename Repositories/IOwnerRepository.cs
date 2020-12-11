using DogGo.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public interface IOwnerRepository
    {
        List<Owner> GetAllOwners();
        OwnerDogs GetOwnerById(int id);
    }
}
