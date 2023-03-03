using System;

namespace Sungaila.InlineTest
{
    /// <summary>
    /// Tests whether the expected and returned value are equal and throws an exception if the two values are not equal.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class AreEqualAttribute : InlineTestAttributeBase
    {
        /// <summary>
        /// The value to compare. This is the value the tests expects.
        /// </summary>
        public object? Expected { get; set; }

        /// <param name="parameters">An argument list for the tested method. This is an array of objects with the same number, order, and type as the parameters of the method to be tested.</param>
        /// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException">Thrown if <see cref="Expected"/> is not equal to the returned value.</exception>
        public AreEqualAttribute(params object?[] parameters) : base(parameters) { }
    }
}