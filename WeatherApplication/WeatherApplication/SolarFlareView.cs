using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApplication
{
    internal class SolarFlareView
    {
        public void Render(SolarFlareDataArr solarFlareData)
        {
            if (null == solarFlareData)
            {
                Console.WriteLine("Solar flare data is null");
            }
            else
            {
                foreach (SolarFlareData flare in solarFlareData.Flares)
                {
                    Console.WriteLine($"Solar flare lasting from {flare.BeginTime} to {flare.EndTime}");
                    Console.WriteLine($"Flare ID: {flare.FlareID}");
                }
                
            }
        }
    }
}
