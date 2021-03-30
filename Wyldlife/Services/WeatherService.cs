using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;
using Wyldlife.Models;

namespace Wyldlife.Services
{
    public class WeatherService
    {
        public IWebHostEnvironment WebHostEnvironment { get; }
        private SqlConnection Connection { get; }
        public WeatherService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
            Connection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=aspnet-Wyldlife-B341C1BA-19F8-4AF7-879B-D899FC66C8B3;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            Connection.Open();
        }

        public async Task<Weather> GetWeather(Location location)
        {
            Weather weather;
            var command = Connection.CreateCommand();
            command.CommandText = @"DELETE FROM dbo.Weather
                                        WHERE updated < DATEADD(HOUR, -1, GETDATE());
                                    SELECT weather FROM dbo.Weather
                                        WHERE locationId=@locationId;";
            command.Parameters.AddWithValue("@locationId", location.Id);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    weather = new Weather(reader.GetString(0));
                }
                else
                {
                    string configFile = File.ReadAllText("./config.json");
                    dynamic config = JObject.Parse(configFile);
                    var client = new HttpClient();
                    var key = config.openweather;
                    string requestString = "https://api.openweathermap.org/data/2.5/onecall?lat="
                                         + location.Coords.Item1 + "&lon=" + location.Coords.Item2
                                         + "&exclude=minutely,hourly,alerts&units=imperial&appid=" + key;
                    string response = await client.GetStringAsync(requestString);
                    weather = new Weather(response);
                }
            }
            return weather;
        }
    }
}
