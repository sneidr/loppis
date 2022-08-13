using loppis.Model;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using SaveList = System.Collections.Generic.List<loppis.Model.Sale>;

namespace loppis.ViewModels;

public delegate void ShutDownDelegate();
public delegate MessageBoxResult ShowMessageBoxDelegate(string message, string caption);

public class SalesViewModel : BindableBase
{
    #region Constants

    private const string cDefaultSaveFileName = @".\transactions.xml";
    private const string cSellerFileName = @".\sellers.csv";

    #endregion

    #region Properties

    private SaleEntry currentEntry;
    private int sumTotal;
    private ObservableCollection<SaleEntry> itemList;
    private bool m_idSelected;
    private bool sellerIdFocused;
    private bool priceFocused;
    private Brush sellerIdBackground;
    private Brush cashierBackground;
    private string cashier;

    public bool SellerIdFocused
    {
        get => sellerIdFocused;

        set
        {
            // Toggle focus to make sure that it is set
            SetProperty(ref sellerIdFocused, !value);
            SetProperty(ref sellerIdFocused, value);
        }
    }

    public bool PriceFocused
    {
        get => priceFocused;
        set
        {
            // Toggle focus to make sure that it is set
            SetProperty(ref priceFocused, !value);
            SetProperty(ref priceFocused, value);
        }
    }
    public string SaveFileName { get; set; }
    public SaleEntry CurrentEntry { get => currentEntry; set => SetProperty(ref currentEntry, value); }
    public int SumTotal { get => sumTotal; set => SetProperty(ref sumTotal, value); }
    public ObservableCollection<SaleEntry> ItemList { get => itemList; set => SetProperty(ref itemList, value); }
    public ObservableCollection<Sale> LastSalesList { get; set; }

    public bool IdSelected { get => m_idSelected; set => SetProperty(ref m_idSelected, value); }
    public ShutDownDelegate ShutDownFunction { get; set; }
    public ShowMessageBoxDelegate MsgBoxFunction { get; set; }
    public Brush SellerIdBackground { get => sellerIdBackground; set => SetProperty(ref sellerIdBackground, value); }
    public Brush CashierBackground { get => cashierBackground; set => SetProperty(ref cashierBackground, value); }
    public string Cashier { get => cashier; set => SetProperty(ref cashier, value); }

    public const int RoundUpId = 999;
    private const int NumberOfSalesToShow = 3;

    public static int? CardId { get; set; }
    public static int? BagId { get; set; }
    #endregion

    #region Construction

    public SalesViewModel(string testFileName = cDefaultSaveFileName)
    {
        SaveFileName = testFileName;
        CurrentEntry = new SaleEntry();
        ItemList = new ObservableCollection<SaleEntry>();
        LastSalesList = new();
        EnterSaleCommand = new DelegateCommand(ExecuteEntry, CanExecuteEntry);
        MoveFocusCommand = new DelegateCommand(ExecuteMove, CanExecuteMove);
        RoundUpCommand = new DelegateCommand(ExecuteRoundUp, CanExecuteRoundUp);
        BagCommand = new DelegateCommand(ExecuteBag, CanExecuteBag);
        CardCommand = new DelegateCommand(ExecuteCard, CanExecuteCard);
        SaveToFileCommand = new DelegateCommand(ExecuteSaveToFile, CanExecuteSaveToFile);
        LoadCommand = new DelegateCommand(ExecuteLoad, CanExecuteLoad);
        UndoCommand = new DelegateCommand<object>(ExecuteUndo, CanExecuteUndo);
        ClearCommand = new DelegateCommand(ExecuteClear, CanExecuteClear);
        EditPreviousSaleCommand = new DelegateCommand<object>(EditPreviousSale, CanEditPreviousSale);
        SellerList = new Dictionary<int, Seller>();
        ShutDownFunction = Application.Current != null ? Application.Current.Shutdown : null;
        MsgBoxFunction = MessageBox.Show;
        SellerIdBackground = new SolidColorBrush(Colors.White);
        CashierBackground = new SolidColorBrush(Colors.White);
        SellerIdFocused = true;
        CardId = null;
        BagId = null;
        Cashier = "Säljare";
    }

    #endregion

