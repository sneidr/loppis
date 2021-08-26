using loppis.Model;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml.Serialization;
using SaveList = System.Collections.Generic.List<System.Collections.ObjectModel.ObservableCollection<loppis.Model.SaleEntry>>;

namespace loppis.ViewModels
{
    public class SalesViewModel : BindableBase
    {
        private const string cDefaultSaveFileName = @".\myfile.xml";
        private SaleEntry currentEntry;
        private int sumTotal;
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

        public string SaveFileName { get; set; }

        public SalesViewModel(string testFileName = cDefaultSaveFileName)
        {
            SaveFileName = testFileName;
            CurrentEntry = new SaleEntry();
            ItemList = new ObservableCollection<SaleEntry>();

            EnterSaleCommand = new DelegateCommand(ExecuteEntry, CanExecuteEntry);
            MoveFocusCommand = new DelegateCommand(ExecuteMove, CanExecuteMove);
            RoundUpCommand = new DelegateCommand(ExecuteRoundUp, CanExecuteRoundUp);
            BagCommand = new DelegateCommand(ExecuteBag, CanExecuteBag);
            CardCommand = new DelegateCommand(ExecuteCard, CanExecuteCard);
            SaveToFileCommand = new DelegateCommand(ExecuteSaveToFile, CanExecuteSaveToFile);
            SellerIdFocused = true;
        }
        private bool CanExecuteSaveToFile()
        {
            return SumTotal > 0 && ItemList.Count > 0;
        }

        private void ExecuteSaveToFile()
        {
            SaveList entries = ReadFromXmlFile();
            entries.Add(ItemList);
            WriteToXmlFile(entries);
            ItemList.Clear();
            SumTotal = 0;
        }

        private void WriteToXmlFile(SaveList entries)
        {
            using (var filestream = new FileStream(SaveFileName, FileMode.Truncate))
            {
                var xmlwriter = new XmlSerializer(typeof(SaveList));
                xmlwriter.Serialize(filestream, (SaveList)entries);
            }
        }

        private SaveList ReadFromXmlFile()
        {
            var entries = new SaveList();
            using (var filestream = new FileStream(SaveFileName, FileMode.OpenOrCreate))
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
                        //TODO: Error bar at the top
                        CopyFileToErrorBackup();
                    }
                }
            }

            return entries;
        }

        // Copies file to new error backup file
        private void CopyFileToErrorBackup()
        {
            int i = NextAvailableErrorFileNumber();
            File.Copy(SaveFileName, GetErrorFileName(i));
        }

        private int NextAvailableErrorFileNumber()
        {
            int i = 0;
            while (File.Exists(path: GetErrorFileName(++i)))
            {
                if (i > 100)
                {
                    // Defensive
                    // Should never happen
                    throw new IOException("Too many error files!");
                }
            }

            return i;
        }

        // Adds "_error<num> to cSaveFileName
        private string GetErrorFileName(int i)
        {
            string dir = Path.GetDirectoryName(SaveFileName);
            string fileName = Path.GetFileNameWithoutExtension(SaveFileName);
            string ext = Path.GetExtension(SaveFileName);

            return Path.Combine(dir, $"{fileName}_error{i}{ext}");
        }

        private bool CanExecuteCard()
        {
            return true;
        }

        private void ExecuteCard()
        {
            CurrentEntry.SellerId = 150;
            CurrentEntry.Price = 15;
            ExecuteMove();
        }

        private bool CanExecuteBag()
        {
            return true;
        }

        private void ExecuteBag()
        {
            CurrentEntry.SellerId = 100;
            CurrentEntry.Price = 5;
            ExecuteMove();
        }

        private bool CanExecuteRoundUp()
        {
            return (SumTotal % 50) != 0;
        }

        private void ExecuteRoundUp()
        {
            var rest = SumTotal % 50;
            CurrentEntry.SellerId = 200;
            CurrentEntry.Price = (50 - rest);
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
        public int SumTotal { get => sumTotal; set => SetProperty(ref sumTotal, value); }
        public ObservableCollection<SaleEntry> ItemList { get => itemList; set => SetProperty(ref itemList, value); }
        public bool IdSelected { get => m_idSelected; set => SetProperty(ref m_idSelected, value); }

        public void EnterSale()
        {
            ItemList.Insert(0, new SaleEntry(CurrentEntry.SellerId, CurrentEntry.Price));
            var total = ItemList.Sum(i => i.Price);
            SumTotal = total != null ? (int)total : 0;
            CurrentEntry.Clear();
            SellerIdFocused = true;
            PriceFocused = false;
        }

        public ICommand EnterSaleCommand { get; set; }
        public ICommand MoveFocusCommand { get; set; }
        public ICommand RoundUpCommand { get; set; }
        public ICommand BagCommand { get; set; }
        public ICommand CardCommand { get; set; }
        public ICommand SaveToFileCommand { get; set; }
    }
}
