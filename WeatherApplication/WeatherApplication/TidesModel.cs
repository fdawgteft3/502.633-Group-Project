using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static WeatherApplication.TidesModel;

namespace WeatherApplication
{
    public class TidesModel
    {
        public class TidesData
        {
            public Metadata Metadata { get; set; }
            public List<TideValue> Values { get; set; }
        }

        public class Metadata
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string Datum { get; set; }
            public DateTime Start { get; set; }
            public int Days { get; set; }
            public int Interval { get; set; }
            public string Height { get; set; }
        }

        public class TideValue
        {
            public DateTime Time { get; set; }
            public double Value { get; set; }
        }

        public async Task<TidesData> GetTidesData(double lat, double lon, string apiKey, DateTime startDate, DateTime endDate)
        {
            ValidateApiKey(apiKey);

            TidesData tidesData = GetTideValues(lat, lon,startDate,endDate);

            DateTime currentDate = startDate;
            string resultMessage = "";

            while (currentDate <= endDate)
            {
                resultMessage += await DownloadAndProcessData(lat, lon, apiKey, startDate, tidesData);
                currentDate = currentDate.AddMonths(1);
            }
            resultMessage += "Done";
            return tidesData;
        }
        private void ValidateApiKey (string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentException("API key cannot be null or empty.");
            }
        }
        private TidesData GetTideValues(double lat, double lon, DateTime startDate, DateTime endDate)
        {
            TidesData tidesData = new TidesData
            {
                Metadata = new Metadata
                {
                    Latitude = lat,
                    Longitude = lon,
                    Datum = "MSL", // Assuming default datum is MSL (Mean Sea Level)
                    Start = startDate,
                    Days = (int)(endDate - startDate).TotalDays + 1, // Total number of days including start and end dates
                    Interval = 0, // Assuming no interval between tide measurements
                    Height = "MSL = 0m" // Assuming default height is at Mean Sea Level
                },
                Values = new List<TideValue>()
            };
            return tidesData;
        }
        private string BuildApiURL(double lon, double lat, string apiKey, DateTime startDate)
        {
            // Calculate the number of days in the current month
            int numberOfDays = DateTime.DaysInMonth(startDate.Year, startDate.Month);
            string dateString = startDate.ToString("yyyy-MM-dd");

            return $"https://api.niwa.co.nz/tides/data?lat={lat}&long={lon}&datum=MSL&numberOfDays={numberOfDays}&apikey={apiKey}&startDate={dateString}";
        }
        private async Task<TidesData> FetchTidesData(string fileName, string url)
        {
            using (HttpClientWrapper client = HttpClientWrapper.Instance)
            {

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync();
                File.WriteAllText(fileName, result);

                return JsonConvert.DeserializeObject<TidesData>(result);
            }
        }
        private async Task<string> DownloadAndProcessData(double lat, double lon, string apiKey, DateTime startDate, TidesData tidesData)
        {
            string resultMessage = "";
            string year = startDate.Year.ToString();
            string month = startDate.Month.ToString("D2");
            string filename = $"tides_{year}_{month}.json";
            resultMessage += $"Downloading {startDate:MMM} {year}\n";

            string url = BuildApiURL(lon, lat, apiKey, startDate);

            try
            {
                TidesData monthlyData = await FetchTidesData(url, filename);
                tidesData.Values.AddRange(monthlyData.Values);
            }
            catch (HttpRequestException ex)
            {
                resultMessage += $"An error occurred while downloading tides data: {ex.Message}\n";
            }

            return resultMessage;
        }
    }
}
