﻿using System;

namespace Sungaila.InlineTest
{
    /// <summary>
    /// Tests whether the returned value is an instance of the generic type and throws an exception if the generic type is not in the inheritance hierarchy of the object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class IsInstanceOfTypeAttribute<T> : InlineTestAttributeBase
    {
        /// <param name="parameters">An argument list for the tested method. This is an array of objects with the same number, order, and type as the parameters of the method to be tested.</param>
        public IsInstanceOfTypeAttribute(params object?[] parameters) : base(parameters) { }
    }
}