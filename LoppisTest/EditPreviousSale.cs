using loppis.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml.Serialization;
using SaveList = System.Collections.Generic.List<loppis.Model.Sale>;

namespace LoppisTest;

[TestClass]
public class EditPreviousSale
{
    
    [TestMethod]
    public void TestEditPreviousSaleCommand()
    {
        TestFiles.RemoveTransactionsFile();


        SalesViewModel vm = new(TestFiles.TransactionsFile);
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
            using (var filestream = new FileStream(TestFiles.TransactionsFile, FileMode.Open))
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
            using (var filestream = new FileStream(TestFiles.TransactionsFile, FileMode.Open))
            {
                var xmlreader = new XmlSerializer(typeof(SaveList));
                entries = (SaveList)xmlreader.Deserialize(filestream);
            }
            Assert.AreEqual(entries.Count, 0);
        }
    }

}
