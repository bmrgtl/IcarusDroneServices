using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Threading;
using IcarusDroneServices;

namespace IcarusDroneServices.Tests
{
    [TestClass]
    public class DroneTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            // Ensure consistent formatting (currency, title case behavior)
            var culture = new CultureInfo("en-US");
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        [TestMethod]
        public void ClientName_IsConvertedToTitleCase()
        {
            var d = new Drone();
            d.ClientName = "jaNe DOE";
            Assert.AreEqual("Jane Doe", d.ClientName);
        }

        [TestMethod]
        public void ServiceProblem_IsSentenceCasedAndLowercasesRest()
        {
            var d = new Drone();
            d.ServiceProblem = "BATTERY ISSUE and sensor FAIL";
            Assert.AreEqual("Battery issue and sensor fail", d.ServiceProblem);
        }

        [TestMethod]
        public void ServiceCost_IsRoundedToTwoDecimals()
        {
            var d = new Drone();
            d.ServiceCost = 12.3456;
            Assert.AreEqual(12.35, d.ServiceCost);
        }

        [TestMethod]
        public void Display_ReturnsExpectedStringWithCurrency()
        {
            var d = new Drone
            {
                ClientName = "alice",
                ServiceCost = 5.5
            };

            // display uses $ and two decimals in en-US culture
            var expected = "Client Name: Alice | Service Cost: $5.50";
            Assert.AreEqual(expected, d.display());
            Assert.AreEqual(expected, d.ToString());
        }

        [TestMethod]
        public void ServiceTag_GetSet_Works()
        {
            var d = new Drone();
            d.ServiceTag = 123;
            Assert.AreEqual(123, d.ServiceTag);
        }
    }
}