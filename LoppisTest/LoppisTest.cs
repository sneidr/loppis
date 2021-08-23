using loppis.Model;
using loppis.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml.Serialization;
namespace LoppisTest
{
    [TestClass]
    public class LoppisTest
    {
        #region EnterSale Tests
        [TestMethod]
        public void TestEnterOneSale_SumIsEqualToPrice()
        {
            SalesViewModel vm = new SalesViewModel();
            vm.CurrentEntry.Price = 62;
            vm.CurrentEntry.SellerId = 12;
            vm.EnterSale();

            Assert.AreEqual(vm.SumTotal, 62);
        }

        [TestMethod]
        public void TestEnterOneSale_ListHasOneEntry()
        {
            SalesViewModel vm = new SalesViewModel();
            vm.CurrentEntry.Price = 62;
            vm.CurrentEntry.SellerId = 12;
            vm.EnterSale();

            Assert.AreEqual(vm.ItemList.Count, 1);
        }

        [TestMethod]
        public void TestEnterOneSale_ListEntryCorrect()
        {
            SalesViewModel vm = new SalesViewModel();
            vm.CurrentEntry.Price = 62;
            vm.CurrentEntry.SellerId = 12;
            vm.EnterSale();

            Assert.AreEqual(vm.ItemList[0].SellerId, 12);
            Assert.AreEqual(vm.ItemList[0].Price, 62);
        }

        [TestMethod]
        public void TestEnterMultipleSales_ListHasMultipleEntries()
        {
            SalesViewModel vm = new SalesViewModel();
            vm.CurrentEntry.Price = 62;
            vm.CurrentEntry.SellerId = 12;
            vm.EnterSale();
            vm.CurrentEntry.Price = 55;
            vm.CurrentEntry.SellerId = 15;
            vm.EnterSale();

            Assert.AreEqual(vm.SumTotal, 62 + 55);
        }

        [TestMethod]
        public void TestEnterOneSale_VMIsClearedAfterEntry()
        {
            SalesViewModel vm = new SalesViewModel();
            vm.CurrentEntry.Price = 62;
            vm.CurrentEntry.SellerId = 12;
            vm.EnterSale();

            Assert.IsNull(vm.CurrentEntry.Price);
            Assert.IsNull(vm.CurrentEntry.SellerId);
        }
        #endregion

        #region Bag Command Tests
        [TestMethod]
        public void TestBagCommand_CanExecute()
        {
            SalesViewModel vm = new SalesViewModel();
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
            SalesViewModel vm = new SalesViewModel();
            vm.BagCommand.Execute(null);
            Assert.AreEqual(vm.CurrentEntry.SellerId, 100);
            Assert.AreEqual(vm.CurrentEntry.Price, 5);
            Assert.IsFalse(vm.SellerIdFocused);
            Assert.IsTrue(vm.PriceFocused);
        }

        #endregion

        #region Card Command Tests
        [TestMethod]
        public void TestCardCommand_CanExecute()
        {
            SalesViewModel vm = new SalesViewModel();
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
            SalesViewModel vm = new SalesViewModel();
            vm.CardCommand.Execute(null);
            Assert.AreEqual(vm.CurrentEntry.SellerId, 150);
            Assert.AreEqual(vm.CurrentEntry.Price, 15);
            Assert.IsFalse(vm.SellerIdFocused);
            Assert.IsTrue(vm.PriceFocused);
        }

        [TestMethod]
        public void TestCardCommand_TryTwice_Focus()
        {
            SalesViewModel vm = new SalesViewModel();
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
            SalesViewModel vm = new SalesViewModel();
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
            SalesViewModel vm = new SalesViewModel();
            vm.CurrentEntry.SellerId = 12;
            vm.CurrentEntry.Price = 43;
            vm.EnterSaleCommand.Execute(null);
            vm.RoundUpCommand.Execute(null);

            Assert.AreEqual(vm.SumTotal, 43);
            Assert.AreEqual(vm.CurrentEntry.Price, 50 - 43);
            Assert.AreEqual(vm.CurrentEntry.SellerId, 200);
            Assert.IsFalse(vm.SellerIdFocused);
            Assert.IsTrue(vm.PriceFocused);
        }

        [TestMethod]
        public void TestRoundUpCommand_Execute_DivisibleBy50()
        {
            SalesViewModel vm = new SalesViewModel();
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

        #region SaveToFileCommand
        [TestMethod]
        public void TestSaveToFile_CanExecute()
        {
            SalesViewModel vm = new SalesViewModel();
            Assert.IsFalse(vm.SaveToFileCommand.CanExecute(null));

            vm.CurrentEntry.SellerId = 12;
            vm.CurrentEntry.Price = 80;
            vm.EnterSale();
            Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        }

        [TestMethod]
        public void TestSaveToFile_Execute()
        {
            SalesViewModel vm = new SalesViewModel();
            vm.CurrentEntry.SellerId = 12;
            vm.CurrentEntry.Price = 80;
            vm.EnterSale();
            Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));

            vm.SaveToFileCommand.Execute(null);

            var reader = new XmlSerializer(typeof(SaleEntry));
            StreamReader sr = new StreamReader(@".\myfile.xml");
            SaleEntry saleEntry = (SaleEntry)reader.Deserialize(sr);
            sr.Close();

            Assert.AreEqual(saleEntry.SellerId, 12);
            Assert.AreEqual(saleEntry.Price, 80);
        }
        #endregion

    }
}
