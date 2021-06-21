using System.Runtime.InteropServices;

namespace SRTPluginProviderRE1C.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]

    public unsafe struct GameEnemyEntry
    {
        [FieldOffset(0x0)] public ushort CurrentHP;

        public static GameEnemyEntry AsStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(GameEnemyEntry*)pb;
            }
        }
    }
}