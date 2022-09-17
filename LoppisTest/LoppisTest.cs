using loppis.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Xml.Serialization;
using SaveList = System.Collections.Generic.List<loppis.Model.Sale>;

namespace LoppisTest;

internal class TestFiles
{
    private const string testFileName = @".\mytestfile.xml";
    private const string sellerFileName = @".\sellers.csv";

    public static void RemoveConfigFile()
    {
        if (File.Exists(sellerFileName))
        {
            File.Delete(sellerFileName);
        }
    }

    public static void SetupConfigFile(string contents)
    {
        RemoveConfigFile();

        File.Create(sellerFileName).Close();
        File.WriteAllText(sellerFileName, contents);
    }
}

[TestClass]
public class LoppisTest
{
    private const string testFileName = @".\mytestfile.xml";

    #region EnterSale Tests
    [TestMethod]
    public void TestCanEnterSale_SellerIdInvalid()
    {
        TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Hej Svej\r\n8;Kasse;1\r\n9;Vykort;2");

        SalesViewModel vm = new();
        Assert.AreEqual(Colors.White, ((SolidColorBrush)vm.SellerIdBackground).Color);
        Assert.IsTrue(vm.LoadCommand.CanExecute(null));
        vm.LoadCommand.Execute(null);

        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 12;
        Assert.IsFalse(vm.EnterSaleCommand.CanExecute(null));
        Assert.AreEqual(Colors.Orange, ((SolidColorBrush)vm.SellerIdBackground).Color);
        vm.CurrentEntry.SellerId = 7;
        Assert.IsTrue(vm.EnterSaleCommand.CanExecute(null));
        Assert.AreEqual(Colors.White, ((SolidColorBrush)vm.SellerIdBackground).Color);
    }

    [TestMethod]
    public void TestEnterOneSale_SumIsEqualToPrice()
    {
        SalesViewModel vm = new();
        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 12;
        vm.EnterSale();

        Assert.AreEqual(vm.SumTotal, 62);
    }

    [TestMethod]
    public void TestEnterOneSale_ListHasOneEntry()
    {
        SalesViewModel vm = new();
        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 12;
        vm.EnterSale();

        Assert.AreEqual(vm.ItemList.Count, 1);
    }

    [TestMethod]
    public void TestEnterOneSale_ListEntryCorrect()
    {
        SalesViewModel vm = new();
        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 12;
        vm.EnterSale();

        Assert.AreEqual(vm.ItemList[0].SellerId, 12);
        Assert.AreEqual(vm.ItemList[0].SellerIdListText, "Säljare:  12");
        Assert.AreEqual(vm.ItemList[0].Price, 62);
    }

    [TestMethod]
    public void TestEnterMultipleSales_ListHasMultipleEntries()
    {
        SalesViewModel vm = new();
        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 12;
        vm.EnterSale();
        vm.CurrentEntry.Price = 55;
        vm.CurrentEntry.SellerId = 15;
        vm.EnterSale();

        Assert.AreEqual(vm.SumTotal, 62 + 55);
        Assert.AreEqual(vm.ItemList[0].Price, 55);
        Assert.AreEqual(vm.ItemList[1].Price, 62);
    }

    [TestMethod]
    public void TestEnterOneSale_VMIsClearedAfterEntry()
    {
        SalesViewModel vm = new();
        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 12;
        vm.EnterSale();

        Assert.IsNull(vm.CurrentEntry.Price);
        Assert.IsNull(vm.CurrentEntry.SellerId);
    }

    [TestMethod]
    public void TestEnterOneSale_RoundUpSellerIdIsOk()
    {
        SalesViewModel vm = new();
        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 999;
        Assert.IsTrue(vm.EnterSaleCommand.CanExecute(null));
    }
    #endregion

    #region Move Command Tests

