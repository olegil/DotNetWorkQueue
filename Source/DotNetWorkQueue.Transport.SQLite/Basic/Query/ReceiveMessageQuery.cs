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
namespace DotNetWorkQueue.Transport.SQLite.Basic.Query
{
    /// <summary>
    /// Dequeues a message from the queue.
    /// </summary>
    public class ReceiveMessageQuery : IQuery<IReceivedMessageInternal>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiveMessageQuery"/> class.
        /// </summary>
        /// <param name="messageId">A specific message identifier to de-queue. If null, the first message found will be de-queued.</param>
        public ReceiveMessageQuery(IMessageId messageId)
        {
            MessageId = messageId;
        }
        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public IMessageId MessageId { get; private set; }
    }
}
