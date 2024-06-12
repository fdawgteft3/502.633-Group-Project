using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using WeatherApplication; 

namespace WeatherApplication
{
    public class WeatherApplicationView 
    {
        public void Render(WeatherData weatherData)
        {
            if (null == weatherData)
            {
                Console.WriteLine("Weather data is null.");
                return;

            }
            else
            {
                Console.WriteLine($"Weather for {weatherData.Name}:");
                RenderCoordInfo(weatherData.Coord);
                RenderWeatherInfo(weatherData.Weather);
                Console.WriteLine($"Base: {weatherData.Base}");
                RenderMainInfo(weatherData.Main);
                Console.WriteLine($"Visibility: {weatherData.Visibility}");
                RenderWindInfo(weatherData.Wind);
                RenderCloudInfo(weatherData.Clouds);
                Console.WriteLine($"Date and Time: {UnixTimeStampToDateTime(weatherData.Dt)}"); // Convert UNIX timestamp to DateTime
                RenderSysInfo(weatherData.Sys);
                Console.WriteLine($"Timezone: {weatherData.Timezone}");
                Console.WriteLine($"City ID: {weatherData.Id}");
                Console.WriteLine($"City Name: {weatherData.Name}");
                Console.WriteLine($"Cod: {weatherData.Cod}");
            }
        }
        private void RenderCoordInfo(CoordInfo coord)
        {
            if(null == coord)
            {
                return;
            }
            Console.WriteLine($"Coordinates: Lon: {coord.Lon}, Lat: {coord.Lat}"); 

        }
        private void RenderWeatherInfo(List<WeatherInfo> weatherInfo)
        {
            if (weatherInfo == null || weatherInfo.Count == 0 || weatherInfo[0] == null) { return; }
            Console.WriteLine($"Weather Description: {weatherInfo[0].Description}"); // Added null-conditional operator
        }
        private void RenderMainInfo (MainInfo main)
        {
            if(main == null) { return; }
            Console.WriteLine($"Temperature: {main.Temp}°C"); 
            Console.WriteLine($"Feels Like: {main.Feels_like}°C");
            Console.WriteLine($"Minimum Temperature: {main.Temp_min}°C");
            Console.WriteLine($"Maximum Temperature: {main.Temp_max}°C");
            Console.WriteLine($"Pressure: {main.Pressure}hPa");
            Console.WriteLine($"Humidity: {main.Humidity}%"); 
        }
        private void RenderWindInfo(WindInfo wind)
        {
            if (wind == null) { return; }
            Console.WriteLine($"Wind Speed: {wind.Speed}m/s");
            Console.WriteLine($"Wind Degree: {wind.Deg}°"); 
        }
        private void RenderCloudInfo (CloudsInfo cloud)
        {
            if (cloud == null) { return; }
            Console.WriteLine($"Cloudiness: {cloud.All}%"); 
        }
        private void RenderSysInfo(SysInfo sys)
        {
            if(sys == null) { return; }
            Console.WriteLine($"Sys Info: Type: {sys.Type}, ID: {sys.Id}, Country: {sys.Country}, Sunrise: {UnixTimeStampToDateTime(sys.Sunrise)}, Sunset: {UnixTimeStampToDateTime(sys.Sunset)}");
        }

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp).UtcDateTime;
        }
        
    }
}