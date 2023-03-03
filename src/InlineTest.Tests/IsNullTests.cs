using System.Threading.Tasks;

namespace Sungaila.InlineTest.Tests
{
    public class IsNullTests
    {
        [IsNull]
        public object? GetNull()
        {
            return null;
        }

#nullable disable
        [IsNull("Null")]
        public object GetNull(string input)
        {
            if (input == "Null")
                return null;

            return "Something";
        }
#nullable enable

        [IsNull]
        public static object? GetNull2()
        {
            return null;
        }

        [IsNull("Null")]
        public static object? GetNull2(string input)
        {
            if (input == "Null")
                return null;

            return "Something";
        }

        [IsNull("Null")]
        public async Task<object?> GetNullAsync(string input)
        {
            await Task.CompletedTask;

            if (input == "Null")
                return null;

            return "Something";
        }

        [IsNull("Null")]
        public async static Task<object?> GetNullAsync2(string input)
        {
            await Task.CompletedTask;

            if (input == "Null")
                return null;

            return "Something";
        }
    }
}