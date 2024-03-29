﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wyldlife.Models
{
    public class Location
    {
        public string Title { get; set; }
        public Guid Id { get; set; }
        public string Author { get; set; }
        public Tuple<double,double> Coords { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }

        public Location()
        {
            Title = "New Title";
            Id = Guid.NewGuid();
            Coords = Tuple.Create(0.0, 0.0);
            Description = "N/A";
            Notes = "N/A";
            Author = string.Empty;
        }
    }
}
