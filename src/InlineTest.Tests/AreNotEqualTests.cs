using System.Threading.Tasks;

namespace Sungaila.InlineTest.Tests
{
    public class AreNotEqualTests
    {
        [AreNotEqual]
        [AreNotEqual(NotExpected = default)]
        [AreNotEqual(NotExpected = default(string?))]
        [AreNotEqual(NotExpected = null)]
        [AreNotEqual(NotExpected = (string?)null)]
        public string? GetSomething()
        {
            return "Yes";
        }

        [AreNotEqual]
        [AreNotEqual(NotExpected = default)]
        [AreNotEqual(NotExpected = default(string?))]
        [AreNotEqual(NotExpected = null)]
        [AreNotEqual(NotExpected = (string?)null)]
        public static string? GetSomething2()
        {
            return "Yes";
        }

        [AreNotEqual(NotExpected = "Servus")]
        public string GetHello()
        {
            return "Hello";
        }

        [AreNotEqual(NotExpected = "Servus")]
        public static string GetHello2()
        {
            return "Hello";
        }

        [AreNotEqual((string?)null, NotExpected = "")]
        [AreNotEqual("Hello", NotExpected = "Servus")]
        [AreNotEqual("Bye", NotExpected = "Ciao")]
        public string GetItself(string input)
        {
            return input;
        }

        [AreNotEqual((string?)null, NotExpected = "")]
        [AreNotEqual("Hello", NotExpected = "Servus")]
        [AreNotEqual("Bye", NotExpected = "Ciao")]
        public static string GetItself2(string input)
        {
            return input;
        }

        [AreNotEqual((string?)null, NotExpected = "")]
        [AreNotEqual("Hello", NotExpected = "Servus")]
        [AreNotEqual("Bye", NotExpected = "Ciao")]
        public async Task<string> GetItselfAsync(string input)
        {
            await Task.CompletedTask;
            return input;
        }

        [AreNotEqual((string?)null, NotExpected = "")]
        [AreNotEqual("Hello", NotExpected = "Servus")]
        [AreNotEqual("Bye", NotExpected = "Ciao")]
        public static async Task<string> GetItselfAsync2(string input)
        {
            await Task.CompletedTask;
            return input;
        }
    }
}