    [TestMethod]
    public void TestMove_InvalidSellerId()
    {
        TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Hej Svej\r\n8;Kasse;1\r\n9;Vykort;2");

        SalesViewModel vm = new();
        Assert.AreEqual(Colors.White, ((SolidColorBrush)vm.SellerIdBackground).Color);

        Assert.IsTrue(vm.LoadCommand.CanExecute(null));
        vm.LoadCommand.Execute(null);

        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 12;
        Assert.IsFalse(vm.MoveFocusCommand.CanExecute(null));
        Assert.AreEqual(Colors.Orange, ((SolidColorBrush)vm.SellerIdBackground).Color);

        vm.CurrentEntry.SellerId = 7;
        Assert.IsTrue(vm.MoveFocusCommand.CanExecute(null));
        Assert.AreEqual(Colors.White, ((SolidColorBrush)vm.SellerIdBackground).Color);

        vm.CurrentEntry.SellerId = 999;
        Assert.IsTrue(vm.MoveFocusCommand.CanExecute(null));
    }

    #endregion

    #region Bag Command Tests
    [TestMethod]
    public void TestBagCommand_CanExecute()
    {
        SalesViewModel vm = new();
        Assert.IsTrue(vm.BagCommand.CanExecute(null));

        vm.CurrentEntry.Price = 12;
        Assert.IsTrue(vm.BagCommand.CanExecute(null));

        vm.CurrentEntry.Clear();
        vm.CurrentEntry.SellerId = 43;
        Assert.IsTrue(vm.BagCommand.CanExecute(null));

        vm.CurrentEntry.Clear();
        Assert.IsTrue(vm.BagCommand.CanExecute(null));
    }

    [TestMethod]
    public void TestBagCommand_Execute()
    {
        {
            SalesViewModel vm = new();
            vm.SellerList.Add(92, new Seller() { Name = "Kasse", DefaultPrice = 7 });
            vm.BagCommand.Execute(null);

            Assert.AreEqual(vm.CurrentEntry.SellerId, 92);
            Assert.AreEqual(vm.CurrentEntry.Price, 7);
            Assert.IsFalse(vm.SellerIdFocused);
            Assert.IsTrue(vm.PriceFocused);
        }
    }

    #endregion

    #region Card Command Tests
    [TestMethod]
    public void TestCardCommand_CanExecute()
    {
        SalesViewModel vm = new();
        Assert.IsTrue(vm.CardCommand.CanExecute(null));

        vm.CurrentEntry.Price = 12;
        Assert.IsTrue(vm.CardCommand.CanExecute(null));

        vm.CurrentEntry.Clear();
        vm.CurrentEntry.SellerId = 43;
        Assert.IsTrue(vm.CardCommand.CanExecute(null));

        vm.CurrentEntry.Clear();
        Assert.IsTrue(vm.CardCommand.CanExecute(null));
    }

    [TestMethod]
    public void TestCardCommand_Execute()
    {
        SalesViewModel vm = new();
        vm.SellerList.Add(200, new Seller() { Name = "Vykort", DefaultPrice = 27 });
        vm.CardCommand.Execute(null);
        Assert.AreEqual(vm.CurrentEntry.SellerId, 200);
        Assert.AreEqual(vm.CurrentEntry.Price, 27);
        Assert.IsFalse(vm.SellerIdFocused);
        Assert.IsTrue(vm.PriceFocused);
    }

    [TestMethod]
    public void TestCardCommand_TryTwice_Focus()
    {
        SalesViewModel vm = new();
        if (vm.CardCommand.CanExecute(null))
        {
            vm.CardCommand.Execute(null);
        }
        else
        {
            Assert.Fail();
        }
        Assert.IsTrue(vm.CardCommand.CanExecute(null));

        Assert.IsTrue(vm.PriceFocused);
        Assert.IsFalse(vm.SellerIdFocused);
    }

    #endregion

