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
namespace DotNetWorkQueue.Transport.SQLite.Basic
{
    /// <summary>
    /// Determines if a specified database exists
    /// </summary>
    /// <remarks>The database could be on the file system or in memory</remarks>
    public static class DatabaseExists
    {
        /// <summary>
        /// Returns true if the specified database exists
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static bool Exists(string connectionString)
        {
            var fileName = GetFileNameFromConnectionString.GetFileName(connectionString);
            return fileName.IsInMemory || System.IO.File.Exists(fileName.FileName);
        }
    }
}
