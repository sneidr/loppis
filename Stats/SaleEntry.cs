using System;
using System.Collections.Generic;
using System.IO;

namespace stats
{
    public class SaleEntry
    {
        public int SellerId;
        public int Price;
    }

    public class Sale
    {
        public string Cashier;
        public DateTime Timestamp;
        public List<SaleEntry> Entries;

        public void Print(StreamWriter sw)
        {
            foreach(SaleEntry e in Entries)
            {
                sw.WriteLine($"{Cashier};{Timestamp};{e.SellerId};{e.Price}");
            }
        }
    }
}
