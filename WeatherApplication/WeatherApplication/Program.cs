﻿using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApplication
{
    class Program
    {

        public static async Task SolarApplication()
        {
            //Solar Application
            // Read the API key from the file

            FileEncoder encoder = FileEncoder.Instance;
            encoder.Write("SolarKey", "owdBAVImRXO4HIhMs1FjQbuT7O2QmcOocfJs370L");
            string actualAPIKey = encoder.Read("SolarKey");

            if (string.IsNullOrEmpty(actualAPIKey))
            {
                Console.Write("Enter API key: ");
                actualAPIKey = Console.ReadLine();

                // Write the API key to the file
                encoder.Write("SolarKey", actualAPIKey);
            }

            SolarFlareModel solarFlareService = new SolarFlareModel(actualAPIKey);
            SolarFlareView solarView = new SolarFlareView();
            SolarFlareController solarController = new SolarFlareController(solarFlareService, solarView);
            string solarStartDate = "2016-01-01";
            string solarEndDate = "2016-01-30";

            await solarController.RefreshSolarFlareData(actualAPIKey, solarStartDate, solarEndDate);
            solarController.RefreshPanelView();
        }
        static async Task WeatherApplicataion()
        {
            FileEncoder encoder = FileEncoder.Instance;
            string actualAPIKey = encoder.Read("WeatherKey");
            // Read the API key from the file
            encoder.Write("WeatherKey", "a173994356f879bb3e422754bfdde559");
            actualAPIKey = encoder.Read("WeatherKey");

            // If API key is not found in the file, prompt the user to input it
            if (string.IsNullOrEmpty(actualAPIKey))
            {
                Console.Write("Enter API key: ");
                actualAPIKey = Console.ReadLine();

                // Write the API key to the file
                encoder.Write("WeatherKey", actualAPIKey);
            }

            // Create an instance of WeatherServiceModel with the API key
            WeatherModel weatherService = new WeatherModel(actualAPIKey);

            // Create an instance of WeatherAPIView
            WeatherApplicationView view = new WeatherApplicationView();

            // Instantiate the controller with the view and model
            WeatherAPIController controller = new WeatherAPIController(weatherService, view);

            // Specify the city name for which you want to retrieve weather data
            string cityName = "Pokeno";

            // Retrieve weather data and render the view
            await controller.RefreshWeatherData(actualAPIKey, cityName);
            controller.RefreshPanelView();
        }
        static async Task UVApplication()
        { 

            //Run of the UV Information
            //Coordinates
            WeatherApplication.CoordInfo coord = new CoordInfo(174.72, -37.39);
            //APIKey
            string APIKEYUV = "fLUvOkhsj0ANkkoa3JH7XifMFUOBcVz4";
            //Create instance of UVView
            UVView uvView = new UVView();
            //Create instance of UVModel
            UVModel uVModel = new UVModel(APIKEYUV, coord);
            //Instantiate the controller
            UVController uVController = new UVController(uVModel, uvView);
            //Make sure it has the data
            await uVController.RefreshUVData(APIKEYUV, coord);
        }
        static void HuntingApplication()
        {
            //Run to display Hunting Information
            //Define filepath that holds Hunting Season Information
            String filepath = "hunting_season_data.txt";
            //Create Instance of HuntingView
            var huntingView = new HuntingView();
            //Create instance of Hunting Model
            var huntingModel = new HuntingModel();
            //Create instance of Hunting Controller
            var huntingController = new HuntingController(huntingModel);
            //Defining parsed list of Hunting Seasons from txt file
            List<HuntingModel.HuntingSeason> season;
            try
            {
                //Try to get expected results from Controller Function
                season = huntingController.RefreshHuntingSeasonData(filepath);
            }
            catch(Exception ex)
            {
                //If method fails to execute for any reason - error message to be displayed
                Console.WriteLine($"An Error Occurred: {ex.Message}");
                return;
            }
            //Run code that prints Hunting Information to Console
            huntingView.Render(season);

        }
        

        static async Task TidesApplication()
        {
            FileEncoder encoder = FileEncoder.Instance;
            encoder.Write("TidesKey", "a173994356f879bb3e422754bfdde559");
            string actualAPIKey = encoder.Read("TidesKey");
           



            //Run through to tidal information
            double lat = -37.406;
            double lon = 175.947;

            if (string.IsNullOrWhiteSpace(actualAPIKey))
            {
                Console.WriteLine("Register for an API key at https://developer.niwa.co.nz");
                return;
            }

            DateTime startDate = new DateTime(2023, 01, 01);
            DateTime endDate = new DateTime(2023, 12, 31);

            DateTime currentDate = startDate;

            actualAPIKey = "VtqRNuV5F79dsA8nPGCBhHaCeEJbocPd";

            while (currentDate <= endDate)
            {
                string year = currentDate.Year.ToString();
                string month = currentDate.Month.ToString("D2");
                string filename = $"tides_{year}_{month}.json";
                Console.WriteLine($"Downloading {currentDate:MMM} {year}");

                // Calculate the number of days in the current month
                int numberOfDays = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
                string dateString = currentDate.ToString("yyyy-MM-dd");

                string url = $"https://api.niwa.co.nz/tides/data?lat={lat}&long={lon}&datum=MSL&numberOfDays={numberOfDays}&apikey={actualAPIKey}&startDate={dateString}";

                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode();

                        string result = await response.Content.ReadAsStringAsync();
                        File.WriteAllText(filename, result);
                        TidesModel.TidesData tidalData = JsonConvert.DeserializeObject<TidesModel.TidesData>(result);

                        //Print MetaData
                        Console.WriteLine($"Latitude: {tidalData.Metadata.Latitude}");
                        Console.WriteLine($"Longitude: {tidalData.Metadata.Longitude}");
                        Console.WriteLine($"Datum: {tidalData.Metadata.Datum}");
                        Console.WriteLine($"Start Date: {tidalData.Metadata.Start}");
                        Console.WriteLine($"Number of Days: {tidalData.Metadata.Days}");
                        Console.WriteLine($"Interval: {tidalData.Metadata.Interval}");
                        Console.WriteLine($"Height: {tidalData.Metadata.Height}");

                        // Print tide values
                        Console.WriteLine("\nTide Values:");
                        foreach (var value in tidalData.Values)
                        {
                            Console.WriteLine($"Time: {value.Time}, Value: {value.Value}");
                        }

                        // Write the JSON response to a file
                        File.WriteAllText(filename, result);
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine($"An error occurred while downloading tides data: {ex.Message}");
                    }
                }

                currentDate = currentDate.AddMonths(1);
            }

            Console.WriteLine("Done");
        }

        static async Task Main(string[] args)
        {
            try
            {
                //Create a Console Navigation Menu to allocate Unser Input to run specified code allowing to view information separately
                Console.WriteLine("1 for solar/2 for Weather/3 for uv/4 for tides/5 for Hunting Information");
                string userInput = Console.ReadLine();
                if (userInput == "1")
                {
                    //Allocate User Input to run Weather Application Method
                    await Program.SolarApplication();
                }
                if (userInput == "2") { 

                    //Allocate User Input to run Solar Application Method
                    await Program.WeatherApplicataion();
                }
                if (userInput == "3")
                {
                    //Allocate User Input to run UV Application Method
                    await Program.UVApplication();
                }
                if (userInput == "4")
                {
                    //Allocate User Input to run Tides Application Method
                    await Program.TidesApplication();
                }
                if(userInput == "5")
                {
                    //Allocate User Input to run Hunting Application Method
                     Program.HuntingApplication();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }
    }


}

