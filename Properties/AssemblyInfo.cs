using MelonLoader;
using System.Reflection;

[assembly: AssemblyCopyright("Copyright © 2023 Samimies")]

[assembly: AssemblyConfiguration(BuildInfo.ModAssemblyConfiguration)]

[assembly: AssemblyFileVersion(BuildInfo.ModVersion)]
[assembly: AssemblyInformationalVersion(BuildInfo.ModVersion)]
[assembly: AssemblyVersion(BuildInfo.ModVersion)]

[assembly: AssemblyProduct(BuildInfo.ModName)]
[assembly: AssemblyTitle(BuildInfo.ModName)]

[assembly: MelonInfo(typeof(TLD_PlaneMod.PlaneMod), BuildInfo.ModName, BuildInfo.ModVersion, BuildInfo.ModAuthor)]
[assembly: MelonGame("Hinterland", "TheLongDark")]

internal static class BuildInfo
{
    internal const string ModName = "PlaneMod";
    internal const string ModAuthor = "Samimies";
    
    internal const string ModVersion = "0.0.1";

    internal const string ModAssemblyConfiguration = "Debug";
}
