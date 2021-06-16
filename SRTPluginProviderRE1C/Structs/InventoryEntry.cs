using System.Diagnostics;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using SRTPluginProviderRE1C.Structs;
using System.Reflection;


namespace SRTPluginProviderRE1C.Structs
{
    [DebuggerDisplay("{_DebuggerDisplay,nq}")]
    public struct InventoryEntry
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string _DebuggerDisplay
        {
            get
            {
                return string.Format("ItemID: ", PlayerInv, PlayerAmmo);
            }
        }

        public byte PlayerInv { get => _playerInv;}
        internal byte _playerInv;

        public byte PlayerAmmo { get => _playerAmmo; }
        internal byte _playerAmmo;

    }
}
