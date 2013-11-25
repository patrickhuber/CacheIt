using Microsoft.Ted.Wacel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.Wacel.Cache
{
    public class ObjectCacheDataProvider : IDataProvider
    {
        public IEnumerable<KeyValuePair<string, object>> BulkGet(List<string> keys, string partition = null)
        {
            throw new NotImplementedException();
        }

        public void BulkPut(IEnumerable<KeyValuePair<string, object>> values, string partition = null)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public IDataProvider Clone(string newName)
        {
            throw new NotImplementedException();
        }

        public DataProviderConfiguration Configuration
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Delete(string key, string partition = null)
        {
            throw new NotImplementedException();
        }

        public object Get(string key, string partition = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll(string partition = null)
        {
            throw new NotImplementedException();
        }

        public void Put(string key, object value, string partition = null)
        {
            throw new NotImplementedException();
        }

        public void Update(string key, object value, string partition = null)
        {
            throw new NotImplementedException();
        }
    }
}
