﻿// ---------------------------------------------------------------------
//This file is part of DotNetWorkQueue
//Copyright © 2016 Brian Lehnen
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// ---------------------------------------------------------------------
using System;
using System.Threading;
using DotNetWorkQueue.Exceptions;
using DotNetWorkQueue.Queue;
using NSubstitute;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace DotNetWorkQueue.Tests.Queue
{
    public class ConsumerMethodQueueTests
    {
        [Fact]
        public void IsDisposed_False_By_Default()
        {
            using (var test = CreateQueue())
            {
                Assert.Equal(test.IsDisposed, false);
            }
        }

        [Fact]
        public void Disposed_Instance_Sets_IsDisposed()
        {
            var test = CreateQueue();
            test.Dispose();
            Assert.Equal(test.IsDisposed, true);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "part of test")]
        [Fact]
        public void Call_Dispose_Multiple_Times_Ok()
        {
            using (var test = CreateQueue())
            {
                test.Dispose();
            }
        }

        [Theory, AutoData]
        public void Disposed_Instance_Get_Configuration_Exception(int value)
        {
            var test = CreateQueue();
            test.Dispose();
            Assert.Throws<ObjectDisposedException>(
                delegate
                {
                    test.Configuration.Worker.WorkerCount = value;
                });
        }

        [Fact]
        public void Disposed_Instance_Start_Exception()
        {
            var test = CreateQueue();
            test.Dispose();
            Assert.Throws<ObjectDisposedException>(
                delegate
                {
                    test.Start();
                });
        }

        [Fact]
        public void Calling_Start_Multiple_Times_Exception()
        {
            using (var test = CreateQueue())
            {
                test.Start();
                Assert.Throws<DotNetWorkQueueException>(
                    delegate
                    {
                        test.Start();
                    });
            }
        }

        private ConsumerMethodQueue CreateQueue()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            fixture.Inject(CreateConsumerQueue());
            return fixture.Create<ConsumerMethodQueue>();
        }

        private IConsumerQueue CreateConsumerQueue()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            var cancelWork = fixture.Create<IQueueCancelWork>();
            fixture.Inject(cancelWork);
            cancelWork.CancellationTokenSource.Returns(new CancellationTokenSource());
            cancelWork.StopTokenSource.Returns(new CancellationTokenSource());

            var stopWorker = fixture.Create<StopWorker>();
            fixture.Inject(stopWorker);
            return fixture.Create<ConsumerQueue>();
        }
    }
}
