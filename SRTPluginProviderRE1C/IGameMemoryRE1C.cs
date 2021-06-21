using System;
using SRTPluginProviderRE1C.Structs;

namespace SRTPluginProviderRE1C
{
    public interface IGameMemoryRE1C
    {
        // Plugin Version Info
        string VersionInfo { get; }

        // Game Info
        string GameInfo { get; set; }

        // Player HP
        byte PlayerCurrentHealth { get; set; }

        // Player Max HP
        byte PlayerMaxHealth { get; set; }

        // Player Poison
        byte PlayerPoison { get; set; }

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
    }
}