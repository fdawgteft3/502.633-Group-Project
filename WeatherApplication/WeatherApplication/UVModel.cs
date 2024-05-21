using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherApplication
{
    public class UVModel
    {
        private readonly HttpClientWrapper m_httpClient = HttpClientWrapper.Instance;
        private string m_apiKey;

        public string Coord { get; set; }
        public void GetCoord() 
        { 
            
        }

        public class UVData
        {
            [Required]
            public List<UVProduct> Products { get; set; }

            [Required]
            public string coordinate { get; set; }
        }

        public class UVProduct
        {
            [Required]
            public string Name { get; set; }

            [Required]
            public List<UVDataEntry> Values { get; set; }
        }

        public class UVDataEntry
        {
            [Required]
            public DateTime Time { get; set; }

            [Required]
            [Range(0, double.MaxValue)]
            public double Value { get; set; }
        }

        public UVModel(string apiKey, CoordInfo inputCoord)
        {
            m_apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public class UVDataDeserializer
        {
            public UVData Deserialize(string json)
            {
                var uvData = new UVData { Products = new List<UVProduct>() };

                var jsonObject = JObject.Parse(json);
                if (jsonObject == null)
                {
                    throw new ArgumentNullException(nameof(json), "JSON object is null.");
                }

                // Check for the "coord" property
                var coordToken = jsonObject["coord"];
                if (coordToken == null)
                {
                    throw new JsonSerializationException("Missing 'coord' property in JSON object.");
                }
                uvData.coordinate = coordToken.Value<string>();

                // Check for the "products" array
                var productsToken = jsonObject["products"];
                if (productsToken == null || !productsToken.HasValues)
                {
                    throw new JsonSerializationException("Missing 'products' array in JSON object.");
                }

                foreach (var productJson in productsToken)
                {
                    // Check for the "name" property
                    var nameToken = productJson["name"];
                    if (nameToken == null)
                    {
                        throw new JsonSerializationException("Missing 'name' property in product JSON object.");
                    }

                    var uvProduct = new UVProduct
                    {
                        Name = nameToken.Value<string>(),
                        Values = new List<UVDataEntry>()
                    };

                    // Check for the "values" array
                    var valuesToken = productJson["values"];
                    if (valuesToken == null || !valuesToken.HasValues)
                    {
                        throw new JsonSerializationException("Missing 'values' array in product JSON object.");
                    }

                    foreach (var valueJson in valuesToken)
                    {
                        // Check for the "time" property
                        var timeToken = valueJson["time"];
                        if (timeToken == null)
                        {
                            throw new JsonSerializationException("Missing 'time' property in value JSON object.");
                        }

                        // Check for the "value" property
                        var valueToken = valueJson["value"];
                        if (valueToken == null)
                        {
                            throw new JsonSerializationException("Missing 'value' property in value JSON object.");
                        }

                        var uvDataEntry = new UVDataEntry
                        {
                            Time = DateTime.Parse(timeToken.ToString()),
                            Value = double.Parse(valueToken.ToString())
                        };
                        uvProduct.Values.Add(uvDataEntry);
                    }
                    uvData.Products.Add(uvProduct);
                }
                return uvData;
            }
        }

        internal async Task<UVData> GetUVDataAsync(string apiKey, CoordInfo coordInfo)
        {

            try
            {
                double lat = coordInfo.Lat;
                double lon = coordInfo.Lon;
                string encodedApiKey = Uri.EscapeDataString(apiKey);
                string url = $"https://api.niwa.co.nz/uv/data?lat={lat}&long={lon}&apikey={apiKey}&units=metric";
                Console.WriteLine(url);
                HttpResponseMessage response = await m_httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                UVDataDeserializer deserializer = new UVDataDeserializer();
                UVData uVData = deserializer.Deserialize(json) ?? throw new Exception("Weather data deserialization failed.");
                return uVData;
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
}

