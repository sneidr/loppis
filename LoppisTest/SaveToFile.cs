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
        SalesViewModel vm = new(TestFiles.TransactionsFile);
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
        TestFiles.RemoveTransactionsFile();

        SalesViewModel vm = new(TestFiles.TransactionsFile)
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
        using (var filestream = new FileStream(TestFiles.TransactionsFile, FileMode.Open))
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
        TestFiles.SetupTransactionsFile("");

        SalesViewModel vm = new(TestFiles.TransactionsFile)
        {
            Cashier = "Simon"
        };
        vm.CurrentEntry.SellerId = 12;
        vm.CurrentEntry.Price = 80;
        vm.EnterSale();
        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        vm.SaveToFileCommand.Execute(null);

        var entries = new SaveList();
        using (var filestream = new FileStream(TestFiles.TransactionsFile, FileMode.Open))
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
        TestFiles.SetupTransactionsFile("ErrorText");
        TestFiles.RemoveErrorFiles();

        SalesViewModel vm = new(TestFiles.TransactionsFile)
        {
            Cashier = "Simon"
        };

        vm.CurrentEntry.SellerId = 12;
        vm.CurrentEntry.Price = 80;
        vm.EnterSale();
        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        vm.SaveToFileCommand.Execute(null);

        var entries = new SaveList();
        using (var filestream = new FileStream(TestFiles.TransactionsFile, FileMode.Open))
        {
            var xmlreader = new XmlSerializer(typeof(SaveList));
            entries = (SaveList)xmlreader.Deserialize(filestream);
        }

        Assert.AreEqual(entries[0][0].SellerId, 12);
        Assert.AreEqual(entries[0][0].Price, 80);

        Assert.IsTrue(File.Exists(TestFiles.FirstErrorFile));
        Assert.IsFalse(File.Exists(TestFiles.SeccondErrorFile));

        File.Delete(TestFiles.TransactionsFile);
        File.Copy(TestFiles.FirstErrorFile, TestFiles.TransactionsFile);

        vm.CurrentEntry.SellerId = 12;
        vm.CurrentEntry.Price = 80;
        vm.EnterSale();
        Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        vm.SaveToFileCommand.Execute(null);

        entries = new SaveList();
        using (var filestream = new FileStream(TestFiles.TransactionsFile, FileMode.Open))
        {
            var xmlreader = new XmlSerializer(typeof(SaveList));
            entries = (SaveList)xmlreader.Deserialize(filestream);
        }

        Assert.AreEqual(entries[0][0].SellerId, 12);
        Assert.AreEqual(entries[0][0].Price, 80);

        Assert.IsTrue(File.Exists(TestFiles.SeccondErrorFile));
    }

    [TestMethod]
    [Ignore]
    public void TestSaveToFile_Execute_Performance()
    {
        TestFiles.SetupTransactionsFile("");

        SalesViewModel vm = new(TestFiles.TransactionsFile);

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
        TestFiles.RemoveTransactionsFile();


        SalesViewModel vm = new(TestFiles.TransactionsFile)
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
}
