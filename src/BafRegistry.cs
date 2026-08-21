using System.Reflection;
using Avolutions.Baf.Core.Module.Abstractions;

namespace Avolutions.Baf.Core;

/// <summary>
/// Internal catalog storing all discovered modules, the assemblies that contain them,
/// and every assembly that was scanned during startup.
/// </summary>
internal sealed record BafRegistry(
    IFeatureModule[] Modules,
    Assembly[] ModuleAssemblies,
    Assembly[] ScannedAssemblies);