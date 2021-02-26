using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.Language.Extensions;
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
            command.CommandText = @"SELECT id,title,author,lat,long,descrip,note
                                    FROM dbo.Locations;";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var loc = new Location();
                    loc.Id = reader.GetGuid(0);
                    loc.Title = reader.GetString(1);
                    loc.Author = reader.GetString(2);
                    loc.Coords = new Tuple<double, double>(reader.GetDouble(3), reader.GetDouble(4));
                    loc.Description = reader.GetString(5);
                    loc.Notes = reader.GetString(6);
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
                INSERT INTO dbo.Locations (id, title, author, lat, long, descrip, note) VALUES(
                    @id, @title, @author, @lat, @long, @descrip, @note);";
            command.Parameters.AddWithValue("@id", loc.Id);
            command.Parameters.AddWithValue("@title", loc.Title);
            command.Parameters.AddWithValue("@author", loc.Author);
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
        public void DeleteLocation(Guid uuid)
        {
            var command = Connection.CreateCommand();
            command.CommandText =
                @"DELETE FROM dbo.Locations
                    WHERE id=@uuid;";
            command.Parameters.AddWithValue("@uuid", uuid.ToString());
            command.ExecuteNonQuery();
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
            command.CommandText = @"SELECT id,title,author,lat,long,descrip,note
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
                    loc.Id = reader.GetGuid(0);
                    loc.Title = reader.GetString(1);
                    loc.Author = reader.GetString(2);
                    loc.Coords = new Tuple<double, double>(reader.GetDouble(3), reader.GetDouble(4));
                    loc.Description = reader.GetString(5);
                    loc.Notes = reader.GetString(6);
                    locations.Add(loc);
                }
            }
            return locations;
        }

        /// <summary>
        /// Gets locations matching a given text query, location, and radius.
        /// </summary>
        /// <param name="query">The query to search the database for.</param>
        /// <param name="coords">Current position.</param>
        /// <param name="radius">Searching radius</param>
        /// <returns>A list of locations matching the query and location constraints..</returns>
        public List<Location> GetLocationsBySearch(string query, Tuple<double,double> coords, int radius)
        {
            List<Location> locations = new List<Location>();
            var command = Connection.CreateCommand();
            command.CommandText = @"SELECT id,title,author,lat,long,descrip,note
                                    FROM dbo.Locations
                                    WHERE LOWER(title) LIKE LOWER(@query)
                                    AND lat BETWEEN @minlat AND @maxlat
                                    AND long BETWEEN @minlong AND @maxlong;";
            query = "%" + query + "%";
            command.Parameters.AddWithValue("@query", query);
            command.Parameters.AddWithValue("@minlat", (coords.Item1 - (Convert.ToDouble(radius) / Convert.ToDouble(69))));
            command.Parameters.AddWithValue("@maxlat", (coords.Item1 + (Convert.ToDouble(radius) / Convert.ToDouble(69))));
            command.Parameters.AddWithValue("@minlong", (coords.Item2 - (Convert.ToDouble(radius) / Convert.ToDouble(54.6))));
            command.Parameters.AddWithValue("@maxlong", (coords.Item2 + (Convert.ToDouble(radius) / Convert.ToDouble(54.6))));
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine(reader.GetString(1));
                    var loc = new Location();
                    loc.Id = reader.GetGuid(0);
                    loc.Title = reader.GetString(1);
                    loc.Author = reader.GetString(2);
                    loc.Coords = new Tuple<double, double>(reader.GetDouble(3), reader.GetDouble(4));
                    loc.Description = reader.GetString(5);
                    loc.Notes = reader.GetString(6);
                    locations.Add(loc);
                }
            }
            return locations;
        }

        /// <summary>
        /// Gets locations from the database that match the given query.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <returns>A list of Locations matching the search query.</returns>
        public List<Location> GetLocationsBySearch(string query)
        {
            List<Location> locations = new List<Location>();
            var command = Connection.CreateCommand();
            command.CommandText = @"SELECT id,title,author,lat,long,descrip,note
                                    FROM dbo.Locations
                                    WHERE LOWER(title) LIKE LOWER(@query);";
            query = "%" + query + "%";
            command.Parameters.AddWithValue("@query", query);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine(reader.GetString(1));
                    var loc = new Location();
                    loc.Id = reader.GetGuid(0);
                    loc.Title = reader.GetString(1);
                    loc.Author = reader.GetString(2);
                    loc.Coords = new Tuple<double, double>(reader.GetDouble(3), reader.GetDouble(4));
                    loc.Description = reader.GetString(5);
                    loc.Notes = reader.GetString(6);
                    locations.Add(loc);
                }
            }
            return locations;
        }

        /// <summary>
        /// Edit a location in the database.
        /// Uses the UUID from the location object to determine which location to edit.
        /// </summary>
        /// <param name="location">Newly edited location object.</param>
        public void EditLocation(Location location)
        {
            var command = Connection.CreateCommand();
            command.CommandText = @"UPDATE dbo.Locations
                                    SET title=@title, lat=@lat, long=@long, descrip=@descrip, note=@note
                                    WHERE id=@id";
            command.Parameters.AddWithValue("@title", location.Title);
            command.Parameters.AddWithValue("@lat", location.Coords.Item1);
            command.Parameters.AddWithValue("@long", location.Coords.Item2);
            if(location.Description == null)
            {
                location.Description = "N/A";
            }
            if(location.Notes == null)
            {
                location.Notes = "N/A";
            }
            command.Parameters.AddWithValue("@descrip", location.Description);
            command.Parameters.AddWithValue("@note", location.Notes);
        }

        /// <summary>
        /// Gets the rating of the given location.
        /// </summary>
        /// <param name="locationId">The location ID.</param>
        /// <returns>An integer out of 5 representing the rating.</returns>
        public Tuple<int,int> GetRating(Guid locationId)
        {
            int rating, numRates;
            rating = 5;
            numRates = 0;
            var command = Connection.CreateCommand();
            command.CommandText = @"SELECT AVG(rating), COUNT(rating) FROM dbo.Reviews
                                    WHERE locationId=@id;";
            command.Parameters.AddWithValue("@id", locationId);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read()) {
                    numRates = reader.GetInt32(1);
                    if (numRates == 0)
                    {
                        rating = 0;
                    }
                    else
                    {
                        rating = reader.GetInt32(0);
                    }
                }
            }

            return new Tuple<int, int>(rating, numRates);
        }
        /// <summary>
        /// Get all the reviews for a given location.
        /// </summary>
        /// <param name="locationId">The location ID.</param>
        /// <returns>A list of Reviews from the given location.</returns>
        public List<Review> GetReviews(Guid locationId)
        {
            List<Review> reviews = new List<Review>();
            var command = Connection.CreateCommand();
            command.CommandText = @"SELECT locationId, author, rating, reviewText
                                    FROM dbo.Reviews
                                    WHERE locationId=@id;";
            command.Parameters.AddWithValue("@id", locationId);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var review = new Review();
                    review.LocationId = reader.GetGuid(1);
                    review.Author = reader.GetString(2);
                    review.Rating = reader.GetInt16(3);
                    review.ReviewText = reader.GetString(4);
                    reviews.Add(review);
                }
            }
            return reviews;
        }

        /// <summary>
        /// Add a review to the database.
        /// </summary>
        /// <param name="review">The review to add to the database.</param>
        public void AddReview(Review review)
        {
            var command = Connection.CreateCommand();
            command.CommandText = @"INSERT INTO dbo.Reviews
                                    (locationId, author, rating, reviewText)
                                    VALUES(
                                    @locationId, @author, @rating, @reviewText);";
            command.Parameters.AddWithValue("@locationId", review.LocationId);
            command.Parameters.AddWithValue("@author", review.Author);
            command.Parameters.AddWithValue("@rating", review.Rating);
            if(review.ReviewText == null)
            {
                review.ReviewText = "N/A";
            }
            else
            {
                command.Parameters.AddWithValue("@reviewText", review.ReviewText);
            }
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes the specified review from the database.
        /// </summary>
        /// <param name="locationId">The location of the review.</param>
        /// <param name="author">The author of the review.</param>
        public void DeleteReview(Guid locationId, string author)
        {
            var command = Connection.CreateCommand();
            command.CommandText = @"DELETE FROM dbo.Reviews
                                    WHERE author=@author AND locationId=@locationId;";
            command.Parameters.AddWithValue("@author", author);
            command.Parameters.AddWithValue("@locationId", locationId);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Edits a review on the server.
        /// The review to edit is specified by locationID and Author of the supplied review.
        /// </summary>
        /// <param name="review">Edited review object.</param>
        public void EditReview(Review review)
        {
            var command = Connection.CreateCommand();
            command.CommandText = @"UPDATE dbo.Reviews
                                    SET rating=@rating, reviewText=@reviewText
                                    WHERE locationId=@locationId AND author=@author;";
            command.Parameters.AddWithValue("@rating", review.Rating);
            command.Parameters.AddWithValue("@reviewText", review.ReviewText);
            command.Parameters.AddWithValue("@locationId", review.LocationId);
            command.Parameters.AddWithValue("@author", review.Author);
        }
    }

    
}
