﻿using loppis.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml.Serialization;
using SaveList = System.Collections.Generic.List<System.Collections.ObjectModel.ObservableCollection<loppis.Model.SaleEntry>>;

namespace LoppisTest
{
    [TestClass]
    public class LoppisTest
    {
        private const string testFileName = @".\myfile.xml";
        #region EnterSale Tests
        [TestMethod]
        public void TestEnterOneSale_SumIsEqualToPrice()
        {
            SalesViewModel vm = new SalesViewModel();
            vm.CurrentEntry.Price = 62;
            vm.CurrentEntry.SellerId = 12;
            vm.EnterSale();

            Assert.AreEqual(vm.SumTotal, 62);
        }

        [TestMethod]
        public void TestEnterOneSale_ListHasOneEntry()
        {
            SalesViewModel vm = new SalesViewModel();
            vm.CurrentEntry.Price = 62;
            vm.CurrentEntry.SellerId = 12;
            vm.EnterSale();

            Assert.AreEqual(vm.ItemList.Count, 1);
        }

        [TestMethod]
        public void TestEnterOneSale_ListEntryCorrect()
        {
            SalesViewModel vm = new SalesViewModel();
            vm.CurrentEntry.Price = 62;
            vm.CurrentEntry.SellerId = 12;
            vm.EnterSale();

            Assert.AreEqual(vm.ItemList[0].SellerId, 12);
            Assert.AreEqual(vm.ItemList[0].Price, 62);
        }

        [TestMethod]
        public void TestEnterMultipleSales_ListHasMultipleEntries()
        {
            SalesViewModel vm = new SalesViewModel();
            vm.CurrentEntry.Price = 62;
            vm.CurrentEntry.SellerId = 12;
            vm.EnterSale();
            vm.CurrentEntry.Price = 55;
            vm.CurrentEntry.SellerId = 15;
            vm.EnterSale();

            Assert.AreEqual(vm.SumTotal, 62 + 55);
            Assert.AreEqual(vm.ItemList[0].Price, 55);
            Assert.AreEqual(vm.ItemList[1].Price, 62);
        }

        [TestMethod]
        public void TestEnterOneSale_VMIsClearedAfterEntry()
        {
            SalesViewModel vm = new SalesViewModel();
            vm.CurrentEntry.Price = 62;
            vm.CurrentEntry.SellerId = 12;
            vm.EnterSale();

            Assert.IsNull(vm.CurrentEntry.Price);
            Assert.IsNull(vm.CurrentEntry.SellerId);
        }
        #endregion

        #region Bag Command Tests
        [TestMethod]
        public void TestBagCommand_CanExecute()
        {
            SalesViewModel vm = new SalesViewModel();
            Assert.IsTrue(vm.BagCommand.CanExecute(null));

            vm.CurrentEntry.Price = 12;
            Assert.IsTrue(vm.BagCommand.CanExecute(null));

            vm.CurrentEntry.Clear();
            vm.CurrentEntry.SellerId = 43;
            Assert.IsTrue(vm.BagCommand.CanExecute(null));

            vm.CurrentEntry.Clear();
            Assert.IsTrue(vm.BagCommand.CanExecute(null));
        }

        [TestMethod]
        public void TestBagCommand_Execute()
        {
            SalesViewModel vm = new SalesViewModel();
            vm.BagCommand.Execute(null);
            Assert.AreEqual(vm.CurrentEntry.SellerId, 100);
            Assert.AreEqual(vm.CurrentEntry.Price, 5);
            Assert.IsFalse(vm.SellerIdFocused);
            Assert.IsTrue(vm.PriceFocused);
        }

        #endregion

        #region Card Command Tests
        [TestMethod]
        public void TestCardCommand_CanExecute()
        {
            SalesViewModel vm = new SalesViewModel();
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
            SalesViewModel vm = new SalesViewModel();
            vm.CardCommand.Execute(null);
            Assert.AreEqual(vm.CurrentEntry.SellerId, 150);
            Assert.AreEqual(vm.CurrentEntry.Price, 15);
            Assert.IsFalse(vm.SellerIdFocused);
            Assert.IsTrue(vm.PriceFocused);
        }