    #region Commands

    public ICommand SaveToFileCommand { get; set; }
    public ICommand CardCommand { get; set; }
    public ICommand BagCommand { get; set; }
    public ICommand RoundUpCommand { get; set; }
    public ICommand MoveFocusCommand { get; set; }
    public ICommand EnterSaleCommand { get; set; }
    public ICommand LoadCommand { get; set; }
    public ICommand UndoCommand { get; set; }
    public ICommand ClearCommand { get; set; }
    public ICommand EditPreviousSaleCommand { get; set; }
    public Dictionary<int, Seller> SellerList { get; set; }

    #region SaveToFile Command


    private bool CanExecuteSaveToFile()
    {
        bool validCashierName = Cashier != "Säljare" && Cashier.Length > 0;
        if (!validCashierName && ((SolidColorBrush)CashierBackground).Color == Colors.White)
        {
            CashierBackground = new SolidColorBrush(Colors.Orange);
        }
        else if (validCashierName && ((SolidColorBrush)CashierBackground).Color == Colors.Orange)
        {
            CashierBackground = new SolidColorBrush(Colors.White);
        }

        return SumTotal > 0 && ItemList.Count > 0 && validCashierName;
    }

    private void ExecuteSaveToFile()
    {
        SaveList entries = ReadFromXmlFile();
        var sale = new Sale(ItemList, Cashier);
        entries.Add(sale);
        LastSalesList.Insert(0, sale);
        if (LastSalesList.Count > NumberOfSalesToShow)
        {
            LastSalesList.RemoveAt(NumberOfSalesToShow);
        }
        WriteToXmlFile(entries);
        ItemList.Clear();
        SumTotal = 0;
    }

    private void WriteToXmlFile(SaveList entries)
    {
        using var filestream = new FileStream(SaveFileName, FileMode.Truncate);
        var xmlwriter = new XmlSerializer(typeof(SaveList));
        xmlwriter.Serialize(filestream, (SaveList)entries);
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
        // Default values
        CurrentEntry.SellerId = 600;
        CurrentEntry.Price = 15;

        foreach (var seller in SellerList)
        {
            if (seller.Value.Name == "Vykort")
            {
                CurrentEntry.SellerId = seller.Key;
                CurrentEntry.Price = seller.Value.DefaultPrice;
            }
        }

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
        // Default values
        CurrentEntry.SellerId = 500;
        CurrentEntry.Price = 5;

        foreach (var seller in SellerList)
        {
            if (seller.Value.Name == "Kasse")
            {
                CurrentEntry.SellerId = seller.Key;
                CurrentEntry.Price = seller.Value.DefaultPrice;
            }
        }
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
        CurrentEntry.SellerId = 999;
        CurrentEntry.Price = (50 - rest);
        ExecuteMove();
    }

    #endregion

    #region Move Command

    private bool CanExecuteMove()
    {
        bool canExecute = CurrentEntry.SellerId != null && (SellerList.ContainsKey(CurrentEntry.SellerId.Value) || CurrentEntry.SellerId.Value == RoundUpId);
        if (!canExecute && ((SolidColorBrush)SellerIdBackground).Color == Colors.White)
        {
            ((SolidColorBrush)SellerIdBackground).Color = Colors.Orange;
        }
        else if (canExecute && ((SolidColorBrush)SellerIdBackground).Color == Colors.Orange)
        {
            ((SolidColorBrush)SellerIdBackground).Color = Colors.White;
        }
        return canExecute;
    }

    private void ExecuteMove()
    {
        SellerIdFocused = false;
        PriceFocused = true;
    }

    #endregion

    #region EnterSale Command

    private bool CanExecuteEntry()
    {

        bool canExecute = CurrentEntry.Price != null && CurrentEntry.Price > 0 && CurrentEntry.SellerId != null &&
            (SellerList.ContainsKey(CurrentEntry.SellerId.Value) || CurrentEntry.SellerId.Value == RoundUpId);

        if (!canExecute && ((SolidColorBrush)SellerIdBackground).Color == Colors.White)
        {
            ((SolidColorBrush)SellerIdBackground).Color = Colors.Orange;
        }
        else if (canExecute && ((SolidColorBrush)SellerIdBackground).Color == Colors.Orange)
        {
            ((SolidColorBrush)SellerIdBackground).Color = Colors.White;
        }

        return canExecute;
    }

