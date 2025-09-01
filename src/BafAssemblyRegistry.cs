using System.Reflection;

namespace Avolutions.Baf.Core;

public static class BafAssemblyRegistry
{
    private static readonly HashSet<Assembly> Assemblies = [];

    public static void Register(Assembly asm) => Assemblies.Add(asm);

    public static IEnumerable<Assembly> GetAssemblies()
    {
        yield return typeof(AssemblyMarker).Assembly;

        foreach (var asm in Assemblies)
        {
            yield return asm;
        }
    }
}
