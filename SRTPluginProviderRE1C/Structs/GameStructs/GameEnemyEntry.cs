using System.Runtime.InteropServices;

namespace SRTPluginProviderRE1C.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x1)]

    public struct GameEnemyEntry
    {
        [FieldOffset(0x0)] public ushort CurrentHP;
    }
}