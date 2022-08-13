using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using SaveList = System.Collections.Generic.List<stats.Sale>;

namespace stats
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Input path: ");
            string dirPath = Console.ReadLine();

            if (!Directory.Exists(dirPath))
            {
                Console.WriteLine("Path does not exist {}", dirPath);
                return;
            }


            StreamWriter sw = new StreamWriter("alltransactions.csv");
            foreach (var file in Directory.EnumerateFiles(dirPath))
            {

                // Deserialize from xml
                SaveList sales = ReadFromXmlFile(file);
                foreach(var sale in sales)
                {
                    sale.Print(sw);
                }
            }


            sw.Close();
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
