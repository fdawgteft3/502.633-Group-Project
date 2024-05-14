using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherApplication
{

    public class SolarFlareModel
    {
        private readonly HttpClientWrapper m_httpClient = HttpClientWrapper.Instance;
        private readonly string m_apiKey;

        public SolarFlareModel(string apiKey)
        {
            m_apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task<SolarFlareData> GetSolarDataAsync(string apiKey, DateTime startDate, DateTime endDate)
        {
            try
            {
                string encodedApiKey = Uri.EscapeDataString(m_apiKey);
                string url = $"https://api.nasa.gov/DONKI/FLR?startDate={startDate}&endDate={endDate}&api_key={apiKey}";
                HttpResponseMessage response = await m_httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                SolarFlareDeserializer deserializer = new SolarFlareDeserializer();
                SolarFlareData solarData = deserializer.DeserializeSolarFlareData(json) ?? throw new Exception("Weather data deserialization failed.");
                return solarData;
            }
            catch (HttpRequestException ex)
            {
                throw new WeatherServiceException("Error occurred while sending the HTTP request.", ex);
            }
            catch (JsonException ex)
            {
                throw new WeatherServiceException("Error occurred while parsing JSON response.", ex);
            }
            catch (Exception ex)
            {
                throw new WeatherServiceException("An error occurred during the asynchronous operation.", ex);
            }
        }
    }

    public class SolarFlareData
    {
        public DateTime BeginTime { get; }
        public DateTime PeakTime { get; }
        public DateTime EndTime { get; }
        public string ClassType { get; }
        public string Note { get; }

        public SolarFlareData(DateTime beginTime, DateTime peakTime, DateTime endTime, string classType, string note)
        {
            BeginTime = beginTime;
            PeakTime = peakTime;
            EndTime = endTime;
            ClassType = classType;
            Note = note;

        }
    }

    public class SolarFlareDeserializer
    {
        public SolarFlareData DeserializeSolarFlareData(string json)
        {
            var jsonObject = JObject.Parse(json);

            var beginTimeObject = jsonObject["beginTime"] ?? throw new Exception("beginTime object is missing in JSON.");
            var peakTimeObject = jsonObject["peakTime"] ?? throw new Exception("peakTime object is missing in JSON.");
            var endTimeObject = jsonObject["endTime"] ?? throw new Exception("endTime object is missing in JSON.");
            var classTypeObject = jsonObject["classType"] ?? throw new Exception("classType object is missing in JSON.");
            var noteObject = jsonObject["note"] ?? throw new Exception("note object is missing in JSON.");

            var beginTime = beginTimeObject.Value<DateTime>();
            var peakTime = peakTimeObject.Value<DateTime>();
            var endTime = endTimeObject.Value<DateTime>();
            var classType = classTypeObject.Value<string>();
            var note = noteObject.Value<string>();

            var solarData = new SolarFlareData(
                beginTime, peakTime, endTime, classType, note
                );
                return solarData;
        }
    }
  
}
