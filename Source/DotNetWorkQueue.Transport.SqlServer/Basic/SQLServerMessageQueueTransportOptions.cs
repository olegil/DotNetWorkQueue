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
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using DotNetWorkQueue.Transport.SqlServer.Schema;
namespace DotNetWorkQueue.Transport.SqlServer.Basic
{
    /// <summary>
    /// Transport options. Generally speaking, this controls the feature set of the transport.
    /// </summary>
    public class SqlServerMessageQueueTransportOptions: IReadonly, ISetReadonly
    {
        private bool _enableStatusTable;
        private bool _enablePriority;
        private bool _enableHoldTransactionUntilMessageCommited;
        private bool _enableStatus;
        private bool _enableHeartBeat;
        private bool _enableDelayedProcessing;
        private QueueTypes _queueType;
        private bool _enableMessageExpiration;

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerMessageQueueTransportOptions" /> class.
        /// </summary>
        public SqlServerMessageQueueTransportOptions()
        {
            EnableDelayedProcessing = false;
            EnableHeartBeat = true;
            EnableHoldTransactionUntilMessageCommited = false;
            EnablePriority = false;
            EnableStatus = true;
            EnableMessageExpiration = false;
            QueueType = QueueTypes.Normal;
            EnableStatusTable = false;

            AdditionalColumns = new ColumnList();
            AdditionalConstraints = new ConstraintList();
        }
        #endregion

        #region User Settings

        /// <summary>
        /// Additional columns that can be attached to the queue.
        /// </summary>
        /// <value>
        /// The additional columns.
        /// </value>
        /// <remarks>See <see cref="IAdditionalMessageData"/> for how to pass in data when saving messages </remarks>
        public ColumnList AdditionalColumns { get; }

        /// <summary>
        /// Additional constraints or indexes that can be attached to the queue.
        /// </summary>
        /// <value>
        /// The additional constraints.
        /// </value>
        public ConstraintList AdditionalConstraints { get; }

        #endregion

