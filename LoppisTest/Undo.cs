using loppis.ViewModels;
using DataAccess.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoppisTest;

[TestClass]
public class Undo
{
    

    [TestMethod]
    public void TestUndoCommand()
    {
        SalesViewModel vm = new();
        vm.CurrentEntry.SellerId = 1;
        vm.CurrentEntry.Price = 3;

        Assert.IsFalse(vm.UndoCommand.CanExecute(0));
        vm.SellerList.Add(1, new Seller() { Name = "Kim Karlsson", DefaultPrice = null });
        Assert.IsTrue(vm.EnterSaleCommand.CanExecute(null));
        vm.EnterSaleCommand.Execute(null);

        Assert.IsTrue(vm.UndoCommand.CanExecute(0));
        vm.UndoCommand.Execute(0);
        Assert.AreEqual(1, vm.CurrentEntry.SellerId);
        Assert.AreEqual(3, vm.CurrentEntry.Price);
        Assert.AreEqual(0, vm.SumTotal);
        Assert.IsTrue(vm.SellerIdFocused);
        Assert.IsFalse(vm.PriceFocused);
    }

}
