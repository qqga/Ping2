namespace Ping2.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            var ex = new Exception("ex1", new Exception("ex2", new Exception("ex3")));

            var msg = ex.GetMsg();

            Assert.IsTrue(msg.Contains("ex1"));
            Assert.IsTrue(msg.Contains("ex2"));
            Assert.IsTrue(msg.Contains("ex3"));
        }
    }
}