using System;

namespace Sungaila.InlineTest
{
    /// <summary>
    /// Tests whether the expected and returned value are unequal and throws an exception if the two values are equal.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class AreNotEqualAttribute : InlineTestAttributeBase
    {
        /// <summary>
        /// The value to compare. This is the value the test expects not to match.
        /// </summary>
        public object? NotExpected { get; set; }

        /// <param name="parameters">An argument list for the tested method. This is an array of objects with the same number, order, and type as the parameters of the method to be tested.</param>
        /// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException">Thrown if <see cref="NotExpected"/> is equal to the returned value.</exception>
        public AreNotEqualAttribute(params object?[] parameters) : base(parameters) { }
    }
}