using loppis.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoppisTest;

[TestClass]
public class Card
{
    [TestMethod]
    public void TestCardCommand_CanExecute()
    {
        SalesViewModel vm = new();
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
        SalesViewModel vm = new();
        vm.SellerList.Add(200, new Seller() { Name = "Vykort", DefaultPrice = 27 });
        vm.CardCommand.Execute(null);
        Assert.AreEqual(vm.CurrentEntry.SellerId, 200);
        Assert.AreEqual(vm.CurrentEntry.Price, 27);
        Assert.IsFalse(vm.SellerIdFocused);
        Assert.IsTrue(vm.PriceFocused);
    }

    [TestMethod]
    public void TestCardCommand_TryTwice_Focus()
    {
        SalesViewModel vm = new();
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
}
