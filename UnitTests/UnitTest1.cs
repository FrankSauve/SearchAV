using System;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void TestAddition()
        {
            Assert.That(1+1==2);
        }
    }
}
