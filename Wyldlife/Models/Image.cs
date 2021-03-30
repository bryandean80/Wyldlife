using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wyldlife.Models
{
    public class Image
    {
        public byte[] image { get; set; }
        public string Author { get; set; }
        public Guid locationId { get; set; }

    }
}
