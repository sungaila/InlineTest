namespace Sungaila.InlineTest.Tests
{
    public struct StructTests
    {
        // structs must have a parameterless constructor
        // so this dummy constructor should be fine
        public StructTests(object? something) { }

        [AreEqual("Moin Moin", Expected = "Moin Moin")]
        public string? EchoValueInstance(string input) => input;

        [AreEqual("Moin Moin", Expected = "Moin Moin")]
        public static string? EchoValueStatic(string input) => input;

        public struct SubStructDepth1
        {
            [AreEqual("Moin Moin", Expected = "Moin Moin")]
            public string? EchoValueInstance(string input) => input;

            [AreEqual("Moin Moin", Expected = "Moin Moin")]
            public static string? EchoValueStatic(string input) => input;

            public struct SubStructDepth2
            {
                [AreEqual("Moin Moin", Expected = "Moin Moin")]
                public string? EchoValueInstance(string input) => input;

                [AreEqual("Moin Moin", Expected = "Moin Moin")]
                public static string? EchoValueStatic(string input) => input;

                public struct SubStructDepth3
                {
                    [AreEqual("Moin Moin", Expected = "Moin Moin")]
                    public string? EchoValueInstance(string input) => input;

                    [AreEqual("Moin Moin", Expected = "Moin Moin")]
                    public static string? EchoValueStatic(string input) => input;

                    public struct SubStructDepth4
                    {
                        [AreEqual("Moin Moin", Expected = "Moin Moin")]
                        public string? EchoValueInstance(string input) => input;

                        [AreEqual("Moin Moin", Expected = "Moin Moin")]
                        public static string? EchoValueStatic(string input) => input;
                    }
                }
            }
        }
    }
}