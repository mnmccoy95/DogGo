using DogGo.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public interface IWalkRepository
    {
        void AddWalk(Walk walk);
        List<Walk> GetWalksByOwner(int id);
        List<Walk> GetWalksByDog(int id);
    }
}
