using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApplication

    //Created by Angus
{
    internal class UVView
    {
        //Get Data for Coord and Product
        public void RenderUVData(List<UVModel.UVData> UVDATA)
        {
            Console.WriteLine("UV Data:");
            Console.WriteLine("--------------------");
            foreach (var UV in UVDATA)
            {
                Console.WriteLine($"Coords: {UV.coordinate}");
                Console.WriteLine($"Products: {UV.Products}");
                Console.WriteLine();
            }
        }
        //Get Data for Name and Products
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

        //Get Data for Time and Value
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
