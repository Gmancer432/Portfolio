using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;

namespace ModelTests
{
    [TestClass]
    public class ModelTests
    {
        [TestMethod]
        public void TestSettingID()
        {
            Tank t = new Tank();
            t.SetID(20);

            Assert.AreEqual(20, t.ID);

            IHasID i = t;

            Assert.AreEqual(20, i.ID);
        }
    }
}
