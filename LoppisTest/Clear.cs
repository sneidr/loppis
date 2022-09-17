using loppis.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoppisTest;

[TestClass]
public class Clear
{
    
    [TestMethod]
    public void TestClearCommand()
    {
        {
            SalesViewModel vm = new();
            Assert.IsFalse(vm.ClearCommand.CanExecute(null));

            vm.CurrentEntry.SellerId = 1;
            Assert.IsTrue(vm.ClearCommand.CanExecute(null));

            vm.ClearCommand.Execute(null);
            Assert.IsNull(vm.CurrentEntry.SellerId);
        }
        {
            SalesViewModel vm = new();
            Assert.IsFalse(vm.ClearCommand.CanExecute(null));

            vm.CurrentEntry.Price = 2;
            Assert.IsTrue(vm.ClearCommand.CanExecute(null));

            vm.ClearCommand.Execute(null);
            Assert.IsNull(vm.CurrentEntry.Price);
        }
        {
            SalesViewModel vm = new();
            Assert.IsFalse(vm.ClearCommand.CanExecute(null));

            vm.CurrentEntry.SellerId = 1;
            vm.CurrentEntry.Price = 2;
            Assert.IsTrue(vm.ClearCommand.CanExecute(null));

            vm.ClearCommand.Execute(null);
            Assert.IsNull(vm.CurrentEntry.SellerId);
            Assert.IsNull(vm.CurrentEntry.Price);
            Assert.IsTrue(vm.SellerIdFocused);
            Assert.IsFalse(vm.PriceFocused);
        }
    }
}
