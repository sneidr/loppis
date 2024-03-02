using loppis.Model;
using loppis.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LoppisTest;

[TestClass]
public class LoadSellers
{
    [TestMethod]
    public void Load_Is_Possible()
    {
        SalesViewModel vm = new();
        Assert.IsTrue(vm.LoadCommand.CanExecute(null));
    }

    [TestMethod]
    public void Program_Is_Shut_Down_If_There_Is_No_Seller_File()
    {
        var testFiles = new TestFiles();
        bool isShutDown = false;
        bool wasMessageBoxShown = false;
        SalesViewModel vm = new(testFiles.TransactionsFile)
        {
            ShutDownFunction = () => { isShutDown = true; },
            MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; }
        };
        if(File.Exists(SalesViewModel.cSellerFileName))
        {
            File.Delete(SalesViewModel.cSellerFileName);
        }
        vm.LoadCommand.Execute(null);

        Assert.AreEqual(0, vm.SellerList.Count);
        Assert.IsTrue(isShutDown);
        Assert.IsTrue(wasMessageBoxShown);
    }

    [TestMethod]
    public void Sellers_Can_Be_Loaded_Into_SellerList()
    {
        var testFiles = new TestFiles();
        SalesViewModel vm = new(testFiles.TransactionsFile);
        vm.LoadSellerList("1;Firstname LastName\r\n2;John Doe\r\n7;Kasse;78\r\n8;Vykort;15");

        Assert.AreEqual(4, vm.SellerList.Count);
        Assert.AreEqual("John Doe", vm.SellerList[2].Name);
    }

    [TestMethod]
    public void Program_Is_Shut_Down_If_Seller_File_Is_Empty()
    {
        var testFiles = new TestFiles();
        SalesViewModel vm = new(testFiles.TransactionsFile);
        bool isShutDown = false;
        bool wasMessageBoxShown = false;
        vm.ShutDownFunction = () => { isShutDown = true; };
        vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
        vm.LoadSellerList("");

        Assert.IsTrue(isShutDown);
        Assert.IsTrue(wasMessageBoxShown);
    }

    [TestMethod]
    public void Program_Is_Shut_Down_If_Seller_Id_Cannot_Be_Interpreted_As_Integer()
    {
        var testFiles = new TestFiles();
        SalesViewModel vm = new(testFiles.TransactionsFile);
        bool isShutDown = false;
        bool wasMessageBoxShown = false;
        vm.ShutDownFunction = () => { isShutDown = true; };
        vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
        vm.LoadSellerList("A;Firstname LastName\r\n2;John Doe\r\n7;Kasse");

        Assert.IsTrue(isShutDown);
        Assert.IsTrue(wasMessageBoxShown);
    }

    [TestMethod]
    public void Program_Is_Shut_Down_If_There_Are_Duplicate_Seller_Ids()
    {
        var testFiles = new TestFiles();
        SalesViewModel vm = new(testFiles.TransactionsFile);
        bool isShutDown = false;
        bool wasMessageBoxShown = false;
        vm.ShutDownFunction = () => { isShutDown = true; };
        vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
        vm.LoadSellerList("1;Firstname LastName\r\n1;John Doe\r\n7;Kasse");

        Assert.IsTrue(isShutDown);
        Assert.IsTrue(wasMessageBoxShown);
    }

    [TestMethod]
    public void Program_Is_Shut_Down_If_A_Semi_Colon_Is_Missing()
    {
        var testFiles = new TestFiles();
        SalesViewModel vm = new(testFiles.TransactionsFile);
        bool isShutDown = false;
        bool wasMessageBoxShown = false;
        vm.ShutDownFunction = () => { isShutDown = true; };
        vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
        vm.LoadSellerList("1 Firstname LastName\r\n1;John Doe\r\n7;Kasse");

        Assert.IsTrue(isShutDown);
        Assert.IsTrue(wasMessageBoxShown);
    }

    [TestMethod]
    public void Program_Is_Shut_Down_If_A_Line_Break_Is_Missing()
    {
        var testFiles = new TestFiles();
        SalesViewModel vm = new(testFiles.TransactionsFile);
        bool isShutDown = false;
        bool wasMessageBoxShown = false;
        vm.ShutDownFunction = () => { isShutDown = true; };
        vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
        vm.LoadSellerList("1 Firstname LastName 1;John Doe 7;Kasse");

        Assert.IsTrue(isShutDown);
        Assert.IsTrue(wasMessageBoxShown);
    }

    [TestMethod]
    public void Default_Price_Can_Be_Set_From_Seller_File()
    {
        var testFiles = new TestFiles();
        SalesViewModel vm = new(testFiles.TransactionsFile);
        vm.LoadSellerList("1;Firstname LastName\r\n8;Vykort;15\r\n11;Kasse;5");

        Assert.AreEqual(3, vm.SellerList.Count);
        Assert.AreEqual(15, vm.SellerList[8].DefaultPrice);
        Assert.AreEqual(5, vm.SellerList[11].DefaultPrice);
    }

    [TestMethod]
    public void Program_Is_Shut_Down_If_Default_Price_Is_Not_An_Integer()
    {
        var testFiles = new TestFiles();
        SalesViewModel vm = new(testFiles.TransactionsFile);
        bool isShutDown = false;
        bool wasMessageBoxShown = false;
        vm.ShutDownFunction = () => { isShutDown = true; };
        vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };

        vm.LoadSellerList("1;Firstname LastName\r\n8;Vykort;15\r\n11;Kasse;Hej");

        Assert.IsTrue(isShutDown);
        Assert.IsTrue(wasMessageBoxShown);

    }

    [TestMethod]
    public void Program_Is_Shut_Down_If_There_Is_No_Bag_In_Seller_File()
    {
        var testFiles = new TestFiles();
        SalesViewModel vm = new(testFiles.TransactionsFile);
        bool isShutDown = false;
        bool wasMessageBoxShown = false;
        vm.ShutDownFunction = () => { isShutDown = true; };
        vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
        vm.LoadSellerList("1;Firstname LastName\r\n2;John Doe\r\n7;Hej Svej\r\n8;Vykort;15");

        Assert.IsTrue(isShutDown);
        Assert.IsTrue(wasMessageBoxShown);

    }

    [TestMethod]
    public void Program_Is_Shut_Down_If_There_Is_No_Default_Price_For_Bags()
    {
        var testFiles = new TestFiles();
        SalesViewModel vm = new(testFiles.TransactionsFile);
        bool isShutDown = false;
        bool wasMessageBoxShown = false;
        vm.ShutDownFunction = () => { isShutDown = true; };
        vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
        vm.LoadSellerList("1;Firstname LastName\r\n2;John Doe\r\n7;Kasse\r\n8;Vykort;15");

        Assert.IsTrue(isShutDown);
        Assert.IsTrue(wasMessageBoxShown);

    }

    [TestMethod]
    public void Program_Is_Shut_Down_If_There_Is_No_Card_In_Seller_File()
    {
        var testFiles = new TestFiles();
        SalesViewModel vm = new(testFiles.TransactionsFile);
        bool isShutDown = false;
        bool wasMessageBoxShown = false;
        vm.ShutDownFunction = () => { isShutDown = true; };
        vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
        vm.LoadSellerList("1;Firstname LastName\r\n2;John Doe\r\n7;Kasse;15");

        Assert.IsTrue(isShutDown);
        Assert.IsTrue(wasMessageBoxShown);

    }

    [TestMethod]
    public void Program_Is_Shut_Down_If_There_Is_No_Default_Price_For_Cards()
    {
        var testFiles = new TestFiles();
        SalesViewModel vm = new(testFiles.TransactionsFile);
        bool isShutDown = false;
        bool wasMessageBoxShown = false;
        vm.ShutDownFunction = () => { isShutDown = true; };
        vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
        vm.LoadSellerList("1;Firstname LastName\r\n2;John Doe\r\n7;Kasse;23\r\n8;Vykort");

        Assert.IsTrue(isShutDown);
        Assert.IsTrue(wasMessageBoxShown);

    }

    [TestMethod]
    public void Program_Is_Shut_Down_If_A_Seller_Has_Reserved_SellerId_999()
    {
        var testFiles = new TestFiles();
        SalesViewModel vm = new(testFiles.TransactionsFile);
        bool isShutDown = false;
        bool wasMessageBoxShown = false;
        vm.ShutDownFunction = () => { isShutDown = true; };
        vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
        vm.LoadSellerList("1;Firstname LastName\r\n2;John Doe\r\n7;Kasse;55\r\n8;Vykort;23\r\n999;Ajajaj");

        Assert.IsTrue(isShutDown);
        Assert.IsTrue(wasMessageBoxShown);

    }
}
