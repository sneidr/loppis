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

        private bool sellerIdFocused;

        public bool SellerIdFocused
        {
            get => sellerIdFocused;
            set => SetProperty(ref sellerIdFocused, value);
        }

        private bool priceFocused;

        public bool PriceFocused
        {
            get => priceFocused;
            set => SetProperty(ref priceFocused, value);
        }


        public SalesViewModel()
        {
            CurrentEntry = new SaleEntry();
            ItemList = new ObservableCollection<SaleEntry>();

            EnterSaleCommand = new DelegateCommand(ExecuteEntry, CanExecuteEntry);
            MoveFocusCommand = new DelegateCommand(ExecuteMove, CanExecuteMove);
            RoundUpCommand = new DelegateCommand(ExecuteRoundUp, CanExecuteRoundUp);
            BagCommand = new DelegateCommand(ExecuteBag, CanExecuteBag);
            CardCommand = new DelegateCommand(ExecuteCard, CanExecuteCard);
            SellerIdFocused = true;
        }

        private bool CanExecuteCard()
        {
            return CanExecuteRoundUp();
        }

        private void ExecuteCard()
        {
            CurrentEntry.SellerId = 150;
            CurrentEntry.Price = 15;
            ExecuteMove();
        }

        private bool CanExecuteBag()
        {
            return CanExecuteRoundUp();
        }

        private void ExecuteBag()
        {
            CurrentEntry.SellerId = 100;
            CurrentEntry.Price = 5;
            ExecuteMove();
        }

        private bool CanExecuteRoundUp()
        {
            return labelTotal != 0 && CurrentEntry.Price == null && CurrentEntry.SellerId == null;
        }

        private void ExecuteRoundUp()
        {
            var rest = labelTotal % 50;
            if (rest != 0)
            {
                CurrentEntry.SellerId = 200;
                CurrentEntry.Price = (50 - rest);
            }
            ExecuteMove();
        }

        private bool CanExecuteMove()
        {
            return CurrentEntry.SellerId != null && CurrentEntry.SellerId > 0;
        }

        private void ExecuteMove()
        {
            PriceFocused = true;
            SellerIdFocused = false;
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
            SellerIdFocused = true;
            PriceFocused = false;
        }

        public ICommand EnterSaleCommand { get; set; }
        public ICommand MoveFocusCommand { get; set; }
        public ICommand RoundUpCommand { get; set; }
        public ICommand BagCommand { get; set; }
        public ICommand CardCommand { get; set; }
    }
}
