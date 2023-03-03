using System.Threading.Tasks;

namespace Sungaila.InlineTest.Tests
{
    public class IsInstanceOfTypeTests
    {
        [IsInstanceOfType<int>]
        public object? GetInt()
        {
            return 42;
        }

#nullable disable
        [IsInstanceOfType<string>("NOP")]
        public object GetString(string input)
        {
            return "Something";
        }

        [IsInstanceOfType<string>("NOP")]
        public string GetString2(string input)
        {
            return "Something";
        }
#nullable enable

        [IsInstanceOfType<int>("Null")]
        [IsInstanceOfType<string>("Something")]
        public object? GetNull2(string input)
        {
            if (input == "Null")
                return 42;

            return "Something";
        }

        [IsInstanceOfType<int>("Null")]
        [IsInstanceOfType<string>("Something")]
        public static object? GetNull3(string input)
        {
            if (input == "Null")
                return 42;

            return "Something";
        }

        [IsInstanceOfType<int>("Null")]
        [IsInstanceOfType<string>("Something")]
        public async Task<object?> GetNull2Async(string input)
        {
            await Task.CompletedTask;

            if (input == "Null")
                return 42;

            return "Something";
        }

        [IsInstanceOfType<int>("Null")]
        [IsInstanceOfType<string>("Something")]
        public async static Task<object?> GetNull3Async(string input)
        {
            await Task.CompletedTask;

            if (input == "Null")
                return 42;

            return "Something";
        }
    }
}