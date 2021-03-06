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
using DotNetWorkQueue.Validation;

namespace DotNetWorkQueue.Transport.SqlServer.Basic.Query
{
    /// <summary>
    /// Returns the current date as UTC.
    /// </summary>
    /// <remarks>We need the date from SQL server, since the local machine time may not be in sync.</remarks>
    public class GetUtcDateQuery : IQuery<DateTime>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetUtcDateQuery"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public GetUtcDateQuery(string connectionString)
        {
            Guard.NotNullOrEmpty(() => connectionString, connectionString);
            ConnectionString = connectionString;
        }
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string ConnectionString { get; private set; }
    }
}
