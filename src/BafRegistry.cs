using System.Reflection;
using Avolutions.Baf.Core.Module.Abstractions;

namespace Avolutions.Baf.Core;

/// <summary>
/// Internal catalog storing all discovered modules and the assemblies that contain them.
/// </summary>
internal sealed record BafRegistry(IFeatureModule[] Modules, Assembly[] Assemblies);