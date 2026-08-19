using System.Reflection;

namespace PaulN10V.MvxTabbedNavigation.Demo
{
    internal static class Utils
    {
        public static IEnumerable<Assembly> MergeWith(this IEnumerable<Assembly> assemblies, params Type[] types)
        {
            var result = assemblies.ToList();
            result.AddRange(types.Select(t => t.Assembly));
            return result;
        }
    }
}
