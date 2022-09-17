using loppis.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoppisTest;

[TestClass]
public class RoundUp
{
    [TestMethod]
    public void TestRoundUpCommand_CanExecute()
    {
        SalesViewModel vm = new();
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
        SalesViewModel vm = new();
        vm.CurrentEntry.SellerId = 12;
        vm.CurrentEntry.Price = 43;
        vm.EnterSaleCommand.Execute(null);
        vm.RoundUpCommand.Execute(null);

        Assert.AreEqual(vm.SumTotal, 43);
        Assert.AreEqual(vm.CurrentEntry.Price, 50 - 43);
        Assert.AreEqual(vm.CurrentEntry.SellerId, 999);
        Assert.IsFalse(vm.SellerIdFocused);
        Assert.IsTrue(vm.PriceFocused);
    }

    [TestMethod]
    public void TestRoundUpCommand_Execute_DivisibleBy50()
    {
        SalesViewModel vm = new();
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

}
