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
        #region Constants

        private const string cDefaultSaveFileName = @".\transactions.xml";

        #endregion

        #region Properties

        private SaleEntry currentEntry;
        private int sumTotal;
        private ObservableCollection<SaleEntry> itemList;
        private bool m_idSelected;
        private bool sellerIdFocused;
        private bool priceFocused;

        public bool SellerIdFocused { get => sellerIdFocused; set => SetProperty(ref sellerIdFocused, value); }
        public bool PriceFocused { get => priceFocused; set => SetProperty(ref priceFocused, value); }
        public string SaveFileName { get; set; }
        public SaleEntry CurrentEntry { get => currentEntry; set => SetProperty(ref currentEntry, value); }
        public int SumTotal { get => sumTotal; set => SetProperty(ref sumTotal, value); }
        public ObservableCollection<SaleEntry> ItemList { get => itemList; set => SetProperty(ref itemList, value); }
        public bool IdSelected { get => m_idSelected; set => SetProperty(ref m_idSelected, value); }

        #endregion

        #region Construction

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

        #endregion

        #region Commands

        public ICommand SaveToFileCommand { get; set; }
        public ICommand CardCommand { get; set; }
        public ICommand BagCommand { get; set; }
        public ICommand RoundUpCommand { get; set; }
        public ICommand MoveFocusCommand { get; set; }
        public ICommand EnterSaleCommand { get; set; }


        #region SaveToFile Command


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

        #endregion

        #region Card Command

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

        #endregion

        #region Bag Command

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

        #endregion

        #region RoundUp Command

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

        #endregion

        #region Move Command

        private bool CanExecuteMove()
        {
            return CurrentEntry.SellerId != null && CurrentEntry.SellerId > 0;
        }

        private void ExecuteMove()
        {
            PriceFocused = true;
            SellerIdFocused = false;
        }

        #endregion

        #region EnterSale Command

        private bool CanExecuteEntry()
        {

            return CurrentEntry.Price != null && CurrentEntry.Price > 0 && CurrentEntry.SellerId != null && CurrentEntry.SellerId > 0;
        }

        private void ExecuteEntry()
        {
            EnterSale();
        }

        public void EnterSale()
        {
            ItemList.Insert(0, new SaleEntry(CurrentEntry.SellerId, CurrentEntry.Price));
            var total = ItemList.Sum(i => i.Price);
            SumTotal = total != null ? (int)total : 0;
            CurrentEntry.Clear();
            SellerIdFocused = true;
            PriceFocused = false;
        }


        #endregion

        #endregion
    }
}
