namespace Sungaila.InlineTest.Tests
{
    public class SubClassTests
    {
        [AreEqual(Expected = 0.0d)]
        public double GetRealInstance() => 0.0d;

        [AreEqual(Expected = 0.0d)]
        public static double GetRealStatic() => 0.0d;

        public class SubClassDepth1
        {
            [AreEqual(Expected = 0.0d)]
            public double GetRealInstance() => 0.0d;

            [AreEqual(Expected = 0.0d)]
            public static double GetRealStatic() => 0.0d;

            public class SubClassDepth2
            {
                [AreEqual(Expected = 0.0d)]
                public double GetRealInstance() => 0.0d;

                [AreEqual(Expected = 0.0d)]
                public static double GetRealStatic() => 0.0d;

                public class SubClassDepth3
                {
                    [AreEqual(Expected = 0.0d)]
                    public double GetRealInstance() => 0.0d;

                    [AreEqual(Expected = 0.0d)]
                    public static double GetRealStatic() => 0.0d;

                    public class SubClassDepth4
                    {
                        [AreEqual(Expected = 0.0d)]
                        public double GetRealInstance() => 0.0d;

                        [AreEqual(Expected = 0.0d)]
                        public static double GetRealStatic() => 0.0d;
                    }
                }
            }
        }
    }
}