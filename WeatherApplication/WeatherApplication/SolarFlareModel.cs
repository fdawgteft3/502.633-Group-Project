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

        public async Task<SolarFlareDataArr> GetSolarDataAsync(string apiKey, string startDate, string endDate)
        {
            try
            {
                string encodedApiKey = Uri.EscapeDataString(m_apiKey);
                string url = $"https://api.nasa.gov/DONKI/FLR?startDate={startDate}&endDate={endDate}&api_key={apiKey}";
                HttpResponseMessage response = await m_httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                SolarFlareDeserializer deserializer = new SolarFlareDeserializer();
                SolarFlareDataArr solarData = deserializer.DeserializeSolarFlareData(json) ?? throw new Exception("Solar flare data deserialization failed.");
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
        public string FlareID { get; }
        public string BeginTime { get; }
        public string PeakTime { get; }
        public string EndTime { get; }
        public string ClassType { get; }
        public string Note { get; }

        public SolarFlareData(string flareID, string beginTime, string peakTime, string endTime, string classType, string note)
        {
            FlareID = flareID;
            BeginTime = beginTime;
            PeakTime = peakTime;
            EndTime = endTime;
            ClassType = classType;
            Note = note;

        }
    }

    public class SolarFlareDataArr
    {
        public SolarFlareData[] Flares { get; set; }
        public SolarFlareDataArr(int flareCount)
        {
            Flares = new SolarFlareData[flareCount];
        }
    }

    public class SolarFlareDeserializer
    {
        public SolarFlareDataArr DeserializeSolarFlareData(string json)
        {
            JArray jsonArray = JArray.Parse(json);
            int flareCount = jsonArray.Count;
            SolarFlareDataArr arr = new SolarFlareDataArr(flareCount);

            int count = 0;
            foreach (JObject jsonObject in jsonArray.Children<JObject>())
            {
                //Console.WriteLine(jsonObject.ToString());
                var flareIdObject = jsonObject["flrID"] ?? throw new Exception("flrID object is missing in JSON.");
                var beginTimeObject = jsonObject["beginTime"] ?? throw new Exception("beginTime object is missing in JSON.");
                var peakTimeObject = jsonObject["peakTime"] ?? throw new Exception("peakTime object is missing in JSON.");
                var endTimeObject = jsonObject["endTime"] ?? throw new Exception("endTime object is missing in JSON.");
                var classTypeObject = jsonObject["classType"] ?? throw new Exception("classType object is missing in JSON.");
                var noteObject = jsonObject["note"] ?? throw new Exception("note object is missing in JSON.");

                var flareId = flareIdObject.Value<string>();
                var beginTime = beginTimeObject.Value<string>();
                var peakTime = peakTimeObject.Value<string>();
                var endTime = endTimeObject.Value<string>();
                var classType = classTypeObject.Value<string>();
                var note = noteObject.Value<string>();

                var solarData = new SolarFlareData(
                    flareId, beginTime, peakTime, endTime, classType, note
                    );
                arr.Flares[count] = solarData;
                count += 1;
            }
            
                return arr;
        }


    }
    public class SolarFlareServiceException : Exception
    {
        public SolarFlareServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
