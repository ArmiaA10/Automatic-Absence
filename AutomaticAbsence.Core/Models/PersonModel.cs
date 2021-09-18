using OpenCvSharp;
using System;

namespace AutomaticAbsence.Core.Models
{
    public class PersonModel
    {
        public string Name { get; set; }
        public Mat Face { get; set; }
        public DateTime Created { get; set; }
    }
}
