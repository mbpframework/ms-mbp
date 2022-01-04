using Hangfire;
using Microsoft.Extensions.Options;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WuhanIns.Nitrogen.BackgroundJob
{
    internal class NgJob : INgJob
    {
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly IRecurringJobManager _recurringJobs;
        private readonly IOptions<BackgroundJobModuleOptions> _options;

        public NgJob(IBackgroundJobClient backgroundJobs,
            IRecurringJobManager recurringJobs,
            IOptions<BackgroundJobModuleOptions> options)
        {
            _backgroundJobs = backgroundJobs;
            _recurringJobs = recurringJobs;
            _options = options;
        }

        public string EnqueueAsyncJob(Expression<Func<Task>> asyncAction)
        {
            return _backgroundJobs.Enqueue(asyncAction);
        }

        public string EnqueueAsyncJob<T>(Expression<Func<T, Task>> asyncAction) where T : class, new()
        {
            return _backgroundJobs.Enqueue(asyncAction);
        }

        public string EnqueueJob(Expression<Action> action)
        {
            return _backgroundJobs.Enqueue(action);
        }

        public string EnqueueJob<T>(Expression<Action<T>> action) where T : class, new()
        {
            return _backgroundJobs.Enqueue(action);
        }

        public void RecurringAsyncJob(string jobId, Expression<Func<Task>> asyncAction, Func<string> cron)
        {
            _recurringJobs.AddOrUpdate(jobId, asyncAction, cron);
        }

        public void RecurringAsyncJob(string jobId, Expression<Func<Task>> asyncAction, string cron)
        {
            _recurringJobs.AddOrUpdate(jobId, asyncAction, cron);
        }

        public void RecurringAsyncJob<T>(string jobId, Expression<Func<T, Task>> asyncAction, Func<string> cron) where T : class, new()
        {
            _recurringJobs.AddOrUpdate(jobId, asyncAction, cron);
        }

        public void RecurringAsyncJob<T>(string jobId, Expression<Func<T, Task>> asyncAction, string cron) where T : class, new()
        {
            _recurringJobs.AddOrUpdate(jobId, asyncAction, cron);
        }

        public void RecurringJob(string jobId, Expression<Action> action, Func<string> cron)
        {
            _recurringJobs.AddOrUpdate(jobId, action, cron);
        }

        public void RecurringJob(string jobId, Expression<Action> action, string cron)
        {
            _recurringJobs.AddOrUpdate(jobId, action, cron);
        }

        public void RecurringJob<T>(string jobId, Expression<Action<T>> action, Func<string> cron) where T : class, new()
        {
            _recurringJobs.AddOrUpdate(jobId, action, cron);
        }

        public void RecurringJob<T>(string jobId, Expression<Action<T>> action, string cron) where T : class, new()
        {
            _recurringJobs.AddOrUpdate(jobId, action, cron);
        }

        public string ScheduleAsyncJob(Expression<Func<Task>> asyncAction, TimeSpan? delay = null)
        {
            if (delay == null)
            {
                delay = TimeSpan.FromMinutes(_options.Value.Delay);
            }

            return _backgroundJobs.Schedule(asyncAction, delay.Value);
        }

        public string ScheduleAsyncJob<T>(Expression<Func<T, Task>> asyncAction, TimeSpan? delay = null) where T : class, new()
        {
            if (delay == null)
            {
                delay = TimeSpan.FromMinutes(_options.Value.Delay);
            }

            return _backgroundJobs.Schedule(asyncAction, delay.Value);
        }

        public string ScheduleJob(Expression<Action> action, TimeSpan? delay = null)
        {
            if (delay == null)
            {
                delay = TimeSpan.FromMinutes(_options.Value.Delay);
            }

            return _backgroundJobs.Schedule(action, delay.Value);
        }

        public string ScheduleJob<T>(Expression<Action<T>> action, TimeSpan? delay = null) where T : class, new()
        {
            if (delay == null)
            {
                delay = TimeSpan.FromMinutes(_options.Value.Delay);
            }

            return _backgroundJobs.Schedule(action, delay.Value);
        }
    }
}
