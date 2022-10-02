using loppis.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Media;

namespace LoppisTest;

[TestClass]
public class MoveFocus
{
    [DataTestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(7)]
    [DataRow(8)]
    [DataRow(9)]
    [DataRow(999)] // 999 is RoundUp default value
    public void Can_Move_Focus_When_SellerId_Is_Valid(int sellerId)
    {
        TestFiles.SetupSellerListFile("1;Firstname LastName\r\n2;John Doe\r\n7;Hej Svej\r\n8;Kasse;1\r\n9;Vykort;2");

        SalesViewModel vm = new();
        vm.LoadCommand.Execute(null);

        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = sellerId;
        Assert.IsTrue(vm.MoveFocusCommand.CanExecute(null));
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(5)]
    [DataRow(100)]
    [DataRow(int.MaxValue)]
    [DataRow(int.MinValue)]
    public void Cannot_Move_Focus_When_SellerId_Is_Invalid(int sellerId)
    {
        TestFiles.SetupSellerListFile("1;Firstname LastName\r\n2;John Doe\r\n7;Hej Svej\r\n8;Kasse;1\r\n9;Vykort;2");

        SalesViewModel vm = new();
        vm.LoadCommand.Execute(null);

        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = sellerId;
        Assert.IsFalse(vm.MoveFocusCommand.CanExecute(null));
    }

    [TestMethod]
    public void SellerId_Background_Is_White_When_SellerId_Is_Valid()
    {
        TestFiles.SetupSellerListFile("1;Firstname LastName\r\n2;John Doe\r\n7;Hej Svej\r\n8;Kasse;1\r\n9;Vykort;2");

        SalesViewModel vm = new();
        vm.LoadCommand.Execute(null);

        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 7;

        vm.MoveFocusCommand.CanExecute(null);
        Assert.AreEqual(Colors.White, ((SolidColorBrush)vm.SellerIdBackground).Color);
    }

    [TestMethod]
    public void SellerId_Background_Is_Orange_When_SellerId_Is_Invalid()
    {
        TestFiles.SetupSellerListFile("1;Firstname LastName\r\n2;John Doe\r\n7;Hej Svej\r\n8;Kasse;1\r\n9;Vykort;2");

        SalesViewModel vm = new();
        vm.LoadCommand.Execute(null);

        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 12;
        
        vm.MoveFocusCommand.CanExecute(null);
        Assert.AreEqual(Colors.Orange, ((SolidColorBrush)vm.SellerIdBackground).Color);
    }

    [TestMethod]
    public void SellerId_Background_Is_White_Before_Any_Move()
    {
        SalesViewModel vm = new();
        Assert.AreEqual(Colors.White, ((SolidColorBrush)vm.SellerIdBackground).Color);
    }
}
