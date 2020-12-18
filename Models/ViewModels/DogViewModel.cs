using System.Collections.Generic;

namespace DogGo.Models.ViewModels
{
    public class DogViewModel
    {
        public Dog Dog { get; set; }
        public List<Walk> Walks { get; set; }
    }
}