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
                    loc.Id = reader.GetGuid(0);
                    loc.Title = reader.GetString(1);
                    loc.Coords = new Tuple<double, double>(reader.GetDouble(2), reader.GetDouble(3));
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
        public void AddLocation(Location loc)
        {
            var command = Connection.CreateCommand();
            command.CommandText =
                @"
                INSERT INTO dbo.Locations (id, title, lat, long, descrip, note) VALUES(
                    @id,
                    @title,
                    @lat,
                    @long,
                    @descrip,
                    @note
                    );";
            command.Parameters.AddWithValue("@id", loc.Id);
            command.Parameters.AddWithValue("@title", loc.Title);
            command.Parameters.AddWithValue("@lat", loc.Coords.Item1);
            command.Parameters.AddWithValue("@long", loc.Coords.Item2);
            if(loc.Description == null)
            {
                loc.Description = "N/A";
            }
            command.Parameters.AddWithValue("@descrip", loc.Description);
            if(loc.Notes == null)
            {
                loc.Notes = "N/A";
            }
            command.Parameters.AddWithValue("@note", loc.Notes);
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
                    WHERE id=@uuid;";
            command.Parameters.AddWithValue("@uuid", uuid.ToString());
        }

        /// <summary>
        /// Search for locations within a given radius of a given position.
        /// </summary>
        /// <param name="coords">The position to search from.</param>
        /// <param name="radius">The search radius, in miles.</param>
        /// <returns>A list of locations within the radius of the position.</returns>
        public List<Location> GetLocationsByDistance(Tuple<double, double> coords, int radius)
        {
            // TODO: Changes in lat/long cause inaccurate measurements.
            // TODO: Account for when long/lat are close to zero.
            List<Location> locations = new List<Location>();
            var command = Connection.CreateCommand();
            command.CommandText = @"SELECT id,title,lat,long,descrip,note
                                    FROM dbo.Locations
                                    WHERE lat BETWEEN @minlat AND @maxlat
                                    AND long BETWEEN @minlong AND @maxlong;";
            command.Parameters.AddWithValue("@minlat", (coords.Item1 - (Convert.ToDouble(radius)/Convert.ToDouble(69))));
            command.Parameters.AddWithValue("@maxlat", (coords.Item1 + (Convert.ToDouble(radius)/Convert.ToDouble(69))));
            command.Parameters.AddWithValue("@minlong", (coords.Item2 - (Convert.ToDouble(radius) / Convert.ToDouble(54.6))));
            command.Parameters.AddWithValue("@maxlong", (coords.Item2 + (Convert.ToDouble(radius) / Convert.ToDouble(54.6))));
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var loc = new Location();
                    loc.Id = Guid.Parse(reader.GetString(0));
                    loc.Title = reader.GetString(1);
                    loc.Coords = new Tuple<double, double>(reader.GetDouble(2), reader.GetDouble(3));
                    loc.Description = reader.GetString(4);
                    loc.Notes = reader.GetString(5);
                    locations.Add(loc);
                }
            }
            return locations;
        }
    }

    
}
