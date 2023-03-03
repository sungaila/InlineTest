using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace Sungaila.InlineTest.Tests
{
    [TestClass]
    public class ReadmeExample
    {
        [AreEqual(6, 3, Expected = 2)]
        [AreEqual(1, 1, Expected = 1)]
        [AreNotEqual(10, 1, NotExpected = 42)]
        [ThrowsException<ArgumentOutOfRangeException>(1, 0)]
        public int Divide(int dividend, int divisor)
        {
            if (divisor == 0)
                throw new ArgumentOutOfRangeException(nameof(divisor));

            return dividend / divisor;
        }

        [TestMethod]
        public void CheckForGeneratedTests()
        {
            // find the generated test class
            var type = typeof(ReadmeExample).Assembly.GetType("Sungaila.InlineTest.Generated.ReadmeExampleTests", true) ?? throw new NullReferenceException();

            // there must be a TestClassAttribute
            Assert.IsNotNull(type.GetCustomAttribute<TestClassAttribute>());

            foreach (var method in type.GetMethods(BindingFlags.DeclaredOnly))
            {
                // all declared methods must have a TestMethodAttribute
                Assert.IsNotNull(method.GetCustomAttribute<TestMethodAttribute>());
            }
        }
    }
}