        [TestMethod]
        public void TestCardCommand_TryTwice_Focus()
        {
            SalesViewModel vm = new SalesViewModel();
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

        #endregion

        #region RoundUp Command Tests
        [TestMethod]
        public void TestRoundUpCommand_CanExecute()
        {
            SalesViewModel vm = new SalesViewModel();
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
            SalesViewModel vm = new SalesViewModel();
            vm.CurrentEntry.SellerId = 12;
            vm.CurrentEntry.Price = 43;
            vm.EnterSaleCommand.Execute(null);
            vm.RoundUpCommand.Execute(null);

            Assert.AreEqual(vm.SumTotal, 43);
            Assert.AreEqual(vm.CurrentEntry.Price, 50 - 43);
            Assert.AreEqual(vm.CurrentEntry.SellerId, 200);
            Assert.IsFalse(vm.SellerIdFocused);
            Assert.IsTrue(vm.PriceFocused);
        }

        [TestMethod]
        public void TestRoundUpCommand_Execute_DivisibleBy50()
        {
            SalesViewModel vm = new SalesViewModel();
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

        #endregion

        #region SaveToFileCommand
        [TestMethod]
        public void TestSaveToFile_CanExecute()
        {
            SalesViewModel vm = new SalesViewModel();
            Assert.IsFalse(vm.SaveToFileCommand.CanExecute(null));

            vm.CurrentEntry.SellerId = 12;
            vm.CurrentEntry.Price = 80;
            vm.EnterSale();
            Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
        }

        [TestMethod]
        public void TestSaveToFile_Execute()
        {
            if (File.Exists(testFileName))
            {
                File.Delete(testFileName);
            }

            SalesViewModel vm = new SalesViewModel();
            vm.CurrentEntry.SellerId = 12;
            vm.CurrentEntry.Price = 80;
            vm.EnterSale();
            vm.CurrentEntry.SellerId = 15;
            vm.CurrentEntry.Price = 90;
            vm.EnterSale();
            Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
            vm.SaveToFileCommand.Execute(null);

            vm.CurrentEntry.SellerId = 20;
            vm.CurrentEntry.Price = 100;
            vm.EnterSale();
            Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
            vm.SaveToFileCommand.Execute(null);

            var entries = new SaveList();
            using (var filestream = new FileStream(testFileName, FileMode.Open))
            {
                var xmlreader = new XmlSerializer(typeof(SaveList));
                entries = (SaveList)xmlreader.Deserialize(filestream);
            }

            Assert.AreEqual(entries[0][0].SellerId, 15);
            Assert.AreEqual(entries[0][0].Price, 90);
            Assert.AreEqual(entries[0][1].SellerId, 12);
            Assert.AreEqual(entries[0][1].Price, 80);
            Assert.AreEqual(entries[1][0].SellerId, 20);
            Assert.AreEqual(entries[1][0].Price, 100);
            Assert.AreEqual(vm.SumTotal, 0);
        }

        [TestMethod]
        public void TestSaveToFile_Execute_FileExists()
        {
            if (File.Exists(testFileName))
            {
                File.Delete(testFileName);
            }
            using (var fs = new FileStream(testFileName, FileMode.Create))
            {
            }

            SalesViewModel vm = new SalesViewModel();
            vm.CurrentEntry.SellerId = 12;
            vm.CurrentEntry.Price = 80;
            vm.EnterSale();
            Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
            vm.SaveToFileCommand.Execute(null);

            var entries = new SaveList();
            using (var filestream = new FileStream(testFileName, FileMode.Open))
            {
                var xmlreader = new XmlSerializer(typeof(SaveList));
                entries = (SaveList)xmlreader.Deserialize(filestream);
            }

            Assert.AreEqual(entries[0][0].SellerId, 12);
            Assert.AreEqual(entries[0][0].Price, 80);
        }

        [TestMethod]
        public void TestSaveToFile_Execute_FileWrongFormat()
        {
            string testFirstErrorFileName = $"{Path.GetFileNameWithoutExtension(testFileName)}_error1{Path.GetExtension(testFileName)}";
            string testSecondErrorFileName = $"{Path.GetFileNameWithoutExtension(testFileName)}_error2{Path.GetExtension(testFileName)}";

            if (File.Exists(testFileName))
            {
                File.Delete(testFileName);
            }
            if (File.Exists(testFirstErrorFileName))
            {
                File.Delete(testFirstErrorFileName);
            }
            if (File.Exists(testSecondErrorFileName))
            {
                File.Delete(testSecondErrorFileName);
            }
            using (var streamWriter = new StreamWriter(testFileName))
            {
                streamWriter.WriteLine("ErrorText");
            }

            SalesViewModel vm = new SalesViewModel();
            vm.CurrentEntry.SellerId = 12;
            vm.CurrentEntry.Price = 80;
            vm.EnterSale();
            Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
            vm.SaveToFileCommand.Execute(null);

            var entries = new SaveList();
            using (var filestream = new FileStream(testFileName, FileMode.Open))
            {
                var xmlreader = new XmlSerializer(typeof(SaveList));
                entries = (SaveList)xmlreader.Deserialize(filestream);
            }

            Assert.AreEqual(entries[0][0].SellerId, 12);
            Assert.AreEqual(entries[0][0].Price, 80);

            Assert.IsTrue(File.Exists(testFirstErrorFileName));
            Assert.IsFalse(File.Exists(testSecondErrorFileName));

            File.Delete(testFileName);
            File.Copy(testFirstErrorFileName, testFileName);

            vm.CurrentEntry.SellerId = 12;
            vm.CurrentEntry.Price = 80;
            vm.EnterSale();
            Assert.IsTrue(vm.SaveToFileCommand.CanExecute(null));
            vm.SaveToFileCommand.Execute(null);

            entries = new SaveList();
            using (var filestream = new FileStream(testFileName, FileMode.Open))
            {
                var xmlreader = new XmlSerializer(typeof(SaveList));
                entries = (SaveList)xmlreader.Deserialize(filestream);
            }

            Assert.AreEqual(entries[0][0].SellerId, 12);
            Assert.AreEqual(entries[0][0].Price, 80);

            Assert.IsTrue(File.Exists(testSecondErrorFileName));
        }
        #endregion

    }
}
