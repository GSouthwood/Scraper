using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SurfScraper.UtilityMethods;

namespace SurfScraperTests
{
    [TestClass]
    public class UnitTest1
    {
        
        [TestMethod]
        public void PercentChecker()
        {
            
            double result = Percent.DataAvailableVsPercentScraped(50, 0, 50);
            Assert.AreEqual(100, result);

            result = Percent.DataAvailableVsPercentScraped(50, 25, 50);
            Assert.AreEqual(50, result);

            result = Percent.DataAvailableVsPercentScraped(50, 2, 50);
            Assert.AreEqual(96, result);

            result = Percent.DataAvailableVsPercentScraped(80, 40, 80);
            Assert.AreEqual(50, result);
        }
    }
}
