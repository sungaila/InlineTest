using System;

namespace Sungaila.InlineTest
{
    /// <summary>
    /// Tests whether the returned value is <see langword="null"/> and throws an exception if it is not.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class IsNullAttribute : InlineTestAttributeBase
    {
        /// <param name="parameters">An argument list for the tested method. This is an array of objects with the same number, order, and type as the parameters of the method to be tested.</param>
        /// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException">Thrown if value is not <see langword="null"/>.</exception>
        public IsNullAttribute(params object?[] parameters) : base(parameters) { }
    }
}