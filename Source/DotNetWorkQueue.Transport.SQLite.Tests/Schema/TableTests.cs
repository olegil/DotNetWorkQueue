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
#region Using

using System.Linq;
using DotNetWorkQueue.Transport.SQLite.Schema;
using Xunit;

#endregion

namespace DotNetWorkQueue.Transport.SQLite.Tests.Schema
{
    public class TableTests
    {
        [Fact]
        public void Default()
        {
            var test = new Table("test");
            Assert.Equal("test", test.Name);
        }
        [Fact]
        public void Set_PrimaryKey()
        {
            var c = new Constraint("ix_testing", ContraintType.PrimaryKey, "testing");
            var test = new Table("test");
            test.Constraints.Add(c);
            Assert.Equal(c, test.PrimaryKey);
        }
        [Fact]
        public void No_PrimaryKey()
        {
            var c = new Constraint("ix_testing", ContraintType.Index, "testing");
            var test = new Table("test");
            test.Constraints.Add(c);
            Assert.Null(test.PrimaryKey);
        }
        [Fact]
        public void No_PrimaryKey2()
        {
            var test = new Table("test");
            Assert.Null(test.PrimaryKey);
        }
        [Fact]
        public void GetSet_Name()
        {
            var test = new Table("test") {Name = "test1"};
            Assert.Equal("test1", test.Name);
        }
        [Fact]
        public void Create_Column()
        {
            var c = new Column("testing", ColumnTypes.Integer, true, null);
            var test = new Table("test");
            test.Columns.Add(c);
            Assert.True(test.Columns.Items.Any(item => item.Name == "testing"));
        }
        [Fact]
        public void Create_Script()
        {
            var c = new Column("testing", ColumnTypes.Integer, true, null);
            var cc = new Constraint("ix_testing", ContraintType.Index, "testing");
            var test = new Table("test");
            test.Constraints.Add(cc);
            test.Columns.Add(c);
            //set the table reference
            foreach (var ccc in test.Constraints)
            {
                ccc.Table = test.Info;
            }
            Assert.Contains("test", test.Script());
        }
    }
}
