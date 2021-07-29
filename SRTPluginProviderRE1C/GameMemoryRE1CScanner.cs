using ProcessMemory;
using static ProcessMemory.Extensions;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SRTPluginProviderRE1C.Structs;
using System.Text;
using SRTPluginProviderRE1C.Structs.GameStructs;

namespace SRTPluginProviderRE1C
{
    internal class GameMemoryRE1CScanner : IDisposable
    {
        private static readonly int MAX_ENTITIES = 8;
        private static readonly int MAX_ITEMS = 8;
        private static readonly int MAX_BOX_ITEMS = 8;

        // Variables
        private ProcessMemoryHandler memoryAccess;
        private GameMemoryRE1C gameMemoryValues;
        public bool HasScanned;
        public bool ProcessRunning => memoryAccess != null && memoryAccess.ProcessRunning;
        public int ProcessExitCode => (memoryAccess != null) ? memoryAccess.ProcessExitCode : 0;

        // Pointer Address Variables
        private int pointerAddressHP;
        private int pointerAddressInv;
        private int pointerAddressPoison;
        private int pointerAddressHPMAX;
        private int pointerAddressIGT;
        private int pointerAddressCW;
        private int pointerAddressEnemies;
        private int pointerAddressBox;

        // Pointer Classes
        private IntPtr BaseAddress { get; set; }

        internal GameMemoryRE1CScanner(Process process = null)
        {
            gameMemoryValues = new GameMemoryRE1C();
            if (process != null)
                Initialize(process);
        }

        internal void Initialize(Process process)
        {
            if (process == null)
                return; // Do not continue if this is null.

            SelectPointerAddresses();
            gameMemoryValues._gameInfo = GameHashes.DetectVersion(process.MainModule.FileName);

            //if (!SelectPointerAddresses(GameHashes.DetectVersion(process.MainModule.FileName)))
            //    return; // Unknown version.

            int pid = GetProcessId(process).Value;
            memoryAccess = new ProcessMemoryHandler(pid);
            if (ProcessRunning)
            {
                BaseAddress = NativeWrappers.GetProcessBaseAddress(pid, PInvoke.ListModules.LIST_MODULES_64BIT); // Bypass .NET's managed solution for getting this and attempt to get this info ourselves via PInvoke since some users are getting 299 PARTIAL COPY when they seemingly shouldn't.
            }
        }

        private void SelectPointerAddresses()
        {
            pointerAddressIGT = 0xAC4B4;
            pointerAddressCW = 0x8351B6;
            pointerAddressHP = 0x83523C;
            pointerAddressPoison = 0x835290;
            pointerAddressHPMAX = 0x835329;
            pointerAddressEnemies = 0x8353BC;
            pointerAddressBox = 0x8387B4;
            pointerAddressInv = 0x838814;
        }

        internal IGameMemoryRE1C Refresh()
        {
            // Player HP
            gameMemoryValues._player = memoryAccess.GetAt<GamePlayer>(IntPtr.Add(BaseAddress, pointerAddressHP));

            if (gameMemoryValues.Player.MaxHP == 96)
            {
                gameMemoryValues.PlayerName = "Jill: ";
            }
            else
            {
                gameMemoryValues.PlayerName = "Chris: ";
            }

            // IGT
            gameMemoryValues._igt = memoryAccess.GetIntAt(IntPtr.Add(BaseAddress, pointerAddressIGT));

            // Current Weapon
            gameMemoryValues._currentWeapon = memoryAccess.GetByteAt(IntPtr.Add(BaseAddress, pointerAddressCW));

            // Enemy Entires
            if (gameMemoryValues._enemyHealth == null)
                gameMemoryValues._enemyHealth = new EnemyHP[32];

            for (int i = 0; i < gameMemoryValues._enemyHealth.Length; ++i)
            {
                GameEnemyEntry gehp = memoryAccess.GetAt<GameEnemyEntry>(IntPtr.Add(BaseAddress + pointerAddressEnemies, (i * 0x18C)));
                gameMemoryValues._enemyHealth[i]._currentHP = gehp.CurrentHP;
            }

            // Box Inventory
            //if (gameMemoryValues._boxInventory == null)
            //{
            //    gameMemoryValues._boxInventory = new InventoryEntry[MAX_ITEMS];
            //    for (int i = 0; i < gameMemoryValues._boxInventory.Length; ++i)
            //        gameMemoryValues._boxInventory[i] = new InventoryEntry();
            //}
            //for (int i = 0; i < gameMemoryValues._boxInventory.Length; ++i)
            //{
            //    try
            //    {
            //        fixed (byte* p = &gameMemoryValues._boxInventory[i]._itemID)
            //            memoryAccess.TryGetByteAt(IntPtr.Add(BaseAddress, pointerAddressBox + (i * 0x2)), p);
            //        fixed (byte* p = &gameMemoryValues._boxInventory[i]._quantity)
            //            memoryAccess.TryGetByteAt(IntPtr.Add(BaseAddress, pointerAddressBox + 1 + (i * 0x2)), p);
            //    }
            //    catch
            //    {
            //        gameMemoryValues._boxInventory[i]._itemID = 0;
            //        gameMemoryValues._boxInventory[i]._quantity = 0;
            //    }
            //}

            // Player Inventory
            if (gameMemoryValues._playerInventory == null)
                gameMemoryValues._playerInventory = new InventoryEntry[8];

            for (int i = 0; i < gameMemoryValues._playerInventory.Length; ++i)
            {
                GameItemEntry gie = memoryAccess.GetAt<GameItemEntry>(IntPtr.Add(BaseAddress + pointerAddressInv, (i * 0x2)));
                gameMemoryValues._playerInventory[i]._itemID = gie.ItemId;
                gameMemoryValues._playerInventory[i]._quantity = gie.StackSize;
            }

            HasScanned = true;
            return gameMemoryValues;
        }

        private int? GetProcessId(Process process) => process?.Id;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls


        private unsafe bool SafeReadByteArray(IntPtr address, int size, out byte[] readBytes)
        {
            readBytes = new byte[size];
            fixed (byte* p = readBytes)
            {
                return memoryAccess.TryGetByteArrayAt(address, size, p);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (memoryAccess != null)
                        memoryAccess.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~REmake1Memory() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}