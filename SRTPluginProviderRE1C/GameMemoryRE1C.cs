using System;
using System.Globalization;
using System.Runtime.InteropServices;
using SRTPluginProviderRE1C.Structs;
using System.Diagnostics;
using System.Reflection;

namespace SRTPluginProviderRE1C
{
    public class GameMemoryRE1C : IGameMemoryRE1C
    {   
        // Player HP
        public byte PlayerHP { get => _playerHP; set => _playerHP = value;  }
        internal byte _playerHP;

        // Player Max HP
        public byte PlayerMaxHP { get => _playerMaxHP; set => _playerMaxHP = value; }
        internal byte _playerMaxHP;

        // Poisoned
        public byte PlayerPoison { get => _playerPoison; set => _playerPoison = value; }
        internal byte _playerPoison;

        // IGT
        public int IGT { get => _igt; set => _igt = value; }
        internal int _igt;

        // Equipped Weapon
        public byte CurrentWeapon { get => _currentWeapon; set => _currentWeapon = value; }
        internal byte _currentWeapon;

        // Versioninfo
        public string VersionInfo => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

        // GameInfo
        public string GameInfo { get =>_gameInfo; set => _gameInfo = value; }
        internal string _gameInfo;
        
        // Inventory Entry
        public InventoryEntry[] InvItem { get => _invItem; set => _invItem = value; }
        internal InventoryEntry[] _invItem;
    }
}
