using System.Threading.Tasks;

namespace Sungaila.InlineTest.Tests
{
    public class IsTrueTests
    {
        [IsTrue]
        public bool GetTrue()
        {
            return true;
        }

        [IsTrue("Yes")]
        public bool GetTrue(string input)
        {
            if (input == "Yes")
                return true;

            return false;
        }

        [IsTrue]
        public static bool GetTrue2()
        {
            return true;
        }

        [IsTrue("Yes")]
        public static bool GetTrue2(string input)
        {
            if (input == "Yes")
                return true;

            return false;
        }

        [IsTrue("Yes")]
        public async Task<bool> GetTrueAsync(string input)
        {
            await Task.CompletedTask;

            if (input == "Yes")
                return true;

            return false;
        }

        [IsTrue("Yes")]
        public async static Task<bool> GetTrueAsync2(string input)
        {
            await Task.CompletedTask;

            if (input == "Yes")
                return true;

            return false;
        }
    }
}