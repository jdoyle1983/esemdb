/*
This file is part of EsEmDb.

    EsEmDb is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    EsEmDb is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with EsEmDb.  If not, see <http://www.gnu.org/licenses/>.
	
	
	EsEmDb
	Original Author: Jason Doyle (jdoyle1983@gmail.com)
*/


using System;
using System.Threading;
using System.IO;

namespace EsEmDb
{
	public enum ColumnType
	{
		INTEGER,
		FLOAT,
		TEXT,
		RAW,
		DATETIME,
		BOOL,
		
		INVALID
	}
	
	
	public class EsEmColumn
	{	
		protected EsEmTable _Table;
		
		protected bool _AutoIncrement;
		protected bool _PrimaryKey;
		internal Int64 _NextAutoIncrementId;
		protected ColumnType _ColumnType;
		protected string _ColumnName;
				
		//Properties
		public bool IsAutoIncrement
		{
			get
			{
				return _AutoIncrement;
			}
		}
		public ColumnType GetColumnType
		{
			get
			{
				return _ColumnType;
			}
		}
		public string Name
		{
			get
			{
				return _ColumnName;
			}
		}
		public bool IsPrimaryKey
		{
			get
			{
				return _PrimaryKey;
			}
		}
		
		internal static EsEmColumn CreateColumn( EsEmTable Table, string ColumnName, ColumnType DataType, bool IsAutoIncrement, bool IsPrimaryKey )
		{
			EsEmColumn c = new EsEmColumn();
			c._Table = Table;
			if((IsAutoIncrement || IsPrimaryKey) && DataType != ColumnType.INTEGER)
				throw new Exception("Auto and Primary Key Can Only Be On an Integer Field");
			c._AutoIncrement = IsAutoIncrement;
			c._ColumnName = ColumnName;
			c._ColumnType = DataType;
			c._NextAutoIncrementId = 1;
			c._PrimaryKey = IsPrimaryKey;
			return c;
		}
		
		internal void ReadColumnHeader( ref FileStream Stream )
		{
			BinaryReader Reader = new BinaryReader(Stream);
			byte[] NameBytes = Reader.ReadBytes(128);
			_ColumnName = DbTools.ByteArrayToDbItemName(NameBytes);
			_ColumnType = DbTools.GetColumnType(Reader.ReadInt16());
			_AutoIncrement = Reader.ReadBoolean();
			_PrimaryKey = Reader.ReadBoolean();
			if(_AutoIncrement)
				_NextAutoIncrementId = Reader.ReadInt64();
		}
		
		internal void WriteColumnHeader( ref FileStream Stream )
		{
			BinaryWriter Writer = new BinaryWriter(Stream);
			byte[] NameBytes = DbTools.DbItemNameToByteArray( _ColumnName, 128 );
			Writer.Write(NameBytes);
			Writer.Write(DbTools.GetColumnTypeId(_ColumnType));
			Writer.Write(_AutoIncrement);
			Writer.Write(_PrimaryKey);
			if(_AutoIncrement)
				Writer.Write(_NextAutoIncrementId);
		}
	}
}