    #region RoundUp Command Tests
    [TestMethod]
    public void TestRoundUpCommand_CanExecute()
    {
        SalesViewModel vm = new();
        Assert.IsFalse(vm.RoundUpCommand.CanExecute(null));

        vm.CurrentEntry.Price = 12;
        Assert.IsFalse(vm.RoundUpCommand.CanExecute(null));

        vm.CurrentEntry.Clear();
        vm.CurrentEntry.SellerId = 43;
        Assert.IsFalse(vm.RoundUpCommand.CanExecute(null));

        vm.SumTotal = 25;
        Assert.IsTrue(vm.RoundUpCommand.CanExecute(null));

        vm.CurrentEntry.Price = 12;
        Assert.IsTrue(vm.RoundUpCommand.CanExecute(null));

        vm.CurrentEntry.Clear();
        vm.CurrentEntry.SellerId = 43;
        Assert.IsTrue(vm.RoundUpCommand.CanExecute(null));

        vm.CurrentEntry.Clear();
        Assert.IsTrue(vm.RoundUpCommand.CanExecute(null));

        vm.SumTotal = 50;
        Assert.IsFalse(vm.RoundUpCommand.CanExecute(null));
    }

    [TestMethod]
    public void TestRoundUpCommand_Execute()
    {
        SalesViewModel vm = new();
        vm.CurrentEntry.SellerId = 12;
        vm.CurrentEntry.Price = 43;
        vm.EnterSaleCommand.Execute(null);
        vm.RoundUpCommand.Execute(null);

        Assert.AreEqual(vm.SumTotal, 43);
        Assert.AreEqual(vm.CurrentEntry.Price, 50 - 43);
        Assert.AreEqual(vm.CurrentEntry.SellerId, 999);
        Assert.IsFalse(vm.SellerIdFocused);
        Assert.IsTrue(vm.PriceFocused);
    }

    [TestMethod]
    public void TestRoundUpCommand_Execute_DivisibleBy50()
    {
        SalesViewModel vm = new();
        vm.CurrentEntry.SellerId = 12;
        vm.CurrentEntry.Price = 50;
        vm.EnterSaleCommand.Execute(null);
        if (vm.RoundUpCommand.CanExecute(null))
        {
            vm.RoundUpCommand.Execute(null);
        }
        Assert.AreEqual(vm.SumTotal, 50);
        Assert.IsNull(vm.CurrentEntry.Price);
        Assert.IsNull(vm.CurrentEntry.SellerId);
        Assert.IsTrue(vm.SellerIdFocused);
        Assert.IsFalse(vm.PriceFocused);
    }

    #endregion

    #region SaveToFileCommand Tests
    [TestMethod]
    public void TestSaveToFile_CanExecute()
    {
        SalesViewModel vm = new(testFileName);
        Assert.AreEqual(Colors.White, ((SolidColorBrush)vm.CashierBackground).Color);
        Assert.IsFalse(vm.SaveToFileCommand.CanExecute(null));

        Assert.AreEqual(vm.Cashier, "Säljare");
        vm.CurrentEntry.SellerId = 12;
        vm.CurrentEntry.Price = 80;
        vm.EnterSale();
        Assert.IsFalse(vm.SaveToFileCommand.CanExecute(null));
        Assert.AreEqual(Colors.Orange, ((SolidColorBrush)vm.CashierBackground).Color);

        vm.Cashier = "Simon";
        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        Assert.AreEqual(Colors.White, ((SolidColorBrush)vm.CashierBackground).Color);

        vm.Cashier = "";
        Assert.IsFalse(vm.SaveToFileCommand.CanExecute(null));
        Assert.AreEqual(Colors.Orange, ((SolidColorBrush)vm.CashierBackground).Color);
    }

