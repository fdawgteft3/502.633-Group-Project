using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApplication
{
    internal class UVView
    {
        public void RenderUVData(List<UVModel.UVData> UVDATA)
        {
            Console.WriteLine("UV Data:");
            Console.WriteLine("--------------------");
            foreach (var UV in UVDATA)
            {
                Console.WriteLine($"Coords: {UV.Coord}");
                Console.WriteLine($"Products: {UV.Products}");
                Console.WriteLine();
            }
        }
        public void RenderProduct(List<UVModel.UVProduct> UVProduct)
        {
            Console.WriteLine("UV Product Data:");
            Console.WriteLine("--------------------");
            foreach (var UV in UVProduct)
            {
                Console.WriteLine($"Name: {UV.Name}");
                Console.WriteLine($"Values: {UV.Values}");
                Console.WriteLine();
            }
        }

        public void RenderUVDataEntry(List<UVModel.UVDataEntry> UVDataEntry)
        {
            Console.WriteLine("UV Entry Data:");
            Console.WriteLine("--------------------");
            foreach (var UV in UVDataEntry)
            {
                Console.WriteLine($"Time: {UV.Time}");
                Console.WriteLine($"Value: {UV.Value}");
                Console.WriteLine();
            }
        }
    }
}
