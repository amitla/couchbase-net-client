﻿ using System;
 using System.Threading;
 using Common.Logging;
 using Couchbase.Configuration;
 using Couchbase.Configuration.Server.Providers;
 using Couchbase.IO.Operations;

namespace Couchbase.Core.Buckets
{
    public class MemcachedBucket : IBucket, IConfigListener
    {
        private readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IClusterManager _clusterManager;
        private IConfigInfo _configInfo;
        private volatile bool _disposed;

         internal MemcachedBucket(IClusterManager clusterManager, string bucketName)
        {
            _clusterManager = clusterManager;
            Name = bucketName;
        }

        public string Name { get; set; }

        public IOperationResult<T> Insert<T>(string key, T value)
        {
            var keyMapper = _configInfo.GetKeyMapper(Name);
            var bucket = keyMapper.MapKey(key);
            var server = bucket.LocatePrimary();

            var operation = new SetOperation<T>(key, value, null);
            var operationResult = server.Send(operation);
            return operationResult;
        }
        
        public IOperationResult<T> Get<T>(string key)
        {
            var keyMapper = _configInfo.GetKeyMapper(Name);
            var bucket = keyMapper.MapKey(key);
            var server = bucket.LocatePrimary();

            var operation = new GetOperation<T>(key, null);
            var operationResult = server.Send(operation);
            return operationResult;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }


        void IConfigListener.NotifyConfigChanged(IConfigInfo configInfo)
        {
            Interlocked.Exchange(ref _configInfo, configInfo);
        }
    }
}
