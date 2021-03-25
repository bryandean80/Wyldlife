using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.IO;

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
            var test = image.ToString();
            var command = Connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO dbo.Images (locationId, author, img)
                    VALUES(@locationId, @author, @img);";
            command.Parameters.AddWithValue("@locationId", locationId);
            command.Parameters.AddWithValue("@author", author);
            //command.Parameters.AddWithValue("@img", image);
            command.Parameters.Add("@img", System.Data.SqlDbType.Binary);
            command.Parameters["@img"].Value = image;
            command.ExecuteNonQuery();
        }

        public List<byte[]> getImages(Guid locationId)
        {
            List<byte[]> images = new List<byte[]>();
            var command = Connection.CreateCommand();

            // Add satellite and terrain images
            command.CommandText = @"SELECT satellite,terrain FROM dbo.Maps
                                        WHERE locationId=@locationId;";
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
