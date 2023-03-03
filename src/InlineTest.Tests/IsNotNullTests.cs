using System.Threading.Tasks;

namespace Sungaila.InlineTest.Tests
{
    public class IsNotNullTests
    {
        [IsNotNull]
        public object? GetNotNull()
        {
            return "Something";
        }

#nullable disable
        [IsNotNull("Null")]
        public object GetNotNull(string input)
        {
            if (input == "Null")
                return "Something";

            return null;
        }
#nullable enable

        [IsNotNull]
        public static object? GetNotNull2()
        {
            return "Something";
        }

        [IsNotNull("Null")]
        public static object? GetNotNull2(string input)
        {
            if (input == "Null")
                return "Something";

            return null;
        }

        [IsNotNull("Null")]
        public async Task<object?> GetNotNullAsync(string input)
        {
            await Task.CompletedTask;

            if (input == "Null")
                return "Something";

            return null;
        }

        [IsNotNull("Null")]
        public async static Task<object?> GetNotNullAsync2(string input)
        {
            await Task.CompletedTask;

            if (input == "Null")
                return "Something";

            return null;
        }
    }
}