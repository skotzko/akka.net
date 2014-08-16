﻿using System;
using Akka.Actor;
using Akka.Routing;
using Akka.TestKit;
using Xunit;
using Akka.Util;

namespace Akka.Tests.Routing
{
    
    public class ListenerSpec : AkkaSpec
    {
        [Fact]
        public void Listener_must_listen_in()
        {
            //arrange
            var fooLatch = new TestLatch(Sys, 2);
            var barLatch = new TestLatch(Sys, 2);
            var barCount = new AtomicCounter(0);

            var broadcast = Sys.ActorOf<BroadcastActor>();
            var newListenerProps = Props.Create(() => new ListenerActor(fooLatch, barLatch, barCount));
            var a1 = Sys.ActorOf(newListenerProps);
            var a2 = Sys.ActorOf(newListenerProps);
            var a3 = Sys.ActorOf(newListenerProps);

            //act
            broadcast.Tell(new Listen(a1));
            broadcast.Tell(new Listen(a2));
            broadcast.Tell(new Listen(a3));

            broadcast.Tell(new Deafen(a3));

            broadcast.Tell(new WithListeners(a => a.Tell("foo")));
            broadcast.Tell("foo");

            //assert
            barLatch.Ready(TestLatch.DefaultTimeout);
            Assert.Equal(2, barCount.Current);

            fooLatch.Ready(TestLatch.DefaultTimeout);
            foreach (var actor in new[] {a1, a2, a3, broadcast})
            {
                Sys.Stop(actor);
            }
        }


        #region Test Actors

        public class BroadcastActor : UntypedActor, IListeners
        {
            public BroadcastActor()
            {
                Listeners = new ListenerSupport();
            }

            protected override void OnReceive(object message)
            {
                PatternMatch.Match(message)
                    .With<ListenerMessage>(l => Listeners.ListenerReceive(l))
                    .With<string>(s =>
                    {
                        if (s.Equals("foo"))
                            Listeners.Gossip("bar");
                    });
            }

            public ListenerSupport Listeners { get; private set; }
        }

        public class ListenerActor : UntypedActor, IListeners
        {
            private TestLatch _fooLatch;
            private TestLatch _barLatch;
            private AtomicCounter _barCount;

            public ListenerActor(TestLatch fooLatch, TestLatch barLatch, AtomicCounter barCount)
            {
                _fooLatch = fooLatch;
                _barLatch = barLatch;
                _barCount = barCount;
                Listeners = new ListenerSupport();
            }

            protected override void OnReceive(object message)
            {
                PatternMatch.Match(message)
                    .With<string>(str =>
                    {
                        if (str.Equals("bar"))
                        {
                            _barCount.GetAndIncrement();
                            _barLatch.CountDown();
                        }

                        if (str.Equals("foo"))
                        {
                            _fooLatch.CountDown();
                        }
                    });
            }

            public ListenerSupport Listeners { get; private set; }
        }


        #endregion
    }
}
