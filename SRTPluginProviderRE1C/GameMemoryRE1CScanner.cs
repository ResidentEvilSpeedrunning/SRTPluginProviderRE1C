using ProcessMemory;
using static ProcessMemory.Extensions;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SRTPluginProviderRE1C.Structs;
using System.Text;

namespace SRTPluginProviderRE1C
{
    internal class GameMemoryRE1CScanner : IDisposable
    {
        private static readonly int MAX_ENTITIES = 64;
        private static readonly int MAX_ITEMS = 256;

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
        // Pointer Classes
        private IntPtr BaseAddress { get; set; }
        private MultilevelPointer PointerPlayerHP { get; set; }
        private MultilevelPointer PointerPlayerPoison { get; set; }
        private MultilevelPointer PointerPlayerHPMax { get; set; }
        private MultilevelPointer PointerPlayerIGT { get; set; }
        private MultilevelPointer PointerPlayerCW { get; set; }
        private MultilevelPointer[] PointerPlayerInv { get; set; }
        
        internal GameMemoryRE1CScanner(Process process = null)
        {
            gameMemoryValues = new GameMemoryRE1C();
            if (process != null)
                Initialize(process);
        }

        internal unsafe void Initialize(Process process)
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

                // Setup the pointers.
                PointerPlayerHP = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressHP));
                PointerPlayerPoison = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressPoison));
                PointerPlayerHPMax = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressHPMAX));
                PointerPlayerIGT = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressIGT));
                PointerPlayerCW = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressCW));

                //PointerPlayerInv = new MultilevelPointer[8];
                //for (int i = 0; i < PointerPlayerInv.Length; ++i)
                //    PointerPlayerInv[i] = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressInv), 0x0 + (i * 2));

                PointerPlayerInv = new MultilevelPointer[8];
                for (int i = 0; i < PointerPlayerInv.Length; ++i)
                    PointerPlayerInv[i] = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressInv));
            }
        }

        private void SelectPointerAddresses()
        {
            pointerAddressHP = 0x83523C;
            pointerAddressInv = 0x838814;
            pointerAddressPoison = 0x835290;
            pointerAddressHPMAX = 0x835329;
            pointerAddressIGT = 0xAC4B4;
            pointerAddressCW = 0x8351B6;
        }


        internal void UpdatePointers()
        {
            PointerPlayerHP.UpdatePointers();
            PointerPlayerPoison.UpdatePointers();
            PointerPlayerHPMax.UpdatePointers();
            PointerPlayerIGT.UpdatePointers();
            PointerPlayerCW.UpdatePointers();

            // InventoryEntries
            for(int i = 0; i < 8; ++i)
                PointerPlayerInv[i].UpdatePointers();
        }

        internal unsafe IGameMemoryRE1C Refresh()
        {
            bool success;

            // Player HP
            fixed (byte* p = &gameMemoryValues._playerHP)
                success = memoryAccess.TryGetByteAt(IntPtr.Add(BaseAddress, pointerAddressHP), p);

            // Player Max HP
            fixed (byte* p = &gameMemoryValues._playerMaxHP)
                success = memoryAccess.TryGetByteAt(IntPtr.Add(BaseAddress, pointerAddressHPMAX), p);

            // Player Poisoned
            fixed (byte* p = &gameMemoryValues._playerPoison)
                success = memoryAccess.TryGetByteAt(IntPtr.Add(BaseAddress, pointerAddressPoison), p);

            // IGT
            fixed (int* p = &gameMemoryValues._igt)
                success = memoryAccess.TryGetIntAt(IntPtr.Add(BaseAddress, pointerAddressIGT), p);

            // Current Weapon
            fixed (byte* p = &gameMemoryValues._currentWeapon)
                success = memoryAccess.TryGetByteAt(IntPtr.Add(BaseAddress, pointerAddressCW), p);

            // Player Inventory
            if (gameMemoryValues._invItem == null)
            {
                gameMemoryValues._invItem = new InventoryEntry[8];
                for (int i = 0; i < gameMemoryValues._invItem.Length; ++i)
                    gameMemoryValues._invItem[i] = new InventoryEntry();
            }
            for (int i = 0; i < gameMemoryValues._invItem.Length; ++i)
            {
                try
                {
                    fixed (byte* p = &gameMemoryValues.InvItem[i]._playerInv)
                        memoryAccess.TryGetByteAt(IntPtr.Add(BaseAddress, pointerAddressInv + (i * 0x2)), p);
                    fixed (byte* p = &gameMemoryValues.InvItem[i]._playerAmmo)
                        memoryAccess.TryGetByteAt(IntPtr.Add(BaseAddress, pointerAddressInv + 1 + (i * 0x2)), p);
                }
                catch
                {}
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