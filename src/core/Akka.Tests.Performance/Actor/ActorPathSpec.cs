﻿//-----------------------------------------------------------------------
// <copyright file="ActorPathSpec.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2016 Typesafe Inc. <http://www.typesafe.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using Akka.Actor;
using NBench;

namespace Akka.Tests.Performance.Actor
{
    /// <summary>
    ///     Performance specifications for <see cref="ActorPath" />
    /// </summary>
    public class ActorPathSpec
    {
        private const string ParseThroughputCounterName = "ParseOp";
        private const double MinimumAcceptableOperationsPerSecond = 1000000.0d; //million op / second
        private static readonly RootActorPath RootAddress = new RootActorPath(Address.AllSystems);
        private Counter _parseThroughput;

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            _parseThroughput = context.GetCounter(ParseThroughputCounterName);
        }

        [PerfBenchmark(Description = "Tests how quickly ActorPath.Parse can run on a LOCAL actor path",
            RunMode = RunMode.Throughput, NumberOfIterations = 13, RunTimeMilliseconds = 1000,
            TestMode = TestMode.Measurement)]
        [CounterThroughputAssertion(ParseThroughputCounterName, MustBe.GreaterThan, MinimumAcceptableOperationsPerSecond
            )]
        [GcMeasurement(GcMetric.TotalCollections, GcGeneration.AllGc)]
        public void ParseLocalThroughput(BenchmarkContext context)
        {
            ActorPath.Parse("akka://Sys/user/foo");
            _parseThroughput.Increment();
        }

        [PerfBenchmark(Description = "Tests how quickly ActorPath.Parse can run on a REMOTE actor path",
            RunMode = RunMode.Throughput, NumberOfIterations = 13, RunTimeMilliseconds = 1000,
            TestMode = TestMode.Measurement)]
        [CounterThroughputAssertion(ParseThroughputCounterName, MustBe.GreaterThan, MinimumAcceptableOperationsPerSecond
            )]
        [GcMeasurement(GcMetric.TotalCollections, GcGeneration.AllGc)]
        public void ParseRemoteThroughput(BenchmarkContext context)
        {
            ActorPath.Parse("akka.tcp://Sys@localhost:9091/user/foo");
            _parseThroughput.Increment();
        }

        [PerfBenchmark(Description = "Tests how quickly ActorPath.TryParse can run on a LOCAL actor path",
            RunMode = RunMode.Throughput, NumberOfIterations = 13, RunTimeMilliseconds = 1000,
            TestMode = TestMode.Measurement)]
        [CounterThroughputAssertion(ParseThroughputCounterName, MustBe.GreaterThan, MinimumAcceptableOperationsPerSecond
            )]
        [GcMeasurement(GcMetric.TotalCollections, GcGeneration.AllGc)]
        public void TryParseLocalThroughput(BenchmarkContext context)
        {
            ActorPath target;
            ActorPath.TryParse("akka://Sys/user/foo", out target);
            _parseThroughput.Increment();
        }

        [PerfBenchmark(Description = "Tests how quickly ActorPath.TryParse can run on a REMOTE actor path",
            RunMode = RunMode.Throughput, NumberOfIterations = 13, RunTimeMilliseconds = 1000,
            TestMode = TestMode.Measurement)]
        [CounterThroughputAssertion(ParseThroughputCounterName, MustBe.GreaterThan, MinimumAcceptableOperationsPerSecond
            )]
        [GcMeasurement(GcMetric.TotalCollections, GcGeneration.AllGc)]
        public void TryParseRemoteThroughput(BenchmarkContext context)
        {
            ActorPath target;
            ActorPath.TryParse("akka.tcp://Sys@localhost:9091/user/foo", out target);
            _parseThroughput.Increment();
        }

        [PerfBenchmark(Description = "Tests how quickly ActorPath.TryParseAddress can run on a LOCAL address",
            RunMode = RunMode.Throughput, NumberOfIterations = 13, RunTimeMilliseconds = 1000,
            TestMode = TestMode.Measurement)]
        [CounterThroughputAssertion(ParseThroughputCounterName, MustBe.GreaterThan, 
            MinimumAcceptableOperationsPerSecond)]
        [GcMeasurement(GcMetric.TotalCollections, GcGeneration.AllGc)]
        public void TryParseAddressLocalThroughput(BenchmarkContext context)
        {
            Address target;
            ActorPath.TryParseAddress("akka://Sys/user/foo", out target);
            _parseThroughput.Increment();
        }

        [PerfBenchmark(Description = "Tests how quickly ActorPath.TryParseAddress can run on a REMOTE address",
            RunMode = RunMode.Throughput, NumberOfIterations = 13, RunTimeMilliseconds = 1000,
            TestMode = TestMode.Measurement)]
        [CounterThroughputAssertion(ParseThroughputCounterName, MustBe.GreaterThan, MinimumAcceptableOperationsPerSecond
            )]
        [GcMeasurement(GcMetric.TotalCollections, GcGeneration.AllGc)]
        public void TryParseAddressRemoteThroughput(BenchmarkContext context)
        {
            Address target;
            ActorPath.TryParseAddress("akka.tcp://Sys@locahost:9101/user/foo", out target);
            _parseThroughput.Increment();
        }

        [PerfBenchmark(Description = "Tests how quickly a single ActorPath '/' operation can run",
            RunMode = RunMode.Throughput, NumberOfIterations = 13, RunTimeMilliseconds = 1000,
            TestMode = TestMode.Measurement)]
        [CounterThroughputAssertion(ParseThroughputCounterName, MustBe.GreaterThan, MinimumAcceptableOperationsPerSecond
            )]
        [GcMeasurement(GcMetric.TotalCollections, GcGeneration.AllGc)]
        public void ActorPath1ConcatenateThroughput(BenchmarkContext context)
        {
            var newPath = RootAddress/"user";
            _parseThroughput.Increment();
        }

        [PerfBenchmark(
            Description =
                "Tests how quickly a two immediate ActorPath '/' operations can run - much closer to how this method works in the real world",
            RunMode = RunMode.Throughput, NumberOfIterations = 13, RunTimeMilliseconds = 1000,
            TestMode = TestMode.Measurement)]
        [CounterThroughputAssertion(ParseThroughputCounterName, MustBe.GreaterThan, MinimumAcceptableOperationsPerSecond
            )]
        [GcMeasurement(GcMetric.TotalCollections, GcGeneration.AllGc)]
        public void ActorPath2ConcatenateThroughput(BenchmarkContext context)
        {
            var newPath = RootAddress/"user"/"foo";
            _parseThroughput.Increment();
        }

        [PerfBenchmark(Description = "Tests how much memory 100,000 ActorPath instances consume",
            RunMode = RunMode.Iterations, NumberOfIterations = 13, RunTimeMilliseconds = 1000,
            TestMode = TestMode.Measurement)]
        [CounterMeasurement(ParseThroughputCounterName)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        [GcMeasurement(GcMetric.TotalCollections, GcGeneration.AllGc)]
        public void MemoryFootprint(BenchmarkContext context)
        {
            var actorPaths = new ActorPath[100000];
            for (var i = 0; i < 100000;)
            {
                actorPaths[i] = new RootActorPath(Address.AllSystems);
                ++i;
                _parseThroughput.Increment();
            }
        }
    }
}