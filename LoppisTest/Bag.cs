using loppis.ViewModels;
using DataAccess.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoppisTest;

[TestClass]
public class Bag
{
    [TestMethod]
    public void SellerId_Taken_From_Kasse_After_Pressing_Bag()
    {
        SalesViewModel vm = new();
        vm.SellerList.Add(92, new Seller() { Name = "Kasse", DefaultPrice = 7 });
        vm.BagCommand.Execute(null);

        Assert.AreEqual(vm.CurrentEntry.SellerId, 92);
    }

    [TestMethod]
    public void Default_Price_Taken_From_Kasse_After_Pressing_Bag()
    {
        SalesViewModel vm = new();
        vm.SellerList.Add(92, new Seller() { Name = "Kasse", DefaultPrice = 7 });
        vm.BagCommand.Execute(null);

        Assert.AreEqual(vm.CurrentEntry.Price, 7);
    }

    [TestMethod]
    public void Price_Is_In_Focus_After_Bag_Command()
    {
        SalesViewModel vm = new();
        vm.SellerList.Add(92, new Seller() { Name = "Kasse", DefaultPrice = 7 });
        vm.BagCommand.Execute(null);

        Assert.IsFalse(vm.SellerIdFocused);
        Assert.IsTrue(vm.PriceFocused);
    }
}

[TestClass]
public class CanBag
{
    [TestMethod]
    public void Can_Bag_Before_Any_Entry()
    {
        SalesViewModel vm = new();
        Assert.IsTrue(vm.BagCommand.CanExecute(null));
    }

    [TestMethod]
    public void Can_Bag_When_Only_Price_Is_Entered()
    {
        SalesViewModel vm = new();

        vm.CurrentEntry.Price = 12;
        Assert.IsTrue(vm.BagCommand.CanExecute(null));
    }

    [TestMethod]
    public void Can_Bag_When_Only_SellerId_Is_Entered()
    {
        SalesViewModel vm = new();

        vm.CurrentEntry.SellerId = 43;
        Assert.IsTrue(vm.BagCommand.CanExecute(null));
    }
}
