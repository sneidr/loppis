using loppis.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Media;

namespace LoppisTest;

[TestClass]
public class EnterSale
{
    #region EnterSale Tests

    [TestMethod]
    public void Sum_Is_Equal_To_Price_When_Entering_One_Sale()
    {
        SalesViewModel vm = new();
        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 12;
        vm.EnterSale();

        Assert.AreEqual(vm.SumTotal, 62);
    }

    [TestMethod]
    public void List_Has_One_Entry_When_Entering_One_Sale()
    {
        SalesViewModel vm = new();
        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 12;
        vm.EnterSale();

        Assert.AreEqual(vm.ItemList.Count, 1);
    }

    [TestMethod]
    public void List_Entry_Is_Correct_When_Entering_One_Sale()
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
    public void All_Sales_Are_Added_To_List_When_Entering_Multiple_Sales()
    {
        SalesViewModel vm = new();
        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 12;
        vm.EnterSale();
        vm.CurrentEntry.Price = 55;
        vm.CurrentEntry.SellerId = 15;
        vm.EnterSale();

        Assert.AreEqual(vm.ItemList[0].Price, 55);
        Assert.AreEqual(vm.ItemList[1].Price, 62);
    }

    [TestMethod]
    public void Sum_Is_Total_Of_Sales_When_Entering_Multiple_Sales()
    {
        SalesViewModel vm = new();
        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 12;
        vm.EnterSale();
        vm.CurrentEntry.Price = 55;
        vm.CurrentEntry.SellerId = 15;
        vm.EnterSale();

        Assert.AreEqual(vm.SumTotal, 62 + 55);
    }

    [TestMethod]
    public void Current_Entry_Is_Cleared_When_Entering_Sale()
    {
        SalesViewModel vm = new();
        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 12;
        vm.EnterSale();

        Assert.IsNull(vm.CurrentEntry.Price);
        Assert.IsNull(vm.CurrentEntry.SellerId);
    }
    #endregion
}

[TestClass]
public class CanEnterSale
{
    [TestMethod]
    public void Can_Enter_Sale_When_SellerId_Is_Valid()
    {
        TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Hej Svej\r\n8;Kasse;1\r\n9;Vykort;2");

        SalesViewModel vm = new();
        vm.LoadCommand.Execute(null);

        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 7;
        Assert.IsTrue(vm.EnterSaleCommand.CanExecute(null));
    }

    [TestMethod]
    public void Cannot_Enter_Sale_When_SellerId_Is_Invalid()
    {
        TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Hej Svej\r\n8;Kasse;1\r\n9;Vykort;2");

        SalesViewModel vm = new();
        vm.LoadCommand.Execute(null);

        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 12;
        Assert.IsFalse(vm.EnterSaleCommand.CanExecute(null));
    }

    [TestMethod]
    public void SellerId_Background_White_When_SellerId_Is_Valid()
    {
        TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Hej Svej\r\n8;Kasse;1\r\n9;Vykort;2");

        SalesViewModel vm = new();
        vm.LoadCommand.Execute(null);

        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 7;
        Assert.AreEqual(Colors.White, ((SolidColorBrush)vm.SellerIdBackground).Color);
    }

    [TestMethod]
    public void SellerId_Background_Orange_When_SellerId_Is_Invalid()
    {
        TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Hej Svej\r\n8;Kasse;1\r\n9;Vykort;2");

        SalesViewModel vm = new();
        Assert.AreEqual(Colors.White, ((SolidColorBrush)vm.SellerIdBackground).Color);
        vm.LoadCommand.Execute(null);

        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 12;
        Assert.IsFalse(vm.EnterSaleCommand.CanExecute(null));
        Assert.AreEqual(Colors.Orange, ((SolidColorBrush)vm.SellerIdBackground).Color);
    }

    [TestMethod]
    public void Can_Enter_Sale_When_SellerId_Is_999_For_RoundUp()
    {
        SalesViewModel vm = new();
        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 999;
        Assert.IsTrue(vm.EnterSaleCommand.CanExecute(null));
    }

}

