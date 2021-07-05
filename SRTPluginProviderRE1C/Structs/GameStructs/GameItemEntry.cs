using System.Runtime.InteropServices;

namespace SRTPluginProviderRE1C.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x2)]

    public struct GameItemEntry
    {
        [FieldOffset(0x0)] public byte ItemId;
        [FieldOffset(0x1)] public byte StackSize;
    }
}