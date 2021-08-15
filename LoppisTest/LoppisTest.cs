using loppis.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoppisTest
{
    [TestClass]
    public class LoppisTest
    {
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

            Assert.AreEqual(vm.CurrentEntry.Price, 0);
            Assert.AreEqual(vm.CurrentEntry.SellerId, 0);
        }
    }
}
