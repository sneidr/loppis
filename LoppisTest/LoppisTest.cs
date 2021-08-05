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
            vm.Price = 62;
            vm.SellerId = 12;
            vm.EnterSale();

            Assert.AreEqual(vm.Sum, 62);
        }

        [TestMethod]
        public void TestEnterOneSale_ListHasOneEntry()
        {
            SalesViewModel vm = new SalesViewModel();
            vm.Price = 62;
            vm.SellerId = 12;
            vm.EnterSale();

            Assert.AreEqual(vm.ItemList.Count, 1);
        }

        [TestMethod]
        public void TestEnterOneSale_ListEntryCorrect()
        {
            SalesViewModel vm = new SalesViewModel();
            vm.Price = 62;
            vm.SellerId = 12;
            vm.EnterSale();

            Assert.AreEqual(vm.ItemList[0], new System.Tuple<int, int>(12, 62));
        }

        [TestMethod]
        public void TestEnterMultipleSales_ListHasMultipleEntries()
        {
            SalesViewModel vm = new SalesViewModel();
            vm.Price = 62;
            vm.SellerId = 12;
            vm.EnterSale();
            vm.Price = 55;
            vm.SellerId = 15;
            vm.EnterSale();

            Assert.AreEqual(vm.Sum, 62 + 55);
        }

        [TestMethod]
        public void TestEnterMultipleSales_ListHasMultipleEntries()
        {
            SalesViewModel vm = new SalesViewModel();
            vm.Price = 62;
            vm.SellerId = 12;
            vm.EnterSale();
            vm.Price = 55;
            vm.SellerId = 15;
            vm.EnterSale();

            Assert.AreEqual(vm.Sum, 62 + 55);
        }
    }
}
