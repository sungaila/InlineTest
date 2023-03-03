using System.Threading.Tasks;

namespace Sungaila.InlineTest.Tests
{
    public class IsFalseTests
    {
        [IsFalse]
        public bool GetFalse()
        {
            return false;
        }

        [IsFalse("No")]
        public bool GetFalse(string input)
        {
            if (input == "No")
                return false;

            return true;
        }

        [IsFalse]
        public static bool GetFalse2()
        {
            return false;
        }

        [IsFalse("No")]
        public static bool GetFalse2(string input)
        {
            if (input == "No")
                return false;

            return true;
        }

        [IsFalse("No")]
        public async Task<bool> GetFalseAsync(string input)
        {
            await Task.CompletedTask;

            if (input == "No")
                return false;

            return true;
        }

        [IsFalse("No")]
        public async static Task<bool> GetFalseAsync2(string input)
        {
            await Task.CompletedTask;

            if (input == "No")
                return false;

            return true;
        }
    }
}