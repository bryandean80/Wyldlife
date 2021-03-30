using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.IO;
using Wyldlife.Models;

namespace Wyldlife.Services
{
    public class ImageService
    {
        public IWebHostEnvironment WebHostEnvironment { get; }
        private SqlConnection Connection { get; }
        public ImageService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
            Connection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=aspnet-Wyldlife-B341C1BA-19F8-4AF7-879B-D899FC66C8B3;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            Connection.Open();
        }
        public void AddImage(Guid locationId, string author, byte[] image)
        {
            var command = Connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO dbo.Images (locationId, author, img)
                    VALUES(@locationId, @author, @img);";
            command.Parameters.AddWithValue("@locationId", locationId);
            command.Parameters.AddWithValue("@author", author);
            command.Parameters.Add("@img", System.Data.SqlDbType.Binary);
            command.Parameters["@img"].Value = image;
            command.ExecuteNonQuery();
        }

        public void AddStory(Guid locationId, string author, byte[] image)
        {
            var command = Connection.CreateCommand();
            command.CommandText = @"
                                    INSERT INTO dbo.Images (locationId, author, img, isStory)
                                    VALUES(@locationId, @author, @img, 1)";
            command.Parameters.AddWithValue("@locationId", locationId);
            command.Parameters.AddWithValue("@author", author);
            command.Parameters.Add("@img", System.Data.SqlDbType.Binary);
            command.Parameters["@img"].Value = image;
            command.ExecuteNonQuery();
        }

        public List<Image> getStories()
        {
            List<Image> images = new List<Image>();
            var command = Connection.CreateCommand();
            command.CommandText = @"DELETE FROM dbo.Images WHERE isStory=1 AND uploaded < GETDATE() - 1;
                                    SELECT locationId,author,img FROM dbo.Images
                                    WHERE isStory=1 AND uploaded>= GETDATE() - 1;";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Image newImage = new Image();
                    newImage.locationId = reader.GetGuid(0);
                    newImage.Author = reader.GetString(1);
                    var sourceStream = reader.GetStream(2);
                    using (var memoryStream = new MemoryStream())
                    {
                        sourceStream.CopyTo(memoryStream);
                        newImage.image = memoryStream.ToArray();
                    }
                    images.Add(newImage);
                }
            }
            return images;
        }

        public List<byte[]> getImages(Guid locationId)
        {
            List<byte[]> images = new List<byte[]>();
            var command = Connection.CreateCommand();

            // Add satellite and terrain images
            command.CommandText = @"SELECT satellite,terrain FROM dbo.Maps
                                        WHERE locationId=@locationId AND isStory=0;";
            command.Parameters.AddWithValue("@locationId", locationId);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var sourceStream = reader.GetStream(0);
                    using (var memoryStream = new MemoryStream())
                    {
                        sourceStream.CopyTo(memoryStream);
                        images.Add(memoryStream.ToArray());
                    }
                    sourceStream = reader.GetStream(1);
                    using (var memoryStream = new MemoryStream())
                    {
                        sourceStream.CopyTo(memoryStream);
                        images.Add(memoryStream.ToArray());
                    }
                }


            }
            command.CommandText = @"SELECT img FROM dbo.Images
                                        WHERE locationId=@locationId";
            
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var sourceStream = reader.GetStream(0);
                    using (var memoryStream = new MemoryStream())
                    {
                        sourceStream.CopyTo(memoryStream);
                        images.Add(memoryStream.ToArray());
                    }
                }
            }

            return images;
        }
    }

    
}
