using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MeiHi.API;
using MeiHi.Model;
using MeiHi.API.Controllers;

namespace MeiHi.API.UnitTest
{
    [TestClass]
    public class ShopControllerTest
    {
        [TestMethod]
        public void CalDistanceTest()
        {
            new ShopController().CalDistance("70.424355,31.221123");
        }
    }
}
