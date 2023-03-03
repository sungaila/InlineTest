using System.Threading.Tasks;

namespace Sungaila.InlineTest.Tests
{
    public class IsNotInstanceOfTypeTests
    {
        [IsNotInstanceOfType<bool>]
        public object? GetInt()
        {
            return 42;
        }

#nullable disable
        [IsNotInstanceOfType<int>("NOP")]
        public object GetString(string input)
        {
            return "Something";
        }

        [IsNotInstanceOfType<int>("NOP")]
        public string GetString2(string input)
        {
            return "Something";
        }
#nullable enable

        [IsNotInstanceOfType<string>("Null")]
        [IsNotInstanceOfType<int>("Something")]
        public object? GetNull2(string input)
        {
            if (input == "Null")
                return 42;

            return "Something";
        }

        [IsNotInstanceOfType<string>("Null")]
        [IsNotInstanceOfType<int>("Something")]
        public static object? GetNull3(string input)
        {
            if (input == "Null")
                return 42;

            return "Something";
        }

        [IsNotInstanceOfType<string>("Null")]
        [IsNotInstanceOfType<int>("Something")]
        public async Task<object?> GetNull2Async(string input)
        {
            await Task.CompletedTask;

            if (input == "Null")
                return 42;

            return "Something";
        }

        [IsNotInstanceOfType<string>("Null")]
        [IsNotInstanceOfType<int>("Something")]
        public async static Task<object?> GetNull3Async(string input)
        {
            await Task.CompletedTask;

            if (input == "Null")
                return 42;

            return "Something";
        }
    }
}