    private void ExecuteEntry()
    {
        EnterSale();
    }

    public void EnterSale()
    {
        ItemList.Insert(0, new SaleEntry(CurrentEntry.SellerId, CurrentEntry.Price));
        UpdateSumTotal();
        CurrentEntry.Clear();
        SetFocusToSellerId();
    }

    private void UpdateSumTotal()
    {
        var total = ItemList.Sum(i => i.Price);
        SumTotal = total != null ? (int)total : 0;
    }

    private void SetFocusToSellerId()
    {
        PriceFocused = false;
        SellerIdFocused = true;
    }

    #endregion

    #region LoadCommand

    private bool CanExecuteLoad()
    {
        return true;
    }

    private void ExecuteLoad()
    {
        try
        {
            bool bagEntryInFile = false;
            bool cardEntryInFile = false;
            SellerList.Clear();
            string sellersContent = File.ReadAllText(cSellerFileName);
            foreach (string line in sellersContent.Split("\r\n"))
            {
                string[] a = line.Split(";");
                Seller seller;
                int sellerId = int.Parse(a[0]);
                if (sellerId == RoundUpId)
                {
                    throw new System.FormatException($"Id 999 is reserved. Please choose another id for row: {line}");
                }
                if (a.Length > 2)
                {
                    seller = new Seller() { Name = a[1], DefaultPrice = int.Parse(a[2]) };
                    SellerList.Add(sellerId, seller);
                }
                else if (a.Length > 1)
                {
                    seller = new Seller() { Name = a[1], DefaultPrice = null };
                    SellerList.Add(sellerId, seller);
                }
                else
                {
                    throw new System.FormatException($"The line was incorrectly formatted: {line}");
                }

                if (seller.Name == "Kasse" && seller.DefaultPrice != null)
                {
                    bagEntryInFile = true;
                    BagId = sellerId;
                }
                if (seller.Name == "Vykort" && seller.DefaultPrice != null)
                {
                    cardEntryInFile = true;
                    CardId = sellerId;

                }
            }
            if (!bagEntryInFile)
            {
                throw new System.FormatException("File must contain entry for \"Kasse\" with default price.");
            }
            if (!cardEntryInFile)
            {
                throw new System.FormatException("File must contain entry for \"Vykort\" with default price.");
            }
        }
        catch (FileNotFoundException ex)
        {
            MsgBoxFunction(ex.Message, "Error!");
            ShutDownFunction();
        }
        catch (System.FormatException ex)
        {
            MsgBoxFunction(ex.Message, "Error!");
            ShutDownFunction();
        }
        catch (System.ArgumentException ex)
        {
            MsgBoxFunction(ex.Message, "Error!");
            ShutDownFunction();
        }
    }


    #endregion

    #region UndoCommand

    private bool CanExecuteUndo(object param)
    {
        return currentEntry.SellerId == null && currentEntry.Price == null;
    }

    private void ExecuteUndo(object param)
    {
        CurrentEntry = ItemList[(int)param];
        ItemList.RemoveAt((int)param);
        UpdateSumTotal();
        SetFocusToSellerId();
    }

    #endregion

    #region ClearCommand

    private bool CanExecuteClear()
    {
        return CurrentEntry.SellerId != null || CurrentEntry.Price != null;
    }

    private void ExecuteClear()
    {
        CurrentEntry.SellerId = null;
        CurrentEntry.Price = null;
        SetFocusToSellerId();
    }

    #endregion

    #region EditPreviousSaleCommand
    private bool CanEditPreviousSale(object param)
    {
        return LastSalesList.Count > 0 && itemList.Count == 0;
    }

    private void EditPreviousSale(object param)
    {
        var sale = LastSalesList[(int)param];
        foreach (var entry in sale.Entries)
        {
            ItemList.Add(entry);
        }
        LastSalesList.Remove(sale);

        UpdateSumTotal();

        SaveList entries = ReadFromXmlFile();
        entries.Remove(sale);
        WriteToXmlFile(entries);
    }

    #endregion

    #endregion
}
