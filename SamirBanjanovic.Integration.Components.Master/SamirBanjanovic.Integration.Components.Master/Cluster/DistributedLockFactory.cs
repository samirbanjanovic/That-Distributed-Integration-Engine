using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Medallion.Threading.Sql;
using Microsoft.Extensions.Configuration;
using OnTrac.Integration.Components.Master.Node.Data.Entities;

namespace OnTrac.Integration.Components.Master.Cluster
{
    public class DistributedLockFactory
    {
        private const string BASE_LOCK_NAME_TEMPLATE = "OnTrac.Integration.Distributed.Master.";

        private readonly IConfiguration _configuration;
        private readonly string _lockDatabaseConnectionString;
        private readonly int _lockAcquireTimeoutMs;        

        public DistributedLockFactory(IConfiguration configuration)
        {
            _configuration = configuration;
            _lockDatabaseConnectionString = configuration["Configuration:Master:LockDatabase"];
            _lockAcquireTimeoutMs = int.Parse(configuration["Configuration:Master:LockAcquireTimeoutMs"] ?? "0");
            
        }

        public async Task<IDisposable> TryAcquireLockAsync(string resourceName)
        {
            return await InternalTryAcquireLockAsync(resourceName);
        }

        public async Task<IDisposable> TryAcquireLockAsync()
        {
            return await InternalTryAcquireLockAsync();
        }

        private async Task<IDisposable> InternalTryAcquireLockAsync(string lockNameExtension = null)
        {
            var baseName = $"{BASE_LOCK_NAME_TEMPLATE}{lockNameExtension ?? "Cluster"}";
            var safeName = SqlDistributedLock.GetSafeLockName(baseName);
            var distributedLock = new SqlDistributedLock(safeName, _lockDatabaseConnectionString);
            var timeout = new TimeSpan(0, 0, 0, 0, _lockAcquireTimeoutMs);

            return await distributedLock.TryAcquireAsync(timeout);
        }
    }
}
