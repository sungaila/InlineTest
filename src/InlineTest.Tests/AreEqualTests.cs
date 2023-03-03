using System.Threading.Tasks;

namespace Sungaila.InlineTest.Tests
{
    public class AreEqualTests
    {
        [AreEqual]
        [AreEqual(Expected = default)]
        [AreEqual(Expected = default(string?))]
        [AreEqual(Expected = null)]
        [AreEqual(Expected = (string?)null)]
        public string? GetNull()
        {
            return null;
        }

        [AreEqual]
        [AreEqual(Expected = default)]
        [AreEqual(Expected = default(string?))]
        [AreEqual(Expected = null)]
        [AreEqual(Expected = (string?)null)]
        public static string? GetNull2()
        {
            return null;
        }

        [AreEqual(Expected = "Hello")]
        public string GetHello()
        {
            return "Hello";
        }

        [AreEqual(Expected = "Hello")]
        public static string GetHello2()
        {
            return "Hello";
        }

        [AreEqual((string?)null, Expected = null)]
        [AreEqual("Hello", Expected = "Hello")]
        [AreEqual("Bye", Expected = "Bye")]
        public string GetItself(string input)
        {
            return input;
        }

        [AreEqual((string?)null, Expected = null)]
        [AreEqual("Hello", Expected = "Hello")]
        [AreEqual("Bye", Expected = "Bye")]
        public static string GetItself2(string input)
        {
            return input;
        }

        [AreEqual((string?)null, Expected = null)]
        [AreEqual("Hello", Expected = "Hello")]
        [AreEqual("Bye", Expected = "Bye")]
        public async Task<string> GetItselfAsync(string input)
        {
            await Task.CompletedTask;
            return input;
        }

        [AreEqual((string?)null, Expected = null)]
        [AreEqual("Hello", Expected = "Hello")]
        [AreEqual("Bye", Expected = "Bye")]
        public static async Task<string> GetItselfAsync2(string input)
        {
            await Task.CompletedTask;
            return input;
        }
    }
}