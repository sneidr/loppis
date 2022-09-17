using loppis.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Media;

namespace LoppisTest;

[TestClass]
public class MoveFocus
{
    [TestMethod]
    public void Can_Move_Focus_When_SellerId_Is_Valid()
    {
        TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Hej Svej\r\n8;Kasse;1\r\n9;Vykort;2");

        SalesViewModel vm = new();
        vm.LoadCommand.Execute(null);

        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 7;
        Assert.IsTrue(vm.MoveFocusCommand.CanExecute(null));
    }

    [TestMethod]
    public void Cannot_Move_Focus_When_SellerId_Is_Invalid()
    {
        TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Hej Svej\r\n8;Kasse;1\r\n9;Vykort;2");

        SalesViewModel vm = new();
        vm.LoadCommand.Execute(null);

        vm.CurrentEntry.Price = 62;
        vm.CurrentEntry.SellerId = 12;
        Assert.IsFalse(vm.MoveFocusCommand.CanExecute(null));
    }

    [TestMethod]
    public void SellerId_Background_Is_White_When_SellerId_Is_Valid()
    {
        TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Hej Svej\r\n8;Kasse;1\r\n9;Vykort;2");

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
        TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Hej Svej\r\n8;Kasse;1\r\n9;Vykort;2");

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
