using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WeatherApplication.UVModel;

namespace WeatherApplication

    //Created by Angus
{
    public class UVView
    {
        public void Render(UVModel.UVData uvData)
        {
            if (null == uvData)
            {
                Console.WriteLine("UV data is null.");
            }
            else
            {
                Console.WriteLine("UV Data:");
                Console.WriteLine("--------------------");
                foreach (var UV in uvData.Products)
                {
                    Console.WriteLine($"Name: {UV.Name}");
                    foreach (var val in UV.Values)
                    {
                        Console.WriteLine(val.Time.ToString());
                        Console.WriteLine(val.Value.ToString());
                    }
                }




            }
        }
        
    }
}
