using System;
using System.IO;
using Quartz;
using Quartz.Impl;
using Serilog;

namespace Unipluss.Sign.StorageService.MovingService
{
    public class MoveFilesService : IDisposable
    {
        public const string SignerePades = "_signerepades.pdf";
        private IScheduler _scheduler;
        private FileSystemWatcher _watcher;

        public void Dispose()
        {
            if (_watcher != null)
                _watcher.Dispose();
            _watcher = null;

            if (_scheduler != null)
                _scheduler.Shutdown();
        }

        public void Start()
        {
            SetupWathcer();

            SetupScheduler();
        }

        private void SetupScheduler()
        {
            // Grab the Scheduler instance from the Factory 
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();


            // define the job and tie it to our HelloJob class
            var job = JobBuilder.Create<HandleFileOlderThanJob>()
                .WithIdentity("HandleFileOlderThanJob")
                .Build();


            // Trigger the job to run now, and then repeat every 10 seconds
            var trigger = TriggerBuilder.Create()
                .ForJob(job)
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(10)
                    .RepeatForever())
                .Build();

            // Tell quartz to schedule the job using our trigger
            _scheduler.ScheduleJob(job, trigger);


            // and start it off
            _scheduler.Start();
        }

        private void SetupWathcer()
        {
            _watcher = new FileSystemWatcher();
            _watcher.Path = AppSettingsReader.MoveFromFolder;
            _watcher.Filter = "*" + SignerePades;
            _watcher.Created += Watcher_Created;
            _watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            var filename = string.Format("*{0}*", e.Name.ToLowerInvariant().Replace("_signerepades.pdf", ""));
            Log.Logger.Debug("Created event fired for {0}", filename);

            var files = Directory.GetFiles(Path.GetDirectoryName(e.FullPath), filename);
            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    var sourcePath = Path.Combine(AppSettingsReader.MoveToFolder, Path.GetFileName(file));
                    File.Move(file, sourcePath);
                    Log.Logger.Debug("Moving file: {0} to {1}", file, sourcePath);
                }
            }
        }
    }
}