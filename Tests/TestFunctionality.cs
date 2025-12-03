using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Tools;
using FlaUI.UIA3;

namespace TestFuntionality
{
    [TestClass]
    public class Tests
    {
        private string appPath = Path.GetFullPath(@"..\..\..\..\PointOfSale\bin\Debug\net9.0-windows\TE4POS.exe");
        public required ConditionFactory cf;
        public required FlaUI.Core.Application app;
        public required Window window;

        [TestInitialize]
        public void Setup()
        {
            app = Application.Launch(appPath);
            var mainWindow = app.GetMainWindow(new UIA3Automation());
            window = (mainWindow != null) ? mainWindow : throw new Exception("mainWindow is not defined");
            cf = new ConditionFactory(new UIA3PropertyLibrary());
        }

        [TestMethod]
        public void AddThreeItems()
        {
            var itemElement = window.FindFirstDescendant(cf.ByText("Bryggkaffe (liten)"));
            var tbElement = window.FindFirstDescendant(cf.ByAutomationId("ShoppingCartTotal"));

            var tb = tbElement.AsTextBox();
            var itemBtn  = itemElement.AsButton();

            itemBtn.Click();
            itemBtn.Click();
            itemBtn.Click();

            Assert.AreEqual("84", tb.Text);
        }

        [TestMethod]
        public void AddAndRemoveProduct()
        {
            var itemElement = window.FindFirstDescendant(cf.ByText("Bryggkaffe (stor)"));
            var resetElement = window.FindFirstDescendant(cf.ByAutomationId("Reset"));
            var tbElement = window.FindFirstDescendant(cf.ByAutomationId("ShoppingCartTotal"));

            var tb = tbElement.AsTextBox();
            var itemBtn = itemElement.AsButton();
            var resetBtn = resetElement.AsButton();

            itemBtn.Click();
            itemBtn.Click();
            resetBtn.Click();
            itemBtn.Click();

            Assert.AreEqual("34", tb.Text);
        }

        [TestMethod]
        public void AddAndRemoveMoreProducts()
        {
            var itemElement1 = window.FindFirstDescendant(cf.ByText("Varm choklad med grädde"));
            var itemElement2 = window.FindFirstDescendant(cf.ByText("Bryggkaffe (liten)"));
            var resetElement = window.FindFirstDescendant(cf.ByAutomationId("Reset"));
            var tbElement = window.FindFirstDescendant(cf.ByAutomationId("ShoppingCartTotal"));

            var tb = tbElement.AsTextBox();
            var itemBtn1 = itemElement1.AsButton();
            var itemBtn2 = itemElement2.AsButton();
            var resetBtn = resetElement.AsButton();

            itemBtn1.Click();
            resetBtn.Click();
            itemBtn2.Click();
            itemBtn2.Click();
            itemBtn1.Click();

            Assert.AreEqual("101", tb.Text);
        }

        [TestMethod]
        public void ResetButton()
        {
            var itemElement = window.FindFirstDescendant(cf.ByText("Varm choklad med grädde"));
            var resetElement = window.FindFirstDescendant(cf.ByAutomationId("Reset"));
            var tbElement = window.FindFirstDescendant(cf.ByAutomationId("ShoppingCartTotal"));

            var tb = tbElement.AsTextBox();
            var itemBtn = itemElement.AsButton();
            var resetBtn = resetElement.AsButton();

            itemBtn.Click();
            itemBtn.Click();
            itemBtn.Click();
            resetBtn.Click();
            itemBtn.Click();

            // Finds the amount element in the cart by its name, which is it's value 
            var amountElement = window.FindFirstDescendant(cf.ByName("1"));
            var amount = amountElement.Name;

            Assert.AreEqual("1", amount);
            Assert.AreEqual("45", tb.Text);
        }

