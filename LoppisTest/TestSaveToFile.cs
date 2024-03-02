using loppis.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Xml.Serialization;
using SaveList = System.Collections.Generic.List<loppis.Model.Sale>;

namespace LoppisTest;

[TestClass]
public class SaveToFile
{
    
    [TestMethod]
    public void TestSaveToFile_CanExecute()
    {
        var testFiles = new TestFiles();
        SalesViewModel vm = new(testFiles.TransactionsFile);
        Assert.AreEqual(Colors.White, ((SolidColorBrush)vm.CashierBackground).Color);
        Assert.IsFalse(vm.SaveToFileCommand.CanExecute(null));

        Assert.AreEqual("Säljare", vm.Cashier);
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
        var testFiles = new TestFiles();
        testFiles.RemoveTransactionsFile();

        SalesViewModel vm = new(testFiles.TransactionsFile)
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

        SaveList entries;
        using (var filestream = new FileStream(testFiles.TransactionsFile, FileMode.Open))
        {
            var xmlreader = new XmlSerializer(typeof(SaveList));
            entries = (SaveList)xmlreader.Deserialize(filestream);
        }

        Assert.AreEqual(15, entries[0][0].SellerId);
        Assert.AreEqual(90, entries[0][0].Price);
        Assert.AreEqual(12, entries[0][1].SellerId);
        Assert.AreEqual(80, entries[0][1].Price);
        Assert.AreEqual(20, entries[1][0].SellerId);
        Assert.AreEqual(100, entries[1][0].Price);
        Assert.AreEqual("Lisa",entries[0].Cashier);
        Assert.AreEqual("Lisa",entries[1].Cashier);
        Assert.AreNotEqual(entries[0].Timestamp, entries[1].Timestamp);
        Assert.AreEqual(0, vm.SumTotal);
    }

    [TestMethod]
    public void TestSaveToFile_Execute_FileExists()
    {
        var testFiles = new TestFiles();
        testFiles.SetupTransactionsFile("");

        SalesViewModel vm = new(testFiles.TransactionsFile)
        {
            Cashier = "Simon"
        };
        vm.CurrentEntry.SellerId = 12;
        vm.CurrentEntry.Price = 80;
        vm.EnterSale();
        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        vm.SaveToFileCommand.Execute(null);

        SaveList entries;
        using (var filestream = new FileStream(testFiles.TransactionsFile, FileMode.Open))
        {
            var xmlreader = new XmlSerializer(typeof(SaveList));
            entries = (SaveList)xmlreader.Deserialize(filestream);
        }

        Assert.AreEqual(12, entries[0][0].SellerId);
        Assert.AreEqual(80, entries[0][0].Price);
    }

    [TestMethod]
    public void TestSaveToFile_Execute_FileWrongFormat()
    {
        var testFiles = new TestFiles();
        testFiles.SetupTransactionsFile("ErrorText");
        testFiles.RemoveErrorFiles();

        SalesViewModel vm = new(testFiles.TransactionsFile)
        {
            Cashier = "Simon"
        };

        vm.CurrentEntry.SellerId = 12;
        vm.CurrentEntry.Price = 80;
        vm.EnterSale();
        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        vm.SaveToFileCommand.Execute(null);

        SaveList entries;
        using (var filestream = new FileStream(testFiles.TransactionsFile, FileMode.Open))
        {
            var xmlreader = new XmlSerializer(typeof(SaveList));
            entries = (SaveList)xmlreader.Deserialize(filestream);
        }

        Assert.AreEqual(12, entries[0][0].SellerId);
        Assert.AreEqual(80, entries[0][0].Price);

        Assert.IsTrue(File.Exists(testFiles.FirstErrorFile));
        Assert.IsFalse(File.Exists(testFiles.SeccondErrorFile));

        File.Delete(testFiles.TransactionsFile);
        File.Copy(testFiles.FirstErrorFile, testFiles.TransactionsFile);

        vm.CurrentEntry.SellerId = 12;
        vm.CurrentEntry.Price = 80;
        vm.EnterSale();
        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        vm.SaveToFileCommand.Execute(null);

        entries.Clear();
        using (var filestream = new FileStream(testFiles.TransactionsFile, FileMode.Open))
        {
            var xmlreader = new XmlSerializer(typeof(SaveList));
            entries = (SaveList)xmlreader.Deserialize(filestream);
        }

        Assert.AreEqual(12, entries[0][0].SellerId);
        Assert.AreEqual(80, entries[0][0].Price);

        Assert.IsTrue(File.Exists(testFiles.SeccondErrorFile));
    }

    [TestMethod]
    [Ignore] // Takes too long to run
    public void TestSaveToFile_Execute_Performance()
    {
        var testFiles = new TestFiles();
        testFiles.SetupTransactionsFile("");

        SalesViewModel vm = new(testFiles.TransactionsFile);

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
        var testFiles = new TestFiles();
        testFiles.RemoveTransactionsFile();


        SalesViewModel vm = new(testFiles.TransactionsFile)
        {
            Cashier = "Lisa"
        };

        vm.CurrentEntry.SellerId = 12;
        vm.CurrentEntry.Price = 80;
        vm.EnterSale();
        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        vm.SaveToFileCommand.Execute(null);
        Assert.AreEqual(80, vm.LastSalesList[0].SumTotal);


        vm.CurrentEntry.SellerId = 15;
        vm.CurrentEntry.Price = 90;
        vm.EnterSale();
        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        vm.SaveToFileCommand.Execute(null);
        Assert.AreEqual(90, vm.LastSalesList[0].SumTotal);
        Assert.AreEqual(80, vm.LastSalesList[1].SumTotal);

        vm.CurrentEntry.SellerId = 20;
        vm.CurrentEntry.Price = 100;
        vm.EnterSale();
        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        vm.SaveToFileCommand.Execute(null);
        Assert.AreEqual(100, vm.LastSalesList[0].SumTotal);
        Assert.AreEqual(90, vm.LastSalesList[1].SumTotal);
        Assert.AreEqual(80, vm.LastSalesList[2].SumTotal);

        vm.CurrentEntry.SellerId = 20;
        vm.CurrentEntry.Price = 110;
        vm.EnterSale();
        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        vm.SaveToFileCommand.Execute(null);
        Assert.AreEqual(110, vm.LastSalesList[0].SumTotal);
        Assert.AreEqual(100, vm.LastSalesList[1].SumTotal);
        Assert.AreEqual(90, vm.LastSalesList[2].SumTotal);
        Assert.AreEqual(3, vm.LastSalesList.Count);
    }
}
