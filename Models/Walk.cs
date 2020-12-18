using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Models
{
    public class Walk
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Duration { get; set; }
        public Walker walker { get; set; }
        public int DogId { get; set; }
        public Dog Dog { get; set; }
        public Owner owner { get; set; }
        public Boolean Accepted { get; set; }
        public Boolean Completed { get; set; }
    }
}
