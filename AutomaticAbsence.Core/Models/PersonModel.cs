using System;
using System.Drawing;

namespace AutomaticAbsence.Core.Models
{
    public class PersonModel
    {
        public string Name { get; set; }
        public Bitmap Face { get; set; }
        public DateTime Created { get; set; }
    }
}