    [TestMethod]
    public void TestSaveToFile_Execute()
    {
        if (File.Exists(testFileName))
        {
            File.Delete(testFileName);
        }

        SalesViewModel vm = new(testFileName)
        {
            Cashier = "Lisa"
        };

        vm.CurrentEntry.SellerId = 12;
        vm.CurrentEntry.Price = 80;
        vm.EnterSale();
        vm.CurrentEntry.SellerId = 15;
        vm.CurrentEntry.Price = 90;
        vm.EnterSale();
        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        vm.SaveToFileCommand.Execute(null);

        vm.CurrentEntry.SellerId = 20;
        vm.CurrentEntry.Price = 100;
        vm.EnterSale();
        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        vm.SaveToFileCommand.Execute(null);

        var entries = new SaveList();
        using (var filestream = new FileStream(testFileName, FileMode.Open))
        {
            var xmlreader = new XmlSerializer(typeof(SaveList));
            entries = (SaveList)xmlreader.Deserialize(filestream);
        }

        Assert.AreEqual(entries[0][0].SellerId, 15);
        Assert.AreEqual(entries[0][0].Price, 90);
        Assert.AreEqual(entries[0][1].SellerId, 12);
        Assert.AreEqual(entries[0][1].Price, 80);
        Assert.AreEqual(entries[1][0].SellerId, 20);
        Assert.AreEqual(entries[1][0].Price, 100);
        Assert.AreEqual(entries[0].Cashier, "Lisa");
        Assert.AreEqual(entries[1].Cashier, "Lisa");
        Assert.AreNotEqual(entries[0].Timestamp, entries[1].Timestamp);
        Assert.AreEqual(vm.SumTotal, 0);
    }

    [TestMethod]
    public void TestSaveToFile_Execute_FileExists()
    {
        if (File.Exists(testFileName))
        {
            File.Delete(testFileName);
        }
        using (var fs = new FileStream(testFileName, FileMode.Create))
        {
        }

        SalesViewModel vm = new(testFileName)
        {
            Cashier = "Simon"
        };
        vm.CurrentEntry.SellerId = 12;
        vm.CurrentEntry.Price = 80;
        vm.EnterSale();
        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        vm.SaveToFileCommand.Execute(null);

        var entries = new SaveList();
        using (var filestream = new FileStream(testFileName, FileMode.Open))
        {
            var xmlreader = new XmlSerializer(typeof(SaveList));
            entries = (SaveList)xmlreader.Deserialize(filestream);
        }

        Assert.AreEqual(entries[0][0].SellerId, 12);
        Assert.AreEqual(entries[0][0].Price, 80);
    }

    [TestMethod]
    public void TestSaveToFile_Execute_FileWrongFormat()
    {
        string testFirstErrorFileName = $"{Path.GetFileNameWithoutExtension(testFileName)}_error1{Path.GetExtension(testFileName)}";
        string testSecondErrorFileName = $"{Path.GetFileNameWithoutExtension(testFileName)}_error2{Path.GetExtension(testFileName)}";

        if (File.Exists(testFileName))
        {
            File.Delete(testFileName);
        }
        if (File.Exists(testFirstErrorFileName))
        {
            File.Delete(testFirstErrorFileName);
        }
        if (File.Exists(testSecondErrorFileName))
        {
            File.Delete(testSecondErrorFileName);
        }
        using (var streamWriter = new StreamWriter(testFileName))
        {
            streamWriter.WriteLine("ErrorText");
        }

        SalesViewModel vm = new(testFileName)
        {
            Cashier = "Simon"
        };

        vm.CurrentEntry.SellerId = 12;
        vm.CurrentEntry.Price = 80;
        vm.EnterSale();
        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        vm.SaveToFileCommand.Execute(null);

        var entries = new SaveList();
        using (var filestream = new FileStream(testFileName, FileMode.Open))
        {
            var xmlreader = new XmlSerializer(typeof(SaveList));
            entries = (SaveList)xmlreader.Deserialize(filestream);
        }

        Assert.AreEqual(entries[0][0].SellerId, 12);
        Assert.AreEqual(entries[0][0].Price, 80);

        Assert.IsTrue(File.Exists(testFirstErrorFileName));
        Assert.IsFalse(File.Exists(testSecondErrorFileName));

        File.Delete(testFileName);
        File.Copy(testFirstErrorFileName, testFileName);

        vm.CurrentEntry.SellerId = 12;
        vm.CurrentEntry.Price = 80;
        vm.EnterSale();
        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        vm.SaveToFileCommand.Execute(null);

        entries = new SaveList();
        using (var filestream = new FileStream(testFileName, FileMode.Open))
        {
            var xmlreader = new XmlSerializer(typeof(SaveList));
            entries = (SaveList)xmlreader.Deserialize(filestream);
        }

        Assert.AreEqual(entries[0][0].SellerId, 12);
        Assert.AreEqual(entries[0][0].Price, 80);

        Assert.IsTrue(File.Exists(testSecondErrorFileName));
    }

