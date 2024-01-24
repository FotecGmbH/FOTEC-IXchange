// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Biss.Log.Producer;
using IXchange.Service.Com.Rest.Controllers;
using Microsoft.Extensions.Logging;

namespace ConnectivityHost.Helper
{
    /// <summary>
    /// <para>Helper fuer background klassen</para>
    /// Klasse BackgroundHelper. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class BackgroundHelper
    {
        /// <summary>
        /// Backgroundworker, fuer Trigger
        /// </summary>
        /// <returns>Task</returns>
        public static async Task Worker(CancellationToken cancellationToken, long waitTime, Func<Task> workerFunction)
        {
            Logging.Log.LogInfo($"[{nameof(BackgroundTriggerWorker)}]({nameof(Worker)}): BackgroundWorker started");
            long milliSecWorkingTime = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay((int) Math.Max(100, waitTime - milliSecWorkingTime), cancellationToken).ConfigureAwait(false);
                }
                catch (TaskCanceledException )
                {
                }

                var sw = new Stopwatch();
                sw.Start();

                try
                {
                    await workerFunction.Invoke();
                }
                catch (Exception e)
                {
                    Logging.Log.LogCritical($"{e}");
                }

                sw.Stop();
                milliSecWorkingTime = sw.ElapsedMilliseconds;
            }

            Logging.Log.LogInfo($"[{nameof(MeasurementResultController)}]({nameof(Worker)}): BackgroundWorker stopped");
        }
    }
}