        #region Options
        /// <summary>
        /// Gets or sets a value indicating whether [enable priority].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable priority]; otherwise, <c>false</c>.
        /// </value>
        public bool EnablePriority
        {
            get { return _enablePriority; }
            set
            {
                FailIfReadOnly();
                _enablePriority = value;
            }
        }
        /// <summary>
        /// If true, a transaction will be held until the message is finished processing.
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable hold transaction until message committed]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableHoldTransactionUntilMessageCommited
        {
            get { return _enableHoldTransactionUntilMessageCommited; }
            set
            {
                FailIfReadOnly();
                _enableHoldTransactionUntilMessageCommited = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable status].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable status]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableStatus
        {
            get { return _enableStatus; }
            set
            {
                FailIfReadOnly();
                _enableStatus = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable heart beat].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable heart beat]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableHeartBeat
        {
            get { return _enableHeartBeat; }
            set
            {
                FailIfReadOnly();
                _enableHeartBeat = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable delayed processing].
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable delayed processing]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableDelayedProcessing
        {
            get { return _enableDelayedProcessing; }
            set
            {
                FailIfReadOnly();
                _enableDelayedProcessing = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable status table].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable status table]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableStatusTable
        {
            get { return _enableStatusTable || AdditionalColumns.Count > 0; }
            set
            {
                FailIfReadOnly();
                _enableStatusTable = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the queue.
        /// </summary>
        /// <value>
        /// The type of the queue.
        /// </value>
        public QueueTypes QueueType
        {
            get { return _queueType; }
            set
            {
                FailIfReadOnly();
                _queueType = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable message expiration].
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable message expiration]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableMessageExpiration
        {
            get { return _enableMessageExpiration; }
            set
            {
                FailIfReadOnly();
                _enableMessageExpiration = value;
            }
        }

        #endregion

        #region Validation
        /// <summary>
        /// Validates the configuration settings
        /// </summary>
        /// <returns></returns>
        public Validation ValidConfiguration()
        {
            var v = new Validation();
            var sbErrors = new StringBuilder();
            v.Valid = true;

            if (EnableHoldTransactionUntilMessageCommited)
            {
                if (EnableHeartBeat)
                {
                    sbErrors.AppendLine("[EnableHeartBeat] must be false when using transactions");
                }
                if (EnableStatus)
                {
                    sbErrors.AppendLine("[EnableStatus] must be false when using transactions. The status table may still be used.");
                }

                if (QueueType != QueueTypes.Normal)
                {
                    sbErrors.AppendLine("[EnableHoldTransactionUntilMessageCommited] must be false when using RPC queues");
                }
            }

            v.ErrorMessage = sbErrors.ToString();
            if (!string.IsNullOrWhiteSpace(v.ErrorMessage))
                v.Valid = false;

            return v;
        }
        #endregion

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly { get; protected set; }

        /// <summary>
        /// Throws an exception if the read only flag is true.
        /// </summary>
        /// <exception cref="System.Data.ReadOnlyException"></exception>
        protected void FailIfReadOnly()
        {
            if (IsReadOnly) throw new InvalidOperationException();
        }

        /// <summary>
        /// Marks this instance as immutable
        /// </summary>
        public void SetReadOnly()
        {
            IsReadOnly = true;
        }

        /// <summary>
        /// Configuration validation status
        /// </summary>
        public class Validation
        {
            /// <summary>
            /// Gets or sets a value indicating whether the configuration is valid.
            /// </summary>
            /// <value>
            ///   <c>true</c> if valid; otherwise, <c>false</c>.
            /// </value>
            public bool Valid { get; set; }
            /// <summary>
            /// Gets or sets the error message.
            /// </summary>
            /// <value>
            /// The error message.
            /// </value>
            public string ErrorMessage { get; set; }
        }

        #region Internal Methods
        /// <summary>
        /// Adds the built in columns.
        /// </summary>
        /// <param name="command">The command.</param>
        internal void AddBuiltInColumns(StringBuilder command)
        {
            if (EnableDelayedProcessing)
            {
                command.Append(", QueueProcessTime ");
            }

            if (EnablePriority)
            {
                command.Append(", Priority ");
            }

            if (EnableStatus)
            {
                command.Append(", Status ");
            }

            if (EnableMessageExpiration || QueueType == QueueTypes.RpcReceive || QueueType == QueueTypes.RpcSend)
            {
                command.Append(", ExpirationTime ");
            }
        }
        /// <summary>
        /// Adds the built in column values.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <param name="expiration">The expiration.</param>
        /// <param name="command">The command.</param>
        internal void AddBuiltInColumnValues(TimeSpan? delay, TimeSpan expiration, StringBuilder command)
        {
            if (EnableDelayedProcessing)
            {
                if (delay.HasValue && delay != TimeSpan.Zero)
                {
                    command.Append(", DATEADD(ms," + delay.Value.TotalMilliseconds.ToString(CultureInfo.InvariantCulture) + ", GetUTCDate()) ");
                }
                else
                {
                    command.Append(", GetUTCDate() ");
                }
            }

            if (EnablePriority)
            {
                command.Append(", @Priority ");
            }

            if (EnableStatus)
            {
                command.Append(", @Status ");
            }

            if (EnableMessageExpiration || QueueType == QueueTypes.RpcReceive || QueueType == QueueTypes.RpcSend)
            {
                if (expiration != TimeSpan.Zero)
                {
                    command.Append(", DATEADD(ms," + expiration.TotalMilliseconds.ToString(CultureInfo.InvariantCulture) + ",GetUTCDate()) ");
                }
                else
                {
                    command.Append(", NULL ");
                }
            }

        }
        /// <summary>
        /// Adds the built in columns parameters.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="data">The data.</param>
        internal void AddBuiltInColumnsParams(SqlCommand command, IAdditionalMessageData data)
        {
            if (EnablePriority)
            {
                var priority = 0;
                if (data.GetPriority().HasValue)
                {
                    // ReSharper disable once PossibleInvalidOperationException
                    priority = data.GetPriority().Value;
                }
                command.Parameters.Add("@priority", SqlDbType.TinyInt, 1).Value = priority;
            }
            if (EnableStatus)
            {
                command.Parameters.Add("@Status", SqlDbType.Int, 4).Value = 0;
            }
        }
        #endregion
    }
    /// <summary>
    /// Types of queues that this transport supports
    /// </summary>
    public enum QueueTypes
    {
        /// <summary>
        /// Standard queue
        /// </summary>
        Normal,
        /// <summary>
        /// RPC send
        /// </summary>
        RpcSend,
        /// <summary>
        /// RPC receive
        /// </summary>
        RpcReceive
    }
}
