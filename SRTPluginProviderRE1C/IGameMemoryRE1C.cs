using System;
using SRTPluginProviderRE1C.Structs;
using SRTPluginProviderRE1C.Structs.GameStructs;

namespace SRTPluginProviderRE1C
{
    public interface IGameMemoryRE1C
    {
        string GameName { get; }
        // Plugin Version Info
        string VersionInfo { get; }

        // Game Info
        string GameInfo { get; set; }

        // Player HP
        GamePlayer Player { get; set; }

        // IGT
        int IGT { get; set; }

        // Current Weapon
        byte CurrentWeapon { get; set; }

        // Enemy HP Array
        EnemyHP[] EnemyHealth { get; set; }

        // Inventory Item Array
        InventoryEntry[] PlayerInventory { get; set; }

        // Box Inventory Item Array
        InventoryEntry[] BoxInventory { get; set; }
        TimeSpan IGTTimeSpan { get; }

        string IGTFormattedString { get; }
    }
}