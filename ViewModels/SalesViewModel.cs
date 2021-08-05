using System;

using System.Collections.Generic;
using System.Linq;

namespace loppis.ViewModels
{
    public class SalesViewModel
    {
        public SalesViewModel()
        {
            ItemList = new List<Tuple<int, int>>();
        }

        public int SellerId { get; set; }
        public int Price { get; set; }
        public int Sum { get; set; }
        public List<Tuple<int, int>> ItemList { get; set; }

        public void EnterSale()
        {
            ItemList.Add(new Tuple<int, int>(SellerId, Price));
            Sum = ItemList.Sum(i => i.Item2);
        }
    }
}
