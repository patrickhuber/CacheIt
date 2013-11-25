using Microsoft.Ted.Wacel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.Wacel.Cache
{
    public class ObjectCacheLockProvider : ILockProvider
    {
        public void Clear()
        {
            throw new NotImplementedException();
        }

        public long Increment(string key, long increment, long defaultValue)
        {
            throw new NotImplementedException();
        }

        public void PutAndUnlock(string key, long value, object lockHandle)
        {
            throw new NotImplementedException();
        }

        public void PutInSub(string key, object value)
        {
            throw new NotImplementedException();
        }

        public long ReadAndLock(string key, long defaultValue, TimeSpan timespan, out object lockHandle)
        {
            throw new NotImplementedException();
        }

        public void Unlock(string key, object lockHandle)
        {
            throw new NotImplementedException();
        }
    }
}
