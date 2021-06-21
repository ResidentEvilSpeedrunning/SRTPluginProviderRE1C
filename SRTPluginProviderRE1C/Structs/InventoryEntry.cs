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
                if (IsItem)
                {
                    return string.Format("ID: {0} | Name: {1} | Quantity: {2}", ItemID, ItemID.ToString(), Quantity);
                }
                return "Empty Slot";
            }
        }

        public ItemEnumeration ItemID { get => (ItemEnumeration)_itemID; }
        internal byte _itemID;
        public string ItemName => ItemID.ToString();
        public byte Quantity { get => _quantity; }
        internal byte _quantity;

        public bool IsItem => Enum.IsDefined(typeof(ItemEnumeration), _itemID);
    }
}
