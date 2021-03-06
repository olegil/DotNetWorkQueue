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
using StackExchange.Redis;
namespace DotNetWorkQueue.Transport.Redis.Basic.Lua
{
    /// <summary>
    /// Moves delayed records to the pending queue
    /// </summary>
    internal class MoveDelayedToPendingLua: BaseLua
    {
        public MoveDelayedToPendingLua(IRedisConnection connection, RedisNames redisNames)
            : base(connection, redisNames)
        {
            Script = @"local signal = tonumber(@signalID)
                       local uuids = redis.call('zrangebyscore', @delaykey, 0, @currenttime, 'LIMIT', 0, @limit)
                       if #uuids == 0 then
                        return 0
                       end

                       for k, v in pairs(uuids) do                             
                        redis.call('zrem',  @delaykey, v)
                        redis.call('lpush', @pendingkey, v) 
                        if signal == 1 then
                            redis.call('publish', @channel, v) 
                        end
                       end

                        if signal == 0 then
                            redis.call('publish', @channel, '') 
                        end
                      return table.getn(uuids)";
        }
        /// <summary>
        /// Moves delayed records to the pending queue
        /// </summary>
        /// <param name="unixTime">The unix time.</param>
        /// <param name="count">The count.</param>
        /// <param name="rpc">if set to <c>true</c> [RPC].</param>
        /// <returns></returns>
        public int Execute(long unixTime, int count, bool rpc)
        {
            if (Connection.IsDisposed)
                return 0;

            var db = Connection.Connection.GetDatabase();
            return (int) db.ScriptEvaluate(LoadedLuaScript, GetParameters(unixTime, count, rpc));
        }
        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <param name="unixTime">The unix time.</param>
        /// <param name="count">The count.</param>
        /// <param name="rpc">if set to <c>true</c> [RPC].</param>
        /// <returns></returns>
        private object GetParameters(long unixTime, int count, bool rpc)
        {
            return new
            {
                pendingkey = (RedisKey)RedisNames.Pending,
                delaykey = (RedisKey)RedisNames.Delayed,
                currenttime = unixTime,
                channel = RedisNames.Notification,
                limit = count,
                signalID = Convert.ToInt32(rpc),
            };
        }
    }
}
