using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wyldlife.Models
{
    public class Review
    {
        public Guid Id { get; set; }
        public Guid LocationId { get; set; }
        public string Author { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
    }
}