    [TestMethod]
    [Ignore]
    public void TestSaveToFile_Execute_Performance()
    {
        if (File.Exists(testFileName))
        {
            File.Delete(testFileName);
        }
        using (var fs = new FileStream(testFileName, FileMode.Create))
        {
        }

        SalesViewModel vm = new(testFileName);

        var stopwatch = new Stopwatch();
        for (int i = 1; i <= 250; i++)
        {
            if (i == 241)
            {
                stopwatch.Start();
            }
            for (int j = 1; j <= 20; j++)
            {
                vm.CurrentEntry.SellerId = i;
                vm.CurrentEntry.Price = i;
                vm.EnterSale();
            }
            Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
            vm.SaveToFileCommand.Execute(null);
        }
        stopwatch.Stop();
        var elapsed = stopwatch.ElapsedMilliseconds;

        Assert.IsTrue(elapsed / 10 < 500, $"{elapsed} ms");
    }

    [TestMethod]
    public void TestSaveToFile_Execute_LastEntriesInList()
    {
        if (File.Exists(testFileName))
        {
            File.Delete(testFileName);
        }

        SalesViewModel vm = new(testFileName)
        {
            Cashier = "Lisa"
        };

        vm.CurrentEntry.SellerId = 12;
        vm.CurrentEntry.Price = 80;
        vm.EnterSale();
        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        vm.SaveToFileCommand.Execute(null);
        Assert.AreEqual(vm.LastSalesList[0].SumTotal, 80);


        vm.CurrentEntry.SellerId = 15;
        vm.CurrentEntry.Price = 90;
        vm.EnterSale();
        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        vm.SaveToFileCommand.Execute(null);
        Assert.AreEqual(vm.LastSalesList[0].SumTotal, 90);
        Assert.AreEqual(vm.LastSalesList[1].SumTotal, 80);

        vm.CurrentEntry.SellerId = 20;
        vm.CurrentEntry.Price = 100;
        vm.EnterSale();
        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        vm.SaveToFileCommand.Execute(null);
        Assert.AreEqual(vm.LastSalesList[0].SumTotal, 100);
        Assert.AreEqual(vm.LastSalesList[1].SumTotal, 90);
        Assert.AreEqual(vm.LastSalesList[2].SumTotal, 80);

        vm.CurrentEntry.SellerId = 20;
        vm.CurrentEntry.Price = 110;
        vm.EnterSale();
        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        vm.SaveToFileCommand.Execute(null);
        Assert.AreEqual(vm.LastSalesList[0].SumTotal, 110);
        Assert.AreEqual(vm.LastSalesList[1].SumTotal, 100);
        Assert.AreEqual(vm.LastSalesList[2].SumTotal, 90);
        Assert.AreEqual(vm.LastSalesList.Count, 3);
    }
    #endregion

    #region LoadCommand Tests

