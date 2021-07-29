using System.Runtime.InteropServices;

namespace SRTPluginProviderRE1C.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0xEE)]

    public struct GamePlayer
    {
        [FieldOffset(0x0)] public byte currentHP;
        [FieldOffset(0x54)] private byte status;
        [FieldOffset(0xED)] public byte maxHP;
        public short CurrentHP => currentHP;
        public short MaxHP => maxHP;
        public float Percentage => CurrentHP > 0 ? (float)CurrentHP / (float)MaxHP : 0f;
        public bool IsAlive => CurrentHP != 0 && CurrentHP > 0 && CurrentHP <= MaxHP;
        public bool IsPoisoned => !(status % 4 == 0 || (status - 1) % 4 == 0) || (status >= 222 && status <= 247);

        public PlayerStatus HealthState
        {
            get =>
                !IsAlive ? PlayerStatus.Dead :
                IsPoisoned ? PlayerStatus.Poisoned :
                Percentage >= 0.75 ? PlayerStatus.Fine :
                Percentage >= 0.50 ? PlayerStatus.FineToo :
                Percentage >= 0.25 ? PlayerStatus.FineToo : PlayerStatus.Danger;
        }

        public string CurrentHealthState => HealthState.ToString();
    }

    public enum PlayerStatus
    {
        Dead,
        Fine,
        FineToo,
        Caution,
        Danger,
        Poisoned
    }
}
