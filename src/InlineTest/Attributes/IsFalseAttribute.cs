using System;

namespace Sungaila.InlineTest
{
    /// <summary>
    /// Tests whether the specified condition is <see langword="false"/> and throws an exception if the condition is <see langword="true"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class IsFalseAttribute : InlineTestAttributeBase
    {
        /// <param name="parameters">An argument list for the tested method. This is an array of objects with the same number, order, and type as the parameters of the method to be tested.</param>
        /// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException">Thrown if the returned value is <see langword="true"/>.</exception>
        public IsFalseAttribute(params object?[] parameters) : base(parameters) { }
    }
}