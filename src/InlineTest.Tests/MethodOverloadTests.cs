namespace Sungaila.InlineTest.Tests
{
    public class MethodOverloadTests
    {
        [AreEqual(Expected = (string?)null)]
        public static string? EchoValue() => null;

        [AreEqual("Moin Moin", Expected = "Moin Moin")]
        public static string? EchoValue(string input) => input;

        [AreEqual("Moin Moin", Expected = "Moin Moin")]
        public static object? EchoValue(object input) => input;

        [AreEqual(1994, Expected = 1994)]
        public static int EchoValue(int input) => input;
    }
}