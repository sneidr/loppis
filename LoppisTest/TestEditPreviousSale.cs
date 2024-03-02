using loppis.Model;
using loppis.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using SaveList = System.Collections.Generic.List<loppis.Model.Sale>;

namespace LoppisTest;

[TestClass]
public class EditPreviousSale
{
    public static Dictionary<int, Seller> TestSellerList => new()
        {
            { 3, new Seller() { Name = "Lisa" } },
            { 5, new Seller() { Name = "Humle Dumle" } },
            { 7, new Seller() { Name = "Pippi" } }
        };

    [TestMethod]
    public void Previous_Item_Should_End_Up_In_List_When_Doing_Edit_Previous()
    {
        var testFiles = new TestFiles();
        testFiles.RemoveTransactionsFile();
        SalesViewModel vm = new(testFiles.TransactionsFile) { Cashier = "Simon", SellerList = TestSellerList };
        vm.CurrentEntry.SellerId = 3;
        vm.CurrentEntry.Price = 34;

        vm.EnterSaleCommand.Execute(null);
        vm.SaveToFileCommand.Execute(null);

        vm.EditPreviousSaleCommand.Execute(0);

        Assert.AreEqual(1, vm.ItemList.Count);
        Assert.AreEqual(34, vm.ItemList[0].Price);
        Assert.AreEqual(3, vm.ItemList[0].SellerId);
        Assert.AreEqual(34, vm.SumTotal);
    }

    [TestMethod]
    public void Previous_Item_Should_Be_Removed_From_Last_Sales_When_Doing_Edit_Previous()
    {
        var testFiles = new TestFiles();
        testFiles.RemoveTransactionsFile();
        SalesViewModel vm = new(testFiles.TransactionsFile) { Cashier = "Simon", SellerList = TestSellerList };

        vm.CurrentEntry.SellerId = 3;
        vm.CurrentEntry.Price = 34;

        vm.EnterSaleCommand.Execute(null);
        vm.SaveToFileCommand.Execute(null);

        vm.EditPreviousSaleCommand.Execute(0);

        Assert.AreEqual(0, vm.LastSalesList.Count);
    }

    [TestMethod]
    public void Previous_Item_Should_Be_Removed_From_Transactions_File_When_Doing_Edit_Previous()
    {
        var testFiles = new TestFiles();
        testFiles.RemoveTransactionsFile();
        SalesViewModel vm = new(testFiles.TransactionsFile) { Cashier = "Simon", SellerList = TestSellerList };

        vm.CurrentEntry.SellerId = 3;
        vm.CurrentEntry.Price = 34;

        vm.EnterSaleCommand.Execute(null);
        vm.SaveToFileCommand.Execute(null);

        vm.EditPreviousSaleCommand.Execute(0);

        SaveList entries;
        using (var filestream = new FileStream(testFiles.TransactionsFile, FileMode.Open))
        {
            var xmlreader = new XmlSerializer(typeof(SaveList));
            entries = (SaveList)xmlreader.Deserialize(filestream);
        }
        Assert.AreEqual(0, entries.Count);
    }

