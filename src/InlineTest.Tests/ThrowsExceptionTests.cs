using System;
using System.Threading.Tasks;

namespace Sungaila.InlineTest.Tests
{
    public class ThrowsExceptionTests
    {
        [ThrowsException<InvalidOperationException>]
        public object? Throw()
        {
            throw new InvalidOperationException();
        }

        [ThrowsException<InvalidOperationException>]
        public object? Throw2()
        {
            throw new InvalidOperationException();
        }

        [ThrowsException<ArgumentException>("Null")]
        [ThrowsException<InvalidOperationException>]
        public object? Throw(string input)
        {
            if (input == "Null")
                throw new ArgumentException(nameof(input));

            throw new InvalidOperationException();
        }

        [ThrowsException<ArgumentException>("Null")]
        [ThrowsException<InvalidOperationException>]
        public object? Throw2(string input)
        {
            if (input == "Null")
                throw new ArgumentException(nameof(input));

            throw new InvalidOperationException();
        }

        [ThrowsException<ArgumentException>("Null")]
        public async Task<object?> ThrowAsync(string input)
        {
            await Task.CompletedTask;
            throw new ArgumentException(nameof(input));
        }

        [ThrowsException<ArgumentException>("Null")]
        public async Task<object?> Throw2Async(string input)
        {
            await Task.CompletedTask;
            throw new ArgumentException(nameof(input));
        }
    }
}