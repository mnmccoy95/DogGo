using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Models.ViewModels
{
    public class WalkFormViewModel
    {
        public Walk walk { get; set; }
        public List<Dog> dogs { get; set; }
    }
}
