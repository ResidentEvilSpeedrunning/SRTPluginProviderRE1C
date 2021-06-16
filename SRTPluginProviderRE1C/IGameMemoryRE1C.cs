using System;
using SRTPluginProviderRE1C.Structs;

namespace SRTPluginProviderRE1C
{
    public interface IGameMemoryRE1C
    {
        // Player HP
        byte PlayerHP { get; set; }

        // Player Max HP
        byte PlayerMaxHP { get; set; }

        // Player Poison
        byte PlayerPoison { get; set; }

        // IGT
        int IGT { get; set; }

        // Current Weapon
        byte CurrentWeapon { get; set; }

        // Versioninfo
        string VersionInfo { get; }

        // GameInfo
        string GameInfo { get; set; }
    }
}