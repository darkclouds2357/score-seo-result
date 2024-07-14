using System.Reflection;

namespace SeoMatchingService
{
    public static class Env
    {
        public readonly static Assembly[] SearchEngineAssembly = [
            Assembly.Load("SeoMatchingService.SearchEngine.Google")
        ];
    }
}