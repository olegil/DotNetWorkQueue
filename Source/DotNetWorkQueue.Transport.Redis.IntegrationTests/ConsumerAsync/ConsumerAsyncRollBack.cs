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
using DotNetWorkQueue.IntegrationTests.Shared;
using DotNetWorkQueue.IntegrationTests.Shared.ConsumerAsync;
using DotNetWorkQueue.IntegrationTests.Shared.Producer;
using DotNetWorkQueue.Transport.Redis.Basic;
using System;
using Xunit;
namespace DotNetWorkQueue.Transport.Redis.IntegrationTests.ConsumerAsync
{
    [Collection("Redis")]
    public class ConsumerAsyncRollBack
    {
        [Theory]
        [InlineData(100, 1, 400, 5, 5, 5, ConnectionInfoTypes.Linux),
         InlineData(50, 5, 200, 5, 1, 3, ConnectionInfoTypes.Linux),
         InlineData(10, 5, 180, 7, 1, 1, ConnectionInfoTypes.Linux),
            InlineData(100, 1, 400, 5, 5, 5, ConnectionInfoTypes.Windows),
         InlineData(50, 5, 200, 5, 1, 3, ConnectionInfoTypes.Windows),
         InlineData(10, 5, 180, 7, 1, 1, ConnectionInfoTypes.Windows)]
        public void Run(int messageCount, int runtime, int timeOut, int workerCount, int readerCount, int queueSize, ConnectionInfoTypes type)
        {
            var queueName = GenerateQueueName.Create();
            var logProvider = LoggerShared.Create(queueName, GetType().Name);
            var connectionString = new ConnectionInfo(type).ConnectionString;
            using (
                var queueCreator =
                    new QueueCreationContainer<RedisQueueInit>(
                        serviceRegister => serviceRegister.Register(() => logProvider, LifeStyles.Singleton)))
            {
                try
                {
                    //create data
                    var producer = new ProducerShared();
                    producer.RunTest<RedisQueueInit, FakeMessage>(queueName,
                        connectionString, false, messageCount, logProvider, Helpers.GenerateData,
                        Helpers.Verify, false);

                    //process data
                    var consumer = new ConsumerAsyncRollBackShared<FakeMessage>();
                    consumer.RunConsumer<RedisQueueInit>(queueName, connectionString, false,
                        workerCount, logProvider,
                        timeOut, readerCount, queueSize, runtime, messageCount, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(12));
                    LoggerShared.CheckForErrors(queueName);
                    using (var count = new VerifyQueueRecordCount(queueName, connectionString))
                    {
                        count.Verify(0, false, -1);
                    }
                }
                finally
                {
                    using (
                        var oCreation =
                            queueCreator.GetQueueCreation<RedisQueueCreation>(queueName,
                                connectionString)
                        )
                    {
                        oCreation.RemoveQueue();
                    }
                }
            }
        }
    }
}
