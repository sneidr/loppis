using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using SaveList = System.Collections.Generic.List<System.Collections.ObjectModel.ObservableCollection<payback.SaleEntry>>;

namespace payback
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Input file path: ");
            string filePath = Console.ReadLine();

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File does not exist {}", filePath);
                return;
            }

            // Read file from disk
            string contents = File.ReadAllText(filePath);

            // Deserialize from xml
            SaveList sales = ReadFromXmlFile(filePath);
            // Calculate all sums
            Dictionary<int, int> sellersAndTotals = new();
            foreach (var arr in sales)
            {
                foreach (var entry in arr)
                {
                    int key = entry.SellerId;
                    int price = entry.Price;
                    if (!sellersAndTotals.ContainsKey(key))
                    {
                        sellersAndTotals.Add(key, price);
                    }
                    else
                    {
                        sellersAndTotals[key] += price;
                    }
                }
            }
            // Print all sums
            foreach (var seller in sellersAndTotals)
            {
                Console.WriteLine($"Säljare {seller.Key}: {seller.Value} kr.");
            }
        }

        private static SaveList ReadFromXmlFile(string filePath)
        {
            var entries = new SaveList();
            using (var filestream = new FileStream(filePath, FileMode.Open))
            {
                if (filestream.Length > 0)
                {
                    var xmlreader = new XmlSerializer(typeof(SaveList));
                    try
                    {
                        entries = (SaveList)xmlreader.Deserialize(filestream);
                    }
                    catch (System.InvalidOperationException)
                    {
                        Console.WriteLine("Failed to deserialize.");
                    }
                }
            }

            return entries;
        }

    }

}