    [TestMethod]
    public void TestLoadCommand()
    {
        TestFiles.RemoveConfigFile();

        {
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            SalesViewModel vm = new(testFileName)
            {
                ShutDownFunction = () => { isShutDown = true; },
                MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; }
            };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.AreEqual(0, vm.SellerList.Count);
            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        {

            TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Kasse;78\r\n8;Vykort;15");

            SalesViewModel vm = new(testFileName);
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.AreEqual(4, vm.SellerList.Count);
            Assert.AreEqual("John Doe", vm.SellerList[2].Name);
        }
        { // Error: Empty file
            TestFiles.SetupConfigFile("");

            SalesViewModel vm = new(testFileName);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        { // Error: Cannot convert to int
            TestFiles.SetupConfigFile("A;Firstname LastName\r\n2;John Doe\r\n7;Kasse");

            SalesViewModel vm = new(testFileName);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        { // Error: Duplicate ids
            TestFiles.SetupConfigFile("1;Firstname LastName\r\n1;John Doe\r\n7;Kasse");

            SalesViewModel vm = new(testFileName);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        { // Error: Missing semicolon
            TestFiles.SetupConfigFile("1 Firstname LastName\r\n1;John Doe\r\n7;Kasse");

            SalesViewModel vm = new(testFileName);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        { // Error: Missing line breaks
            TestFiles.SetupConfigFile("1 Firstname LastName 1;John Doe 7;Kasse");

            SalesViewModel vm = new(testFileName);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        { // Default price
            TestFiles.SetupConfigFile("1;Firstname LastName\r\n8;Vykort;15\r\n11;Kasse;5");

            SalesViewModel vm = new(testFileName);
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.AreEqual(3, vm.SellerList.Count);
            Assert.AreEqual(15, vm.SellerList[8].DefaultPrice);
            Assert.AreEqual(5, vm.SellerList[11].DefaultPrice);
        }
        { // Error: Default price not int
            TestFiles.SetupConfigFile("1;Firstname LastName\r\n8;Vykort;15\r\n11;Kasse;Hej");
            SalesViewModel vm = new(testFileName);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };

            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        { // Error: No bag entry in file
            TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Hej Svej\r\n8;Vykort;15");

            SalesViewModel vm = new(testFileName);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        { // Error: No default price for bag entry
            TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Kasse\r\n8;Vykort;15");

            SalesViewModel vm = new(testFileName);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        { // Error: No card entry in file
            TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Kasse;15");

            SalesViewModel vm = new(testFileName);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        { // Error: No default price for card entry
            TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Kasse;23\r\n8;Vykort");

            SalesViewModel vm = new(testFileName);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        { // Error: 999 is reserved for roundup
            TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Kasse;55\r\n8;Vykort;23\r\n999;Ajajaj");

            SalesViewModel vm = new(testFileName);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
    }

    #endregion

    #region UndoCommand Tests

    [TestMethod]
    public void TestUndoCommand()
    {
        SalesViewModel vm = new();
        vm.CurrentEntry.SellerId = 1;
        vm.CurrentEntry.Price = 3;

        Assert.IsFalse(vm.UndoCommand.CanExecute(0));
        vm.SellerList.Add(1, new Seller() { Name = "Kim Karlsson", DefaultPrice = null });
        Assert.IsTrue(vm.EnterSaleCommand.CanExecute(null));
        vm.EnterSaleCommand.Execute(null);

        Assert.IsTrue(vm.UndoCommand.CanExecute(0));
        vm.UndoCommand.Execute(0);
        Assert.AreEqual(1, vm.CurrentEntry.SellerId);
        Assert.AreEqual(3, vm.CurrentEntry.Price);
        Assert.AreEqual(0, vm.SumTotal);
        Assert.IsTrue(vm.SellerIdFocused);
        Assert.IsFalse(vm.PriceFocused);
    }

    #endregion

    #region ClearCommand Tests
    [TestMethod]
    public void TestClearCommand()
    {
        {
            SalesViewModel vm = new();
            Assert.IsFalse(vm.ClearCommand.CanExecute(null));

            vm.CurrentEntry.SellerId = 1;
            Assert.IsTrue(vm.ClearCommand.CanExecute(null));

            vm.ClearCommand.Execute(null);
            Assert.IsNull(vm.CurrentEntry.SellerId);
        }
        {
            SalesViewModel vm = new();
            Assert.IsFalse(vm.ClearCommand.CanExecute(null));

            vm.CurrentEntry.Price = 2;
            Assert.IsTrue(vm.ClearCommand.CanExecute(null));

            vm.ClearCommand.Execute(null);
            Assert.IsNull(vm.CurrentEntry.Price);
        }
        {
            SalesViewModel vm = new();
            Assert.IsFalse(vm.ClearCommand.CanExecute(null));

            vm.CurrentEntry.SellerId = 1;
            vm.CurrentEntry.Price = 2;
            Assert.IsTrue(vm.ClearCommand.CanExecute(null));

            vm.ClearCommand.Execute(null);
            Assert.IsNull(vm.CurrentEntry.SellerId);
            Assert.IsNull(vm.CurrentEntry.Price);
            Assert.IsTrue(vm.SellerIdFocused);
            Assert.IsFalse(vm.PriceFocused);
        }
    }
    #endregion

    #region EditPreviousSaleCommand

    [TestMethod]
    public void TestEditPreviousSaleCommand()
    {
        if (File.Exists(testFileName))
        {
            File.Delete(testFileName);
        }

        SalesViewModel vm = new(testFileName);
        Assert.IsFalse(vm.EditPreviousSaleCommand.CanExecute(0));

        vm.CurrentEntry.SellerId = 3;
        vm.CurrentEntry.Price = 34;

        vm.SellerList.Add(3, new Seller() { Name = "Lisa" });
        vm.SellerList.Add(5, new Seller() { Name = "Humle Dumle" });
        vm.Cashier = "Simon";

        Assert.IsTrue(vm.EnterSaleCommand.CanExecute(null));
        vm.EnterSaleCommand.Execute(null);

        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        vm.SaveToFileCommand.Execute(null);

        vm.CurrentEntry.SellerId = 5;
        vm.CurrentEntry.Price = 54;

        Assert.IsTrue(vm.EnterSaleCommand.CanExecute(null));
        vm.EnterSaleCommand.Execute(null);

        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        vm.SaveToFileCommand.Execute(null);

        Assert.AreEqual(vm.ItemList.Count, 0);

        {
            Assert.IsTrue(vm.EditPreviousSaleCommand.CanExecute(1));
            vm.EditPreviousSaleCommand.Execute(1);

            Assert.AreEqual(vm.ItemList.Count, 1);
            Assert.AreEqual(vm.ItemList[0].Price, 34);
            Assert.AreEqual(vm.ItemList[0].SellerId, 3);
            Assert.AreEqual(vm.SumTotal, 34);

            Assert.AreEqual(vm.LastSalesList.Count, 1);

            var entries = new SaveList();
            using (var filestream = new FileStream(testFileName, FileMode.Open))
            {
                var xmlreader = new XmlSerializer(typeof(SaveList));
                entries = (SaveList)xmlreader.Deserialize(filestream);
            }
            Assert.AreEqual(entries.Count, 1);
            Assert.AreEqual(entries[0].Entries[0].SellerId, 5);
            Assert.AreEqual(entries[0].Entries[0].Price, 54);
        }
        {
            Assert.IsFalse(vm.EditPreviousSaleCommand.CanExecute(0));
            vm.ItemList.Clear();

            Assert.IsTrue(vm.EditPreviousSaleCommand.CanExecute(0));
            vm.EditPreviousSaleCommand.Execute(0);

            Assert.AreEqual(vm.ItemList.Count, 1);
            Assert.AreEqual(vm.ItemList[0].Price, 54);
            Assert.AreEqual(vm.ItemList[0].SellerId, 5);

            Assert.AreEqual(vm.LastSalesList.Count, 0);

            var entries = new SaveList();
            using (var filestream = new FileStream(testFileName, FileMode.Open))
            {
                var xmlreader = new XmlSerializer(typeof(SaveList));
                entries = (SaveList)xmlreader.Deserialize(filestream);
            }
            Assert.AreEqual(entries.Count, 0);
        }
    }

    #endregion
}
