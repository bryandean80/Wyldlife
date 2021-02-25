using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wyldlife.Models
{
    public class Location
    {
        public string Title { get; set; }
        public Guid Id { get; set; }
        public Tuple<double,double> Coords { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
    }
}