    [TestMethod]
    public void TestEditPreviousSaleCommand()
    {
        var testFiles = new TestFiles();
        testFiles.RemoveTransactionsFile();
        SalesViewModel vm = new(testFiles.TransactionsFile) { Cashier = "Simon", SellerList = TestSellerList };

        vm.CurrentEntry.SellerId = 3;
        vm.CurrentEntry.Price = 34;

        vm.EnterSaleCommand.Execute(null);
        vm.SaveToFileCommand.Execute(null);

        vm.CurrentEntry.SellerId = 5;
        vm.CurrentEntry.Price = 54;

        vm.EnterSaleCommand.Execute(null);
        vm.SaveToFileCommand.Execute(null);

        {
            vm.EditPreviousSaleCommand.Execute(1);

            Assert.AreEqual(1, vm.ItemList.Count);
            Assert.AreEqual(34, vm.ItemList[0].Price);
            Assert.AreEqual(3, vm.ItemList[0].SellerId);
            Assert.AreEqual(34, vm.SumTotal);

            Assert.AreEqual(1, vm.LastSalesList.Count);

            SaveList entries;
            using (var filestream = new FileStream(testFiles.TransactionsFile, FileMode.Open))
            {
                var xmlreader = new XmlSerializer(typeof(SaveList));
                entries = (SaveList)xmlreader.Deserialize(filestream);
            }
            Assert.AreEqual(1, entries.Count);
            Assert.AreEqual(5, entries[0].Entries[0].SellerId);
            Assert.AreEqual(54, entries[0].Entries[0].Price);
        }
        {
            Assert.IsFalse(vm.EditPreviousSaleCommand.CanExecute(null));
            vm.ItemList.Clear();

            Assert.IsTrue(vm.EditPreviousSaleCommand.CanExecute(null));
            vm.EditPreviousSaleCommand.Execute(0);

            Assert.AreEqual(1, vm.ItemList.Count);
            Assert.AreEqual(54, vm.ItemList[0].Price);
            Assert.AreEqual(5, vm.ItemList[0].SellerId);

            Assert.AreEqual(0, vm.LastSalesList.Count);

            SaveList entries;
            using (var filestream = new FileStream(testFiles.TransactionsFile, FileMode.Open))
            {
                var xmlreader = new XmlSerializer(typeof(SaveList));
                entries = (SaveList)xmlreader.Deserialize(filestream);
            }
            Assert.AreEqual(0, entries.Count);
        }
    }
}

[TestClass]
public class CanEditPreviousSale
{
    public static Dictionary<int, Seller> TestSellerList => new()
        {
            { 3, new Seller() { Name = "Lisa" } },
            { 5, new Seller() { Name = "Humle Dumle" } }
        };

    [TestMethod]
    public void Cannot_Edit_Previous_Sale_When_No_Sale_Has_Been_Entered()
    {
        var testFiles = new TestFiles();
        testFiles.RemoveTransactionsFile();

        SalesViewModel vm = new(testFiles.TransactionsFile);
        Assert.IsFalse(vm.EditPreviousSaleCommand.CanExecute(null));
    }

    [TestMethod]
    public void Can_Edit_Previous_Sale_When_One_Sale_Has_Been_Entered()
    {
        var testFiles = new TestFiles();
        testFiles.RemoveTransactionsFile();

        SalesViewModel vm = new(testFiles.TransactionsFile) { Cashier = "Simon", SellerList = TestSellerList };

        vm.CurrentEntry.SellerId = 3;
        vm.CurrentEntry.Price = 34;

        vm.EnterSaleCommand.Execute(null);
        vm.SaveToFileCommand.Execute(null);
        Assert.IsTrue(vm.EditPreviousSaleCommand.CanExecute(null));
    }

    [TestMethod]
    public void Cannot_Edit_Previous_Sale_When_Already_Editing()
    {
        var testFiles = new TestFiles();
        testFiles.RemoveTransactionsFile();
        SalesViewModel vm = new(testFiles.TransactionsFile) { Cashier = "Simon", SellerList = TestSellerList };

        vm.CurrentEntry.SellerId = 3;
        vm.CurrentEntry.Price = 34;

        vm.EnterSaleCommand.Execute(null);
        vm.SaveToFileCommand.Execute(null);

        vm.EditPreviousSaleCommand.Execute(0);
        Assert.IsFalse(vm.EditPreviousSaleCommand.CanExecute(null));
    }

    [TestMethod]
    public void Can_Edit_Previous_Sale_After_Clearing_Edit()
    {
        var testFiles = new TestFiles();
        testFiles.RemoveTransactionsFile();
        SalesViewModel vm = new(testFiles.TransactionsFile) { Cashier = "Simon", SellerList = TestSellerList };

        vm.CurrentEntry.SellerId = 3;
        vm.CurrentEntry.Price = 34;

        vm.EnterSaleCommand.Execute(null);
        vm.SaveToFileCommand.Execute(null);

        vm.CurrentEntry.SellerId = 5;
        vm.CurrentEntry.Price = 44;

        vm.EnterSaleCommand.Execute(null);
        vm.SaveToFileCommand.Execute(null);

        vm.EditPreviousSaleCommand.Execute(0);

        vm.UndoCommand.Execute(0);
        Assert.IsTrue(vm.EditPreviousSaleCommand.CanExecute(null));
    }
}