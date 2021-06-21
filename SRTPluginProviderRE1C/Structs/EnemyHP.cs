using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SRTPluginProviderRE1C.Structs
{
    [DebuggerDisplay("{_DebuggerDisplay,nq}")]
    public struct EnemyHP
    {
        /// <summary>
        /// Debugger display message.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string _DebuggerDisplay
        {
            get
            {
                if (IsAlive)
                {
                    return string.Format("Enemy Current HP: {0}", CurrentHP);
                }
                return "DEAD";
            }
        }
        public ushort CurrentHP { get => _currentHP; }
        internal ushort _currentHP;
        public bool IsTrigger => CurrentHP == 0xFFFF;
        public bool IsAlive => !IsTrigger && CurrentHP > 0;
    }
}