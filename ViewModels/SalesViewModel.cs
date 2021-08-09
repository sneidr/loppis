using loppis.Model;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace loppis.ViewModels
{
    public class SalesViewModel : BindableBase
    {
        private SaleEntry currentEntry;
        private int labelTotal1;
        private ObservableCollection<SaleEntry> itemList;
        private bool m_idSelected;



        public SalesViewModel()
        {
            CurrentEntry = new SaleEntry();
            ItemList = new ObservableCollection<SaleEntry>();

            EnterSaleCommand = new DelegateCommand(ExecuteEntry, CanExecuteEntry);
        }

        private bool CanExecuteEntry()
        {
            return CurrentEntry.Price != null && CurrentEntry.Price > 0 && CurrentEntry.SellerId != null && CurrentEntry.SellerId > 0;
        }

        private void ExecuteEntry()
        {
            EnterSale();
        }

        public SaleEntry CurrentEntry { get => currentEntry; set => SetProperty(ref currentEntry, value); }
        public int labelTotal { get => labelTotal1; set => SetProperty(ref labelTotal1, value); }
        public ObservableCollection<SaleEntry> ItemList { get => itemList; set => SetProperty(ref itemList, value); }
        public bool IdSelected { get => m_idSelected; set => SetProperty(ref m_idSelected, value); }

        public void EnterSale()
        {
            ItemList.Add(new SaleEntry(CurrentEntry.SellerId, CurrentEntry.Price));
            var total = ItemList.Sum(i => i.Price);
            labelTotal = total != null ? (int)total : 0;
            CurrentEntry.Clear();
        }

        public ICommand EnterSaleCommand { get; set; }
    }
}
