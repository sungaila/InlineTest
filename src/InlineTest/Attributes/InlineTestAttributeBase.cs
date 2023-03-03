using System;

namespace Sungaila.InlineTest
{
    /// <summary>
    /// Base class for all InlineTest attributes.
    /// </summary>
    public abstract class InlineTestAttributeBase : Attribute
    {
        /// <summary>
        /// An argument list for the tested method. This is an array of objects with the same number, order, and type as the parameters of the method to be tested.
        /// </summary>
        public object?[] Parameters { get; }

        /// <param name="parameters">An argument list for the tested method. This is an array of objects with the same number, order, and type as the parameters of the method to be tested.</param>
        public InlineTestAttributeBase(params object?[] parameters)
        {
            Parameters = parameters;
        }
    }
}