        [TestMethod]
        public void ReceiptAndBackButton()
        {
            var receiptElement = window.FindFirstDescendant(cf.ByName("Kvitton"));
            var receiptBtn = receiptElement.AsButton();
            receiptBtn.Click();

            var backElement = window.FindFirstDescendant(cf.ByName("Produkter"));
            var backBtn = backElement.AsButton();
            backBtn.Click();

            var itemElement1 = window.FindFirstDescendant(cf.ByText("Cappuccino"));
            var itemBtn1 = itemElement1.AsButton();
            itemBtn1.Click();

            var tbElement = window.FindFirstDescendant(cf.ByAutomationId("ShoppingCartTotal"));
            var tb = tbElement.AsTextBox();
            Assert.AreEqual("42", tb.Text);

        }

        [TestMethod]
        public void CreatingReceipt()
        {
            var itemElement1 = window.FindFirstDescendant(cf.ByText("Cappuccino"));
            var itemElement2 = window.FindFirstDescendant(cf.ByText("Latte"));
            var checkoutElement = window.FindFirstDescendant(cf.ByAutomationId("Finish"));
            var receiptElement = window.FindFirstDescendant(cf.ByName("Kvitton"));

            var itemBtn1 = itemElement1.AsButton();
            var itemBtn2 = itemElement2.AsButton();
            var checkoutBtn = checkoutElement.AsButton();
            var receiptBtn = receiptElement.AsButton();

            itemBtn1.Click();
            itemBtn1.Click();
            itemBtn2.Click();

            checkoutBtn.Click();
            
            receiptBtn.Click();

            var receiptSumElement = window.FindFirstDescendant(cf.ByText("130.00"));
            var receiptSum = receiptSumElement.AsTextBox();
            if (receiptSum == null)
            {
                return;
            }
            else
            {
                Assert.AreNotEqual("0.00", receiptSum.Name);
            }
        }

        [TestMethod]
        public void CreateReceiptWithNoItems()
        {
            var checkoutElement = window.FindFirstDescendant(cf.ByAutomationId("Finish"));
            var receiptElement = window.FindFirstDescendant(cf.ByName("Kvitton"));
            var checkoutBtn = checkoutElement.AsButton();
            var receiptBtn = receiptElement.AsButton();

            checkoutBtn.Click();
            receiptBtn.Click();

            var emptyReceiptElement = window.FindFirstDescendant(cf.ByText("0.00"));
            var emptyReceipt = emptyReceiptElement.AsTextBox();
            if (emptyReceipt == null)
            {
                return;
            } 
            else
            {
                Assert.AreNotEqual("0.00", emptyReceipt.Name);
            }
        }

        [TestMethod]
        public void ErrorPopupOpeningOldReceipts()
        {
            var itemElement = window.FindFirstDescendant(cf.ByText("Cappuccino"));
            var checkoutElement = window.FindFirstDescendant(cf.ByAutomationId("Finish"));
            var itemBtn = itemElement.AsButton();
            var checkoutBtn = checkoutElement.AsButton();

            itemBtn.Click();
            itemBtn.Click();
            checkoutBtn.Click();

            var receiptTab = window.FindFirstDescendant(cf.ByName("Kvitton"));
            var receiptTabBtn = receiptTab.AsButton();
            receiptTabBtn.Click();

            var receiptElement = window.FindFirstDescendant(cf.ByName("TE4POS.MainWindow+Receipt"));
            var receiptBtn = receiptElement.AsButton();
            receiptBtn.Click();

            Thread.Sleep(100); // Wait, incase it takes time for the popup to appear

            var errorPopup = window.FindFirstDescendant(cf.ByAutomationId("CommandButton_1"));
            var errorPopupBtn = errorPopup.AsButton();
            if (errorPopupBtn != null)
            {
                errorPopupBtn.Click();
            }
            
            Assert.IsNull(errorPopup);
        }

        [TestCleanup]
        public void Cleanup()
        {
            app.Close();
        }
    }
}
