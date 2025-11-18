using FlaUI.UIA3;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core;

namespace TestFunctionailty
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
            var itemElement = window.FindFirstDescendant(cf.ByText("Mineralvatten"));
            var resetElement = window.FindFirstDescendant(cf.ByAutomationId("Reset"));
            var tbElement = window.FindFirstDescendant(cf.ByAutomationId("ShoppingCartTotal"));

            var tb = tbElement.AsTextBox();
            var itemBtn = itemElement.AsButton();
            var resetBtn = resetElement.AsButton();

            itemBtn.Click();
            itemBtn.Click();
            resetBtn.Click();
            itemBtn.Click();

            Assert.AreEqual("20", tb.Text);
        }

        [TestMethod]
        public void AddAndRemoveMoreProducts()
        {
            var itemElement1 = window.FindFirstDescendant(cf.ByText("Panini (kyckling & pesto)"));
            var itemElement2 = window.FindFirstDescendant(cf.ByText("Cheesecake (bit)"));
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

            Assert.AreEqual("142", tb.Text);
        }

        [TestMethod]
        public void CheckAmountReset()
        {
            var itemElement = window.FindFirstDescendant(cf.ByText("Panini (kyckling & pesto)"));
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
            Assert.AreEqual("58", tb.Text);
        }

        [TestCleanup]
        public void Cleanup()
        {
            app.Close();
        }
    }
}
