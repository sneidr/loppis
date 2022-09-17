using loppis.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoppisTest;

[TestClass]
public class Load
{
    

    [TestMethod]
    public void TestLoadCommand()
    {
        TestFiles.RemoveConfigFile();

        {
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            SalesViewModel vm = new(TestFiles.TransactionsFile)
            {
                ShutDownFunction = () => { isShutDown = true; },
                MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; }
            };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.AreEqual(0, vm.SellerList.Count);
            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        {

            TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Kasse;78\r\n8;Vykort;15");

            SalesViewModel vm = new(TestFiles.TransactionsFile);
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.AreEqual(4, vm.SellerList.Count);
            Assert.AreEqual("John Doe", vm.SellerList[2].Name);
        }
        { // Error: Empty file
            TestFiles.SetupConfigFile("");

            SalesViewModel vm = new(TestFiles.TransactionsFile);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        { // Error: Cannot convert to int
            TestFiles.SetupConfigFile("A;Firstname LastName\r\n2;John Doe\r\n7;Kasse");

            SalesViewModel vm = new(TestFiles.TransactionsFile);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        { // Error: Duplicate ids
            TestFiles.SetupConfigFile("1;Firstname LastName\r\n1;John Doe\r\n7;Kasse");

            SalesViewModel vm = new(TestFiles.TransactionsFile);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        { // Error: Missing semicolon
            TestFiles.SetupConfigFile("1 Firstname LastName\r\n1;John Doe\r\n7;Kasse");

            SalesViewModel vm = new(TestFiles.TransactionsFile);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        { // Error: Missing line breaks
            TestFiles.SetupConfigFile("1 Firstname LastName 1;John Doe 7;Kasse");

            SalesViewModel vm = new(TestFiles.TransactionsFile);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        { // Default price
            TestFiles.SetupConfigFile("1;Firstname LastName\r\n8;Vykort;15\r\n11;Kasse;5");

            SalesViewModel vm = new(TestFiles.TransactionsFile);
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.AreEqual(3, vm.SellerList.Count);
            Assert.AreEqual(15, vm.SellerList[8].DefaultPrice);
            Assert.AreEqual(5, vm.SellerList[11].DefaultPrice);
        }
        { // Error: Default price not int
            TestFiles.SetupConfigFile("1;Firstname LastName\r\n8;Vykort;15\r\n11;Kasse;Hej");
            SalesViewModel vm = new(TestFiles.TransactionsFile);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };

            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        { // Error: No bag entry in file
            TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Hej Svej\r\n8;Vykort;15");

            SalesViewModel vm = new(TestFiles.TransactionsFile);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        { // Error: No default price for bag entry
            TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Kasse\r\n8;Vykort;15");

            SalesViewModel vm = new(TestFiles.TransactionsFile);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        { // Error: No card entry in file
            TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Kasse;15");

            SalesViewModel vm = new(TestFiles.TransactionsFile);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        { // Error: No default price for card entry
            TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Kasse;23\r\n8;Vykort");

            SalesViewModel vm = new(TestFiles.TransactionsFile);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
        { // Error: 999 is reserved for roundup
            TestFiles.SetupConfigFile("1;Firstname LastName\r\n2;John Doe\r\n7;Kasse;55\r\n8;Vykort;23\r\n999;Ajajaj");

            SalesViewModel vm = new(TestFiles.TransactionsFile);
            bool isShutDown = false;
            bool wasMessageBoxShown = false;
            vm.ShutDownFunction = () => { isShutDown = true; };
            vm.MsgBoxFunction = (string a, string b) => { wasMessageBoxShown = true; return System.Windows.MessageBoxResult.OK; };
            Assert.IsTrue(vm.LoadCommand.CanExecute(null));
            vm.LoadCommand.Execute(null);

            Assert.IsTrue(isShutDown);
            Assert.IsTrue(wasMessageBoxShown);
        }
    }

}
