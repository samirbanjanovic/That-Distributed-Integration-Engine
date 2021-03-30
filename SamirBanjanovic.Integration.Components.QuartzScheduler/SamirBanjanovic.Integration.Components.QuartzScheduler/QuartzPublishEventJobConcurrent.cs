using System.Threading.Tasks;
using Quartz;

namespace OnTrac.Integration.Components.QuartzScheduler
{
    internal sealed class QuartzPublishEventJobConcurrent
       : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await QuartzJobWorker.DoWork(context.JobDetail.JobDataMap);
        }
    }
}
