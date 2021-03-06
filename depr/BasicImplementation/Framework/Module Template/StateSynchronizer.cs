using SyncUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerImplementation.Framework.Module_Template
{
    public class StateSynchronizer
    {
        private readonly AsyncPulseSource _stateMutex = new AsyncPulseSource();

        private int _lockCount = 0;

        public ModuleState State { get; private set; } = ModuleState.Waiting;
        public class StateLock : IDisposable
        {
            private readonly StateSynchronizer _stateSynchronizer;

            public StateLock(StateSynchronizer stateSynchronizer)
            {
                _stateSynchronizer = stateSynchronizer;
            }

            public async void Dispose()
            {
                await _stateSynchronizer.Release();
            }
        }

        public async Task Release()
        {
            using (var token = await _stateMutex.LockAsync())
            {
                _lockCount--;
                if (_lockCount == 0)
                {
                    State = ModuleState.Waiting;
                    _stateMutex.PulseAll(token);
                }
                else if (_lockCount < 0)
                {
                    throw new Exception("Lock count can't be negative!");
                }
            }
        }

        public async Task<StateLock> SetStateAsync(ModuleState state)
        {
            using (var token = await _stateMutex.LockAsync())
            {
                while (State != ModuleState.Waiting //no wait state
                && !(state == ModuleState.Writing && State == state)) //requested and actual state are write
                {
                    await _stateMutex.WaitAsync(token);
                }
                _lockCount++;
                State = state;
                return new StateLock(this);
            }
        }
    }
}
