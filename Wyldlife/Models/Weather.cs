using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Wyldlife.Models
{
    public class ForecastDay
    {
        public int high;
        public int low;
        public string icon;
        public string description;
    }
    public class Weather
    {
        public string weatherData;
        public int currentTemp;
        public string currentIcon;
        public string currentDescription;
        public ForecastDay[] forecast;

        public Weather(string weather)
        {
            weatherData = weather;
            dynamic weatherObj = JObject.Parse(weather);
            currentTemp = weatherObj.current.temp;
            currentDescription = weatherObj.current.weather.description;
            currentIcon = getEmoji(weatherObj.current.weather.icon);
            forecast = new ForecastDay[8];
            for(int i =0; i < 8; i++)
            {
                forecast[i].high = weatherObj.daily[i].temp.max;
                forecast[i].low = weatherObj.daily[i].temp.min;
                forecast[i].description = weatherObj.daily[i].weather.description;
                forecast[i].icon = getEmoji(weatherObj.daily[i].weather.icon);
            }
        }
        private string getEmoji(string iconName)
        {
            switch (iconName)
            {
                case "01d":
                    return "☀️";
                case "01n":
                    return "🌕";
                case "02d":
                    return "🌤️";
                case "02n":
                    return "☁️";
                case "03d":
                    return "🌥️";
                case "03n":
                    return "☁️";
                case "04d":
                    return "☁️";
                case "04n":
                    return "☁️";
                case "09d":
                    return "🌧️";
                case "09n":
                    return "🌧️";
                case "10d":
                    return "🌦️";
                case "10n":
                    return "🌧️";
                case "11d":
                    return "⛈️";
                case "11n":
                    return "⛈️";
                case "13d":
                    return "🌨️";
                case "13n":
                    return "🌨️";
                case "50d":
                    return "🌫️";
                case "50n":
                    return "🌫️";
                default:
                    return "🌡";

            }
        }
    }
}
