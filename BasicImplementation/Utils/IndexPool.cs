using SyncUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerImplementation.Utils
{
    public class IndexPool
    {
        private readonly SortedList<int, int> _pool = new SortedList<int, int>(); 
      
        public IndexPool(int start = 0)
        {
            _pool.Add(start, start);
        }

        public PoolIndex GetNext()
        {
            int id = _pool.First().Value;

            _pool.RemoveAt(0);
            if(_pool.Count == 0)
            {
                int next = id+1;
                _pool.Add(next, next);
            }

            return new PoolIndex(this, id);
        }
        
        private void ReturnIndex(int i)
        {
            _pool.Add(i, i);
        }
        public class PoolIndex : IDisposable
        {
            private IndexPool _pool;
            public int Value { get; }

            private readonly SemaphoreLock _lock = new();
           // private readonly AsyncMutex _mutex = new AsyncMutex();

            public PoolIndex(IndexPool pool, int index)
            {
                _pool = pool ?? throw new ArgumentNullException(nameof(pool));
                Value = index;
            }

            ~PoolIndex()
            {
                ReturnIndex().Wait();
            }

            private async Task ReturnIndex()
            {
                using (var l = await _lock.LockAsync())
                {
                    if (_pool != null)
                    {
                        _pool.ReturnIndex(Value);
                        _pool = null;
                    }
                }
            }

            public async void Dispose()
            {
                 await ReturnIndex();
            }
        }
    }

}
