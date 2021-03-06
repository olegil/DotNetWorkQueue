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
using System.Data.SQLite;
using DotNetWorkQueue.Exceptions;
using DotNetWorkQueue.Transport.SQLite.Basic.Command;
using DotNetWorkQueue.Validation;

namespace DotNetWorkQueue.Transport.SQLite.Basic.CommandHandler
{
    /// <summary>
    /// Creates tables for storing job info
    /// </summary>
    public class CreateJobTablesCommandHandler : ICommandHandlerWithOutput<CreateJobTablesCommand, QueueCreationResult>
    {
        private readonly IConnectionInformation _connectionInformation;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateJobTablesCommandHandler"/> class.
        /// </summary>
        /// <param name="connectionInformation">The connection information.</param>
        public CreateJobTablesCommandHandler(IConnectionInformation connectionInformation)
        {
            Guard.NotNull(() => connectionInformation, connectionInformation);
            _connectionInformation = connectionInformation;
        }

        /// <summary>
        /// Handles the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        /// <exception cref="DotNetWorkQueueException"></exception>
        public QueueCreationResult Handle(CreateJobTablesCommand command)
        {
            var script = string.Empty;
            try
            {
                if (!DatabaseExists.Exists(_connectionInformation.ConnectionString))
                { //no db file, create
                    var fileName = GetFileNameFromConnectionString.GetFileName(_connectionInformation.ConnectionString);
                    SQLiteConnection.CreateFile(fileName.FileName);
                }
                using (var conn = new SQLiteConnection(_connectionInformation.ConnectionString))
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction())
                    {
                        foreach (var t in command.Tables)
                        {
                            using (var commandSql = conn.CreateCommand())
                            {
                                script = t.Script();
                                commandSql.Transaction = trans;
                                commandSql.CommandText = script;
                                commandSql.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }
                }

                return new QueueCreationResult(QueueCreationStatus.Success);
            }
            //if the queue already exists, return that status; otherwise, bubble the error
            catch (SQLiteException error)
            {
                if (error.ResultCode == SQLiteErrorCode.Error && error.Message.Contains("table") && error.Message.Contains("already exists"))
                {
                    return new QueueCreationResult(QueueCreationStatus.AttemptedToCreateAlreadyExists);
                }
                throw new DotNetWorkQueueException($"Failed to create table. SQL script was {script}",
                    error);
            }
        }
    }
}
