using loppis.Model;
using System.Collections.ObjectModel;
using System.Linq;

namespace loppis.ViewModels
{
    public class SalesViewModel
    {
        public SalesViewModel()
        {
            CurrentEntry = new SaleEntry();
            ItemList = new ObservableCollection<SaleEntry>();
        }

        public SaleEntry CurrentEntry { get; set; }
        public int labelTotal { get; set; }
        public ObservableCollection<SaleEntry> ItemList { get; set; }

        public void EnterSale()
        {
            ItemList.Add(new SaleEntry(CurrentEntry.SellerId, CurrentEntry.Price));
            labelTotal = ItemList.Sum(i => i.Price);
            CurrentEntry.Clear();
        }
    }
}
