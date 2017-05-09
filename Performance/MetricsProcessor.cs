﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splitio.Services.Client.Interfaces;
using Splitio.Services.Client.Classes;
using System.Diagnostics;
using hq.metrics;
using hq.metrics.Core;

namespace Performance
{
    public class MetricsProcessor
    {
        ISplitClient client;
        IMetric timer;
        TimerMetric timerr;

        public MetricsProcessor()
        {
            Metrics metrics = new Metrics();
            timer = metrics.Timer(typeof(MetricsProcessor),"GET TREATMENT",TimeUnit.Seconds, TimeUnit.Nanoseconds);
            timerr = new TimerMetric(TimeUnit.Seconds, TimeUnit.Milliseconds);
        }

        private ISplitClient GetInstance(string apikey)
        {
            var config = new ConfigurationOptions();
            config.FeaturesRefreshRate = 30;
            config.SegmentsRefreshRate = 30;
            config.Endpoint = "https://sdk-aws-staging.split.io";
            config.EventsEndpoint = "https://events-aws-staging.split.io";
            config.ReadTimeout = 30000;
            config.ConnectionTimeout = 30000;
            config.Ready = 240000;
            config.NumberOfParalellSegmentTasks = 10;
            config.MaxImpressionsLogSize = 500000;
            config.ImpressionsRefreshRate = 300000;
            var instance = new SelfRefreshingClient(apikey, config);
            return instance;
        }

        public void ProcessRequest(int numberOfThreads, int minutesRunning)
        {
            client = GetInstance("mhfvtl3ilt6aeds8i1amn8qjssbdarlr76ju");
            Console.WriteLine("Running with: " + numberOfThreads + " threads " + minutesRunning + " minutes ");
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < numberOfThreads; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    var atributes = new Dictionary<string, object>();
                    atributes.Add("atrib", "1475094824");
                    atributes.Add("atrib2", "20");
                    while (true)
                    {
                        timerr.Time(() =>
                        {
                            client.GetTreatment("abcdefghijklmnopqrxyz123456789ABCDEF", "benchmark_jw_1", atributes);
                        });
                    }
                });
            }
            while (true)
            {
                if (sw.Elapsed > TimeSpan.FromMinutes(minutesRunning))
                {
                    Console.WriteLine("Values : ");
                    foreach (var value in timerr.Values)
                    {
                        Console.Write(value.ToString() + " - " );
                    }
                    Console.WriteLine(" ");
                    Console.WriteLine("Max : ");
                    Console.WriteLine(timerr.Max);
                    Console.WriteLine("Min : ");
                    Console.WriteLine(timerr.Min);
                    Console.Read();
                    return;
                }
            }

            
        }
    }
}