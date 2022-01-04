using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WuhanIns.Nitrogen.BackgroundJob;
using Xunit;

namespace Nitrogen.BackgroundJob.Test
{
    public class BackgroundJob_Test
    {
        private IServiceCollection services = new ServiceCollection();
        private readonly INgJob _ngJob = null;

        public BackgroundJob_Test()
        {
            var backgroundjobOptions = new BackgroundJobModuleOptions_Test()
            {
                ServerName = "Nitrogen.JobServer",
                Delay = 3,
                JobStorage = new JobStorageOptions_Test()
                {
                    ConnectionString = "mongodb://instest:instest@172.18.35.33:27017/jobs?authSource=admin",
                    Provider = "mongo",
                    TablePrefix = "nitrogen.job"
                }
            };

            services.AddSingleton(Options.Create<BackgroundJobModuleOptions_Test>(backgroundjobOptions));

            var mongoUrlBuilder = new MongoUrlBuilder(backgroundjobOptions.JobStorage.ConnectionString);
            var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());
            services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseMongoStorage(mongoClient, mongoUrlBuilder.DatabaseName, new MongoStorageOptions
        {
            MigrationOptions = new MongoMigrationOptions
            {
                MigrationStrategy = new MigrateMongoMigrationStrategy(),
                BackupStrategy = new CollectionMongoBackupStrategy()
            },
            Prefix = backgroundjobOptions.JobStorage.TablePrefix,
            CheckConnection = true
        })
    );

            services.AddHangfireServer(serverOptions =>
            {
                serverOptions.ServerName = backgroundjobOptions.ServerName;
            });


            services.AddSingleton(typeof(INgJob), typeof(NgJob_Test));
            _ngJob = services.BuildServiceProvider().GetService<INgJob>();
        }

        [Fact]
        public void Enqueue_Test()
        {
            _ngJob.EnqueueJob(() => Console.WriteLine("1"));
            _ngJob.EnqueueAsyncJob(() => MethodExcute.GetName());
            _ngJob.EnqueueAsyncJob<A>((m) => m.GetName(""));
            _ngJob.EnqueueAsyncJob<A>((m) => m.GetName());
            _ngJob.EnqueueJob<A>((m) => m.GetName());
        }



        [Fact]
        public void Recurring_Test()
        {
            _ngJob.ScheduleJob(() => Console.WriteLine("1"), TimeSpan.FromSeconds(10));
            _ngJob.ScheduleJob<A>((m) => m.GetName());
            _ngJob.ScheduleAsyncJob(() => MethodExcute.GetName(), TimeSpan.FromSeconds(10));
            _ngJob.ScheduleAsyncJob<A>((m) => m.GetName());
        }

        [Fact]
        public void Schedule_Test()
        {
            _ngJob.RecurringJob("1", () => Console.WriteLine("1"), "0 0 12 * * ?");
            _ngJob.RecurringJob("2", () => Console.WriteLine("1"), () => "0 0 12 * * ?");
            _ngJob.RecurringAsyncJob("3", () => MethodExcute.GetName(), "0 0 12 * * ?");
            _ngJob.RecurringAsyncJob("4", () => MethodExcute.GetName(), () => "0 0 12 * * ?");
            _ngJob.RecurringJob<A>("5", (m) => m.GetName(), "0 0 12 * * ?");
            _ngJob.RecurringJob<A>("5", (m) => m.GetName(), () => "0 0 12 * * ?");
            _ngJob.RecurringAsyncJob<A>("4", (m) => m.GetName(), () => "0 0 12 * * ?");
            _ngJob.RecurringAsyncJob<A>("4", (m) => m.GetName(), "0 0 12 * * ?");
        }
    }

    public class A
    {
        public string Name { get; set; }

        public Task GetName(string a)
        {
            return Task.Run(() => Console.WriteLine(Name));
        }

        public Task GetName()
        {
            return Task.Run(() => Console.WriteLine(Name));
        }
    }

    public class NgJob_Test : INgJob
    {
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly IRecurringJobManager _recurringJobs;
        private readonly IOptions<BackgroundJobModuleOptions_Test> _options;

        public NgJob_Test(IBackgroundJobClient backgroundJobs,
            IRecurringJobManager recurringJobs,
            IOptions<BackgroundJobModuleOptions_Test> options)
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

    public class BackgroundJobModuleOptions_Test
    {
        /// <summary>
        /// 延迟时间
        /// </summary>
        public int Delay { get; set; }

        /// <summary>
        /// 持久化存储类型
        /// </summary>
        public JobStorageOptions_Test JobStorage { get; set; }

        /// <summary>
        /// 服务名
        /// </summary>
        public string ServerName { get; set; }
    }

    public class JobStorageOptions_Test
    {
        public string Provider { get; set; }

        public string ConnectionString { get; set; }

        public string TablePrefix { get; set; }
    }
}
