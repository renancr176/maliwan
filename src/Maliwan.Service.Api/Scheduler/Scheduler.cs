using System.Reflection;
using Quartz;

namespace Maliwan.Service.Api.Scheduler;

public static class Scheduler
{
    public static void AddJobs(this IServiceCollection services)
    {
        //services.AddTransient<SomeJob>();
    }

    public static void AddScheduler(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddJobs();

        services.Configure<QuartzOptions>(options =>
        {
            options.Scheduling.IgnoreDuplicates = true; // default: false
            options.Scheduling.OverWriteExistingData = true; // default: true
        });

        //See examples here https://www.quartz-scheduler.net/documentation/quartz-3.x/packages/microsoft-di-integration.html#di-aware-job-factories
        services.AddQuartz(q =>
        {
            #region Config

            q.SchedulerId = Assembly.GetCallingAssembly()?.GetName()?.Name ?? "Maliwan.Services.Api";
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.UseSimpleTypeLoader();
            q.UseInMemoryStore();
            q.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = 10;
            });
            q.UseTimeZoneConverter();

            #endregion

            //q.ScheduleJob<SomeJob>(trigger => trigger
            //    .WithIdentity(nameof(SomeJob))
            //    .WithDailyTimeIntervalSchedule(x => x.WithInterval(1, IntervalUnit.Minute))
            //    .StartNow());
        });

        services.AddQuartzHostedService(options =>
        {
            // when shutting down we want jobs to complete gracefully
            options.WaitForJobsToComplete = true;
        });
    }
}