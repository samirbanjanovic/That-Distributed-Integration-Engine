using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using TDIE.NodeApi.Data;
using TDIE.NodeApi.Data.Entities;

namespace TDIE.NodeApi.Data.LiteDb
{
    public class LiteDbProcessStoreAccess
        : IProcessStoreAccess
    {
        private readonly string _dbPath;

        public LiteDbProcessStoreAccess(string dbPath)
        {
            _dbPath = dbPath;
            CreateDbPath();
        }

        public Task DeleteProcessDetailsAsync(Guid processId)
        {
            using (GetLiteDatabaseConnection(out LiteCollection<ProcessDetails> processCollection))
            {
                processCollection.Delete(x => x.NodeProcessId == processId);

                return Task.CompletedTask;
            }
        }

        public Task<ProcessDetails> GetNodeProcessesAsync(Guid processId)
        {
            using (GetLiteDatabaseConnection(out LiteCollection<ProcessDetails> processCollection))
            {                
                return Task.FromResult(processCollection.FindOne(x => x.NodeProcessId == processId));
            }
        }


        public Task InsertProcessDetailsAsync(ProcessDetails processDetails)
        {
            using (GetLiteDatabaseConnection(out LiteCollection<ProcessDetails> processCollection))
            {
                processCollection.Insert(processDetails);

                return Task.CompletedTask;
            }
        }

        public Task<IEnumerable<ProcessDetails>> GetNodeProcessesAsync()
        {
            using (GetLiteDatabaseConnection(out LiteCollection<ProcessDetails> processCollection))
            {
                return Task.FromResult<IEnumerable<ProcessDetails>>(processCollection.FindAll().ToList());
            }
        }


        public Task<IEnumerable<ProcessDetails>> GetNodeProcessesAsync(string packageName)
        {
            using (GetLiteDatabaseConnection(out LiteCollection<ProcessDetails> processCollection))
            {
                var result = processCollection.Find(x => x.PackageName == packageName).ToList();

                return Task.FromResult<IEnumerable<ProcessDetails>>(result);
            }
        }

        public Task<IEnumerable<ProcessDetails>> GetNodeProcessesAsync(long settingsId)
        {
            using (GetLiteDatabaseConnection(out LiteCollection<ProcessDetails> processCollection))
            {
                var result = processCollection.Find(x => x.ProcessDetailsId == settingsId).ToList();

                return Task.FromResult<IEnumerable<ProcessDetails>>(result);
            }
        }

        private void CreateDbPath()
        {
            var dirPath = Path.GetDirectoryName(Path.GetFullPath(_dbPath));
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        private LiteDatabase GetLiteDatabaseConnection(out LiteCollection<ProcessDetails> processCollection)
        {
            var processDb = new LiteDatabase(_dbPath);
            processCollection = processDb.GetCollection<ProcessDetails>();
            processCollection.EnsureIndex(x => x.NodeProcessId, true);
            processCollection.EnsureIndex(x => x.PackageName, false);

            return processDb;
        }
    }
}
