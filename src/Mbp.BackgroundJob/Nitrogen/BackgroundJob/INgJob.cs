using WuhanIns.Nitrogen.Dependency;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WuhanIns.Nitrogen.BackgroundJob
{
    /// <summary>
    /// 后台作业服务
    /// </summary>
    public interface INgJob : IScopeDependency
    {
        /// <summary>
        /// 加入作业，立即执行
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        string EnqueueJob(Expression<Action> action);

        /// <summary>
        /// 加入作业，异步执行
        /// </summary>
        /// <param name="asyncAction"></param>
        /// <returns></returns>
        string EnqueueAsyncJob(Expression<Func<Task>> asyncAction);

        /// <summary>
        /// 加入作业，立即执行
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        string EnqueueJob<T>(Expression<Action<T>> action) where T : class, new();

        /// <summary>
        /// 加入作业，异步执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asyncAction"></param>
        /// <returns></returns>
        string EnqueueAsyncJob<T>(Expression<Func<T, Task>> asyncAction) where T : class, new();

        /// <summary>
        /// 加入作业，延期执行，默认延迟3分钟
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        string ScheduleJob(Expression<Action> action, TimeSpan? delay = null);

        /// <summary>
        /// 加入作业，延期执行，默认延迟3分钟
        /// </summary>
        /// <param name="asyncAction"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        string ScheduleAsyncJob(Expression<Func<Task>> asyncAction, TimeSpan? delay = null);

        /// <summary>
        /// 加入作业，延期执行，默认延迟3分钟
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        string ScheduleJob<T>(Expression<Action<T>> action, TimeSpan? delay = null) where T : class, new();

        /// <summary>
        /// 加入作业，延期执行，默认延迟3分钟
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asyncAction"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        string ScheduleAsyncJob<T>(Expression<Func<T, Task>> asyncAction, TimeSpan? delay = null) where T : class, new();

        /// <summary>
        /// 加入作业，定期执行
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="action"></param>
        /// <param name="cron"></param>
        /// <returns></returns>
        void RecurringJob(string jobId, Expression<Action> action, Func<string> cron);

        /// <summary>
        /// 加入作业，定期执行
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="action"></param>
        /// <param name="cron"></param>
        /// <returns></returns>
        void RecurringAsyncJob(string jobId, Expression<Func<Task>> asyncAction, Func<string> cron);

        /// <summary>
        /// 加入作业，定期执行
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="action"></param>
        /// <param name="cron"></param>
        /// <returns></returns>
        void RecurringJob(string jobId, Expression<Action> action, string cron);

        /// <summary>
        /// 加入作业，定期执行
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="action"></param>
        /// <param name="cron"></param>
        /// <returns></returns>
        void RecurringAsyncJob(string jobId, Expression<Func<Task>> asyncAction, string cron);

        /// <summary>
        /// 加入作业，定期执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jobId"></param>
        /// <param name="action"></param>
        /// <param name="cron"></param>
        /// <returns></returns>
        void RecurringJob<T>(string jobId, Expression<Action<T>> action, Func<string> cron) where T : class, new();

        /// <summary>
        /// 加入作业，定期执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jobId"></param>
        /// <param name="action"></param>
        /// <param name="cron"></param>
        /// <returns></returns>
        void RecurringAsyncJob<T>(string jobId, Expression<Func<T, Task>> asyncAction, Func<string> cron) where T : class, new();

        /// <summary>
        /// 加入作业，定期执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jobId"></param>
        /// <param name="action"></param>
        /// <param name="cron"></param>
        /// <returns></returns>
        void RecurringJob<T>(string jobId, Expression<Action<T>> action, string cron) where T : class, new();

        /// <summary>
        /// 加入作业，定期执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jobId"></param>
        /// <param name="action"></param>
        /// <param name="cron"></param>
        /// <returns></returns>
        void RecurringAsyncJob<T>(string jobId, Expression<Func<T, Task>> asyncAction, string cron) where T : class, new();
    }
}
