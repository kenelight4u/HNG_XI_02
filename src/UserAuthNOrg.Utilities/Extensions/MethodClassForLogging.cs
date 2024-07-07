using System.Runtime.CompilerServices;

namespace UserAuthNOrg.Utilities.Extensions
{
    public static class MethodClassForLogging
    {
        public static string MethodName([CallerMemberName] string callingMethodName = null)
        {
            return callingMethodName;
        }

        public static string MethodPath([CallerFilePath] string path = null)
        {
            return path;
        }
    }
}
