using System;
using System.ComponentModel;
using System.Threading;
using Serilog;

namespace AJF.Pinger.PingerService
{
    public class Worker
    {
        private const string ConfigFileName = "photomover.config";
        private BackgroundWorker _backgroundWorker;
        //private List<IPhotoMoverConfig> _config;
        public bool WorkDone { get; set; }

        public void Start()
        {
            try
            {
                //var pathList = ConfigurationManager.AppSettings["PathList"].Split(';');
                //_config = new List<IPhotoMoverConfig>();

                //foreach (var path in pathList)
                //{
                //    Log.Logger.Information("Watching: " + path);
                //    var photoMoverConfig = new PhotoMoverConfig
                //    {
                //        Path = path
                //    };
                //    _config.Add(photoMoverConfig);
                //}

                _backgroundWorker = new BackgroundWorker
                {
                    WorkerSupportsCancellation = true
                };
                _backgroundWorker.DoWork += _backgroundWorker_DoWork;
                _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
                _backgroundWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "During Start", new object[0]);
                throw;
            }
        }

        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WorkDone = true;
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Log.Logger.Information("Doing work");

            try
            {
                DoWorkInternal(sender);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "During do work", new object[0]);
                throw;
            }
        }

        private void DoWorkInternal(object sender)
        {
            WorkDone = false;

            while (true)
            {
                var backgroundWorker = sender as BackgroundWorker;
                if (backgroundWorker == null || backgroundWorker.CancellationPending)
                {
                    Log.Information("backgroundworker.CancellationPending: {@backgroundWorker}", backgroundWorker);
                    return;
                }

                Thread.Sleep(1 * 500);
                Log.Information("Hello world");
                int res;
                var divRem = Math.DivRem(DateTime.Now.Second,5, out res);
                if(divRem==0)
                    Log.Warning("Hello world");
            }
        }


        public void Stop()
        {
            if (_backgroundWorker != null)
            {
                _backgroundWorker.CancelAsync();

                while (!WorkDone)
                    Thread.Sleep(500);
            }
        }
    }
}