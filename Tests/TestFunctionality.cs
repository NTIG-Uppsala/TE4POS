using System.Diagnostics;
using FlaUI.UIA3;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlaUI.Core;

namespace TestFunctionailty
{
    [TestClass]
    [DoNotParallelize]
    public class Tests
    {
        // Change [USERNAME] to your actual Windows username before running the tests
        private string appPath = "C:\\Users\\[USERNAME]\\source\\repos\\TE4POS\\bin\\Debug\\net9.0-windows\\TE4POS.exe";
        public required ConditionFactory cf;
        public required FlaUI.Core.Application app;
        public required Window window;

        [TestInitialize]
        public void Setup()
        {
            app = Application.Launch(appPath);
            var mainWindow = app.GetMainWindow(new UIA3Automation());
            window = mainWindow;
            cf = new ConditionFactory(new UIA3PropertyLibrary());

        }

        [TestMethod]
        public void AddThreeCoffee()
        {
            var btnElement = window.FindFirstDescendant(cf.ByAutomationId("Add_Coffee"));
            var tbElement = window.FindFirstDescendant(cf.ByAutomationId("sum"));

            var tb = tbElement.AsTextBox();
            var btn = btnElement.AsButton();

            btn.Click();
            btn.Click();
            btn.Click();

            Assert.AreEqual("147", tb.Text);
        }

        [TestMethod]
        public void AddAndRemoveCoffee()
        {
            var removeBtnElement = window.FindFirstDescendant(cf.ByAutomationId("Remove_Coffee"));
            var addBtnElement = window.FindFirstDescendant(cf.ByAutomationId("Add_Coffee"));
            var tbElement = window.FindFirstDescendant(cf.ByAutomationId("sum"));

            var tb = tbElement.AsTextBox();
            var addBtn = addBtnElement.AsButton();
            var removeBtn = removeBtnElement.AsButton();

            addBtn.Click();
            removeBtn.Click();

            Assert.AreEqual("0", tb.Text);
        }

        [TestMethod]
        public void AddAndRemoveManyCoffee()
        {
            var removeBtnElement = window.FindFirstDescendant(cf.ByAutomationId("Remove_Coffee"));
            var addBtnElement = window.FindFirstDescendant(cf.ByAutomationId("Add_Coffee"));
            var tbElement = window.FindFirstDescendant(cf.ByAutomationId("sum"));

            var tb = tbElement.AsTextBox();
            var addBtn = addBtnElement.AsButton();
            var removeBtn = removeBtnElement.AsButton();

            addBtn.Click();
            removeBtn.Click();
            addBtn.Click();
            addBtn.Click();
            addBtn.Click();
            removeBtn.Click();

            Assert.AreEqual("98", tb.Text);
        }

        [TestCleanup]
        public void Cleanup()
        {
            app.Close();
        }
    }
}
