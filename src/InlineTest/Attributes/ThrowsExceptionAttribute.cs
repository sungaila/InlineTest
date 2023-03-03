using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Sungaila.InlineTest
{
    /// <summary>
    /// Tests whether the method throws exact given exception of type <typeparamref name="T"/> (and not of derived type) and throws <see cref="AssertFailedException"/> if the method does not throws exception or throws exception of type other than <typeparamref name="T"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ThrowsExceptionAttribute<T> : InlineTestAttributeBase where T : Exception
    {
        /// <param name="parameters">An argument list for the tested method. This is an array of objects with the same number, order, and type as the parameters of the method to be tested.</param>
        /// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException">Thrown if the method does not throw exception of type <typeparamref name="T"/>.</exception>
        public ThrowsExceptionAttribute(params object?[] parameters) : base(parameters) { }
    }
}