using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Wyldlife.Models;

namespace Wyldlife.Services
{
    public class LocationService
    {
        public IWebHostEnvironment WebHostEnvironment { get; }
        private SqlConnection Connection { get; }
        public LocationService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
            Connection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=aspnet-Wyldlife-B341C1BA-19F8-4AF7-879B-D899FC66C8B3;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            Connection.Open();
        }
        /// <summary>
        /// Retrieves a list of all locations in the db.
        /// </summary>
        /// <returns>A list of Location objects.</returns>
        public List<Location> GetLocations()
        {
            List<Location> locations = new List<Location>();
            var command = Connection.CreateCommand();
            command.CommandText = @"SELECT id,title,lat,long,descrip,note
                                    FROM dbo.Locations;";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var loc = new Location();
                    loc.Id = Guid.Parse(reader.GetString(0));
                    loc.Title = reader.GetString(1);
                    loc.Coords = new Tuple<double, double>(reader.GetDouble(2), reader.GetDouble(2));
                    loc.Description = reader.GetString(4);
                    loc.Notes = reader.GetString(5);
                    locations.Add(loc);
                }
            }
                return locations;
        }
        /// <summary>
        /// Adds a location to the database.
        /// </summary>
        /// <param name="loc">Location object to add.</param>
        public void addLocation(Location loc)
        {
            var command = Connection.CreateCommand();
            command.CommandText =
                @"
                INSERT INTO dbo.Locations (title, lat, long, descrip, note) VALUES(
                    $title,
                    $lat,
                    $long,
                    $descrip,
                    $note
                    );";
            command.Parameters.AddWithValue("$title", loc.Title);
            command.Parameters.AddWithValue("$lat", loc.Coords.Item1);
            command.Parameters.AddWithValue("$long", loc.Coords.Item2);
            command.Parameters.AddWithValue("$descrip", loc.Description);
            command.Parameters.AddWithValue("$note", loc.Notes);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes a location from the database.
        /// </summary>
        /// <param name="uuid">ID of the location to remove.</param>
        public void deleteLocation(Guid uuid)
        {
            var command = Connection.CreateCommand();
            command.CommandText =
                @"DELETE FROM dbo.Locations
                    WHERE id=$uuid;";
            command.Parameters.AddWithValue("$uuid", uuid.ToString());
        }
    }
}
