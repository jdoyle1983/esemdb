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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace EsEmDb
{
	
	
	public class EsEmTable
	{
		internal EsEmDatabase _Database;
		protected bool _TableLocked = false;
		
		protected string _TableName;
		internal Collection<EsEmColumn> _TableColumns = new Collection<EsEmColumn>();
		internal long _TableOffset = -1;
		internal OffsetLink _TableLink = new OffsetLink();
		internal OffsetLink _RowLink = new OffsetLink();
        internal Collection<Collection<object>> _TableCache = new Collection<Collection<object>>();
		
		public string Name
		{
			get
			{
				return _TableName;
			}
		}
		public long ColumnCount
		{
			get
			{
				return _TableColumns.Count;
			}
		}
		public EsEmColumn this[int ColumnIndex]
		{
			get
			{
				return _TableColumns[ColumnIndex];
			}
		}
		public EsEmColumn this[string ColumnName]
		{
			get
			{
				for( int i = 0; i < _TableColumns.Count; i++ )
					if(_TableColumns[i].Name == ColumnName)
						return _TableColumns[i];
				throw new Exception("Column '" + ColumnName + "' Not Found In Table");
			}
		}
		
		public void CreateColumn( string ColumnName, ColumnType DataType )
		{
			CreateColumn( ColumnName, DataType, false, false );
		}
		
		public void CreateColumn( string ColumnName, ColumnType DataType, bool IsAutoIncrement, bool IsPrimaryKey )
		{
			if(!_TableLocked)
			{
				EsEmColumn c = EsEmColumn.CreateColumn( this, ColumnName, DataType, IsAutoIncrement, IsPrimaryKey );
				if(c.IsPrimaryKey)
				{
					for( int i = 0; i < _TableColumns.Count; i++ )
						if(_TableColumns[i].IsPrimaryKey)
							throw new Exception("Cannot Have More Than 1 Primary Key");
				}
				_TableColumns.Add( c );
			}
			else
				throw new Exception("Table Is Already Committed, Adding Columns Is Not Allowed");
		}
		
		internal static EsEmTable CreateTable( EsEmDatabase Database, string TableName )
		{
			EsEmTable t = new EsEmTable();
			t._TableName = TableName;
			t._Database = Database;
			return t;
		}
		
		public void CommitTable()
		{
			DbTools.WaitForFile();
			FileStream Stream = File.OpenWrite( _Database._FilePath );
			long HeaderOffset = Stream.Length;
			long PrevLast = _Database._Header.Link.Last;
			_Database._Header.Link.Last = HeaderOffset;
			if( PrevLast != -1 )
			{
				for( int i = 0; i < _Database._Tables.Count; i++ )
					if( _Database._Tables[i]._TableOffset == PrevLast )
						_Database._Tables[i]._TableLink.Next = HeaderOffset;;
				_TableLink.Prev = PrevLast;
			}
			else
			{
				_Database._Header.Link.First = HeaderOffset;
				_Database._Header.Link.Last = HeaderOffset;
				_TableLink.Prev = -1;
			}
			_TableOffset = HeaderOffset;
			_TableLink.Next = -1;
			_Database._Tables.Add(this);
			_Database.WriteTableHeaders(ref Stream );
			Stream.Close();
			DbTools.ReleaseFile();
		}
		
		internal void ReadTableHeader( ref FileStream Stream, long Offset )
		{
			_TableOffset = Offset;
			BinaryReader Reader = new BinaryReader(Stream);
			Reader.BaseStream.Seek(Offset,SeekOrigin.Begin);
			byte[] NameBytes = Reader.ReadBytes(128);
			_TableName = DbTools.ByteArrayToDbItemName(NameBytes);
			_RowLink.Read( ref Stream );
			_TableLink.Read( ref Stream );
			int ColumnCount = Reader.ReadInt32();
			for( int i = 0; i < ColumnCount; i++ )
			{
				EsEmColumn c = new EsEmColumn();
				c.ReadColumnHeader( ref Stream );
				_TableColumns.Add( c );
			}
			
			_TableLocked = true;
			
			/*Console.WriteLine( "Read Table Header     : " + _TableName );
			Console.WriteLine( "Table Offset:         : " + _TableOffset.ToString() );
			Console.WriteLine( "Prev Table Offset     : " + _TableLink.Prev.ToString() );
			Console.WriteLine( "Next Table Offset     : " + _TableLink.Next.ToString() );
			Console.WriteLine( "First Row Offset      : " + _RowLink.First.ToString() );
			Console.WriteLine( "Last Row Offset       : " + _RowLink.Last.ToString() );
			Console.WriteLine();
			Console.WriteLine();*/
		}
		
		internal void WriteTableHeader( ref FileStream Stream, long Offset )
		{
			/*Console.WriteLine( "Writing Table Header  : " + _TableName );
			Console.WriteLine( "Table Offset:         : " + _TableOffset.ToString() );
			Console.WriteLine( "Prev Table Offset     : " + _TableLink.Prev.ToString() );
			Console.WriteLine( "Next Table Offset     : " + _TableLink.Next.ToString() );
			Console.WriteLine( "First Row Offset      : " + _RowLink.First.ToString() );
			Console.WriteLine( "Last Row Offset       : " + _RowLink.Last.ToString() );
			Console.WriteLine();
			Console.WriteLine();*/
			_TableOffset = Offset;
			BinaryWriter Writer = new BinaryWriter(Stream);
			Writer.BaseStream.Seek(Offset,SeekOrigin.Begin);
			byte[] NameBytes = DbTools.DbItemNameToByteArray( _TableName, 128 );
			Writer.Write(NameBytes);
			_RowLink.Write( ref Stream );
			_TableLink.Write( ref Stream );
			Writer.Write(_TableColumns.Count);
			for( int i = 0; i < _TableColumns.Count; i++ )
				_TableColumns[i].WriteColumnHeader( ref Stream );
			
			_TableLocked = true;
		}
		
		
	}
}
