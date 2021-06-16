using SRTPluginBase;
using System;

namespace SRTPluginProviderRE1C
{
    internal class PluginInfo : IPluginInfo
    {
        public string Name => "Game Memory Provider (Resident Evil 1 Classic)";

        public string Description => "A game memory provider plugin for Resident Evil 1 Classic.";

        public string Author => "Mysterion_06_ (Pointers & Coding) & Squirrelies (Provider of the SRTHost)";

        public Uri MoreInfoURL => new Uri("https://github.com/ResidentEvilSpeedrunning/SRTPluginProviderRE1C");

        public int VersionMajor => assemblyFileVersion.ProductMajorPart;

        public int VersionMinor => assemblyFileVersion.ProductMinorPart;

        public int VersionBuild => assemblyFileVersion.ProductBuildPart;

        public int VersionRevision => assemblyFileVersion.ProductPrivatePart;

        private System.Diagnostics.FileVersionInfo assemblyFileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
    }
}
