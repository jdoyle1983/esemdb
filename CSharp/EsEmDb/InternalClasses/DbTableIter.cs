
using System;
using System.IO;
using System.Collections.ObjectModel;

namespace EsEmDb
{
	internal enum ColumnMatchOperation
	{
		IsNull,
		IsNotNull,
		NotEqual,
		Equal,
		GreaterThan,
		LessThan,
		GreaterThanOrEqualTo,
		LessThanOrEqualTo,
		Invalid
	}
	
	internal class sRowHeader
	{
		public long _MyOffset = -1;
		public long _PrevOffset = -1;
		public long _NextOffset = -1;
		
		public sRowHeader()
		{
		}
		
		public sRowHeader( long Offset, ref FileStream Stream )
		{
			Read( Offset, ref Stream );
		}
		
		public void Read( long Offset, ref FileStream Stream )
		{
			Stream.Seek(Offset, SeekOrigin.Begin);
			BinaryReader Reader = new BinaryReader( Stream );
			_MyOffset = Offset;
			_PrevOffset = Reader.ReadInt64();
			_NextOffset = Reader.ReadInt64();
		}
		
		public void Write( ref FileStream Stream )
		{
			Stream.Seek(_MyOffset,SeekOrigin.Begin);
			BinaryWriter Writer = new BinaryWriter( Stream );
			Writer.Write(_PrevOffset);
			Writer.Write(_NextOffset);
		}
		
		public void Write( ref FileStream Stream, long Offset )
		{
			_MyOffset = Offset;
			Write( ref Stream );
		}
		
		public sRowHeader GetPrev( ref FileStream Stream )
		{
			if(_PrevOffset != -1)
				return new sRowHeader( _PrevOffset, ref Stream );
			return null;
		}
		
		public sRowHeader GetNext( ref FileStream Stream )
		{
			if(_NextOffset != -1)
				return new sRowHeader( _NextOffset, ref Stream );
			return null;
		}
	}
	
	public class DbTableIter
	{
		public EsEmTable _Table;
		private BinaryReader Reader;
		private BinaryWriter Writer;
		private FileStream Stream;
		sRowHeader _CurrentHeader;
		private bool _IsNew = false;
		private bool _IsPrimed = false;
		private bool _IsDeleted = false;
        private int RowIndex = -1;
        internal bool BuildingCache = false;
		
		//Collection<byte[]> RowData = new Collection<byte[]>();
        Collection<object> RowData = new Collection<object>();
        //Collection<object> CacheRowData = new Collection<object>();
		Collection<string> RowNames = new Collection<string>();
		
		public int ColumnCount
		{
			get
			{
				return _Table._TableColumns.Count;
			}
		}
		public object this[int Index]
		{
			get
			{
				if(_IsDeleted)
					throw new Exception("Row is Deleted, Cannot Read");
				if(Index < RowData.Count && Index >= 0)
				{
                    if(RowData[Index] != null)
					{
                        if (_Table._TableColumns[Index].GetColumnType == ColumnType.INTEGER)
                            return (long)RowData[Index];
                        else if (_Table._TableColumns[Index].GetColumnType == ColumnType.FLOAT)
                            return (float)RowData[Index];
                        else if (_Table._TableColumns[Index].GetColumnType == ColumnType.TEXT)
                            return (string)RowData[Index];
                        else if (_Table._TableColumns[Index].GetColumnType == ColumnType.RAW)
                            return (byte[])RowData[Index];
                        else if (_Table._TableColumns[Index].GetColumnType == ColumnType.DATETIME)
                            return (DateTime)RowData[Index];
                        else if (_Table._TableColumns[Index].GetColumnType == ColumnType.BOOL)
                            return (bool)RowData[Index];
					}
				}
				return null;
			}
			set
			{
				if( _IsDeleted )
					throw new Exception("Row Is Deleted, Cannot Modify");
				if(Index < RowData.Count && Index >= 0)
				{
                    RowData[Index] = value;
                    /*
					if(_Table._TableColumns[Index].GetColumnType == ColumnType.INTEGER)
						RowData[Index] = BitConverter.GetBytes((long)value);
					else if(_Table._TableColumns[Index].GetColumnType == ColumnType.FLOAT)
						RowData[Index] = BitConverter.GetBytes((float)value);
					else if(_Table._TableColumns[Index].GetColumnType == ColumnType.TEXT)
						RowData[Index] = DbTools.StringToByteArray((string)value);
					else if(_Table._TableColumns[Index].GetColumnType == ColumnType.RAW)
						RowData[Index] = (byte[])value;
					else if(_Table._TableColumns[Index].GetColumnType == ColumnType.DATETIME)
						RowData[Index] = BitConverter.GetBytes(((DateTime)value).Ticks);
					else if(_Table._TableColumns[Index].GetColumnType == ColumnType.BOOL)
						RowData[Index] = BitConverter.GetBytes((bool)value);*/
				}
			}
		}
		public object this[string Column]
		{
			get
			{
				for( int i = 0; i < _Table._TableColumns.Count; i++ )
					if(_Table._TableColumns[i].Name == Column)
						return this[i];
				return null;
			}
			set
			{
				for( int i = 0; i < _Table._TableColumns.Count; i++ )
					if(_Table._TableColumns[i].Name == Column)
						this[i] = value;
			}
		}
		
		public DbTableIter( ref EsEmTable Tbl )
		{
			_Table = Tbl;
			DbTools.WaitForFile();
            Reload();
		}

        private void Reload()
        {
            try
            {
                Stream.Close();
            }
            catch { }
            RowNames.Clear();
            for (int i = 0; i < _Table._TableColumns.Count; i++)
                RowNames.Add(_Table._TableColumns[i].Name);
            Stream = new FileStream(_Table._Database._FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            Reader = new BinaryReader(Stream);
            Writer = new BinaryWriter(Stream);
            _IsNew = false;
		    _IsPrimed = false;
		    _IsDeleted = false;
            RowIndex = -1;
        }
		
		private void ReadData()
		{
			RowData.Clear();
			for( int i = 0; i < RowNames.Count; i++ )
			{
                if (_Table._Database.CacheTables && !BuildingCache)
                {
                    if (_Table._TableColumns[i].GetColumnType == ColumnType.INTEGER)
                        RowData.Add((long)_Table._TableCache[RowIndex][i]);
                    else if (_Table._TableColumns[i].GetColumnType == ColumnType.FLOAT)
                        RowData.Add((float)_Table._TableCache[RowIndex][i]);
                    else if (_Table._TableColumns[i].GetColumnType == ColumnType.TEXT)
                        RowData.Add((string)_Table._TableCache[RowIndex][i]);
                    else if (_Table._TableColumns[i].GetColumnType == ColumnType.RAW)
                        RowData.Add((byte[])_Table._TableCache[RowIndex][i]);
                    else if (_Table._TableColumns[i].GetColumnType == ColumnType.DATETIME)
                        RowData.Add((DateTime)_Table._TableCache[RowIndex][i]);
                    else if (_Table._TableColumns[i].GetColumnType == ColumnType.BOOL)
                        RowData.Add((bool)_Table._TableCache[RowIndex][i]);
                }
                else
                {
                    int DataSize = Reader.ReadInt32();
                    byte[] Data = Reader.ReadBytes(DataSize);
                    switch (_Table._TableColumns[i].GetColumnType)
                    {
                        case ColumnType.BOOL:
                            {
                                RowData.Add(BitConverter.ToBoolean(Data,0));
                            }break;
                        case ColumnType.DATETIME:
                            {
                                RowData.Add(new DateTime(BitConverter.ToInt64(Data, 0)));
                            }break;
                        case ColumnType.FLOAT:
                            {
                                RowData.Add(((float)BitConverter.ToDouble(Data, 0)));
                            }break;
                        case ColumnType.INTEGER:
                            {
                                RowData.Add(BitConverter.ToInt64(Data, 0));
                            }break;
                        case ColumnType.RAW:
                            {
                                RowData.Add(Data);
                            }break;
                        case ColumnType.TEXT:
                            {
                                RowData.Add(DbTools.ByteArrayToString(Data));
                            }break;
                    }
                }
			}
			
		}

        public bool SeekRowIndex(int Index)
        {
            First();
            for (int i = 0; i < Index; i++)
                if (!Read())
                    return false;
            return true;
        }
		
		private bool First()
		{		
			if(_Table._RowLink.First != -1)
			{
                RowIndex = 0;
				_CurrentHeader = new sRowHeader( _Table._RowLink.First, ref Stream );
				ReadData();
				_IsNew = false;
				_IsPrimed = true;
				return true;
			}

			_CurrentHeader = null;
			return false;
		}
		
		public void Close()
		{
			Stream.Close();
			DbTools.ReleaseFile();
		}
		
		
		public bool Read()
		{
			_IsDeleted = false;
			if( !_IsPrimed )
				return First();
            RowIndex++;
			sRowHeader nextRow = _CurrentHeader.GetNext( ref Stream );
			if(nextRow == null)
				return false;
			_CurrentHeader = nextRow;
			ReadData();
			_IsNew = false;
			return true;
		}

        public void Reset()
        {
            _IsPrimed = false;
        }
		
		public bool Prev()
		{
			_IsDeleted = false;
			sRowHeader prevRow = _CurrentHeader.GetPrev( ref Stream );
			if(prevRow == null)
				return false;
			_CurrentHeader = prevRow;
			ReadData();
			_IsNew = false;
            RowIndex--;
			return true;
		}
		
		private void UpdateTableHeader()
		{
			long cReadOffset = Stream.Position;
			_Table.WriteTableHeader( ref Stream, _Table._TableOffset );
			Stream.Seek(cReadOffset, SeekOrigin.Begin);
		}
		
		public bool Delete()
		{
			if(_Table._RowLink.First == _CurrentHeader._MyOffset)
			{
				if( _CurrentHeader._NextOffset != -1 )
					_Table._RowLink.First = _CurrentHeader._NextOffset;
				else
					_Table._RowLink.First = -1;
			}
			if(_Table._RowLink.Last == _CurrentHeader._MyOffset)
			{
				if( _CurrentHeader._PrevOffset != -1 )
					_Table._RowLink.Last = _CurrentHeader._PrevOffset;
				else
					_Table._RowLink.Prev = -1;
			}
			
			sRowHeader prevRow = _CurrentHeader.GetPrev( ref Stream );
			sRowHeader nextRow = _CurrentHeader.GetNext( ref Stream );
			
			if(prevRow != null)
			{
				if(nextRow != null)
					prevRow._NextOffset = nextRow._MyOffset;
				else
					prevRow._NextOffset = -1;
			}
			
			if(nextRow != null)
			{
				if(prevRow != null)
					nextRow._PrevOffset = prevRow._MyOffset;
				else
					nextRow._PrevOffset = -1;
			}
			
			if(prevRow != null)
				prevRow.Write(ref Stream);
			if(nextRow != null)
				nextRow.Write(ref Stream);
			UpdateTableHeader();
			_IsDeleted = true;

            if (_Table._Database.CacheTables)
                _Table._TableCache.RemoveAt(RowIndex);

			return true;
		}

        public bool DeleteSet(int[] Set)
        {
            Collection<sRowHeader> tHeaders = new Collection<sRowHeader>();
            Reset();
            int cIndex = 0;
            while(Read())
            {
                for(int i = 0; i < Set.Length; i++)
                    if(cIndex == Set[i])
                        tHeaders.Add(_CurrentHeader);
                cIndex++;
            }

            for (int i = 0; i < tHeaders.Count; i++)
            {
                Reload();
                bool Found = false;
                while (!Found)
                {
                    Read();
                    if(_CurrentHeader._MyOffset == tHeaders[i]._MyOffset)
                    {
                        Found = true;
                        Delete();
                    }
                }
            }

            /*
            Collection<sRowHeader> Headers = new Collection<sRowHeader>();
            for (int i = 0; i < Set.Length; i++)
            {
                SeekRowIndex(Set[i]);
                Headers.Add(_CurrentHeader);
            }

            for (int i = 0; i < Headers.Count; i++)
            {
                _CurrentHeader = Headers[i];
                Stream.Seek(_CurrentHeader._MyOffset, SeekOrigin.Begin);
                ReadData();
                _IsNew = false;
                Delete();
            }*/

            return true;
        }

        private void WriteRecord()
		{
			bool doUpdateHeader = false;
			long cReadOffset = Reader.BaseStream.Position;
			_CurrentHeader.Write( ref Stream );
			for( int i = 0; i < _Table.ColumnCount; i++ )
			{
				if(_IsNew && _Table._TableColumns[i].IsAutoIncrement && RowData[i] == null)
				{
					RowData[i] = _Table._TableColumns[i]._NextAutoIncrementId;
					_Table._TableColumns[i]._NextAutoIncrementId++;
					doUpdateHeader = true;
				}
                byte[] WriteBytes = new byte[0];

                switch (_Table._TableColumns[i].GetColumnType)
                {
                    case ColumnType.BOOL:
                        {
                            WriteBytes = BitConverter.GetBytes((bool)RowData[i]);
                        }break;
                    case ColumnType.DATETIME:
                        {
                            WriteBytes = BitConverter.GetBytes(((DateTime)RowData[i]).Ticks);
                        }break;
                    case ColumnType.FLOAT:
                        {
                            WriteBytes = BitConverter.GetBytes((float)RowData[i]);
                        }break;
                    case ColumnType.INTEGER:
                        {
                            WriteBytes = BitConverter.GetBytes((long)RowData[i]);
                        }break;
                    case ColumnType.RAW:
                        {
                            WriteBytes = (byte[])RowData[i];
                        }break;
                    case ColumnType.TEXT:
                        {
                            WriteBytes = DbTools.StringToByteArray((string)RowData[i]);
                        }break;
                }

                Writer.Write(WriteBytes.Length);
                Writer.Write(WriteBytes);
			}
			Stream.Seek(cReadOffset, SeekOrigin.Begin);
			if(doUpdateHeader)
				UpdateTableHeader();
		}
		
		public bool Update()
		{
			long OldOffset = _CurrentHeader._MyOffset;
			
			sRowHeader prevRow = _CurrentHeader.GetPrev( ref Stream );
			sRowHeader nextRow = _CurrentHeader.GetNext( ref Stream );
			
			_CurrentHeader._MyOffset = Reader.BaseStream.Length;
			if(prevRow != null)
				prevRow._NextOffset = _CurrentHeader._MyOffset;
			if(nextRow != null)
				nextRow._PrevOffset = _CurrentHeader._MyOffset;
			
			if(prevRow != null)
				prevRow.Write( ref Stream );
			if(nextRow != null)
				nextRow.Write( ref Stream );
			
			WriteRecord();	
			
			bool doWriteHeader = false;
			if(_Table._RowLink.First == OldOffset)
			{
				_Table._RowLink.First = _CurrentHeader._MyOffset;
				doWriteHeader = true;
			}
			if(_Table._RowLink.Last == OldOffset)
			{
				_Table._RowLink.Last = _CurrentHeader._MyOffset;
				doWriteHeader = true;
			}
			
			if(doWriteHeader)
				UpdateTableHeader();

            if (_Table._Database.CacheTables)
            {
                for (int i = 0; i < _Table._TableColumns.Count; i++)
                    _Table._TableCache[RowIndex][i] = this[i];
            }
			
			return true;
		}
		
		public bool StartNew()
		{
			RowData.Clear();
			for( int i = 0; i < _Table.ColumnCount; i++ )
				RowData.Add( null );
			_CurrentHeader = new sRowHeader();
			_CurrentHeader._MyOffset = Reader.BaseStream.Length;
			_CurrentHeader._PrevOffset = _Table._RowLink.Last;
			_IsNew = true;
			return true;
		}
		
		public bool CancelNew()
		{
			First();
			return true;
		}
		
		public bool CommitNew()
		{

			//Special Case, First Record
			if( _Table._RowLink.First == -1 && _Table._RowLink.Last == -1)
			{
				WriteRecord();
                if (_Table._Database.CacheTables)
                {
                    Collection<object> newRow = new Collection<object>();
                    for (int i = 0; i < _Table._TableColumns.Count; i++)
                        newRow.Add(this[i]);
                    _Table._TableCache.Add(newRow);
                }
				_Table._RowLink.First = _CurrentHeader._MyOffset;
				_Table._RowLink.Last = _CurrentHeader._MyOffset;
				UpdateTableHeader();
				First();
			}
			else
			{
				_CurrentHeader._PrevOffset = _Table._RowLink.Last;
				_Table._RowLink.Last = _CurrentHeader._MyOffset;
				WriteRecord();
                if (_Table._Database.CacheTables)
                {
                    Collection<object> newRow = new Collection<object>();
                    for (int i = 0; i < _Table._TableColumns.Count; i++)
                        newRow.Add(this[i]);
                    _Table._TableCache.Add(newRow);
                }
				sRowHeader prevRow = _CurrentHeader.GetPrev( ref Stream );
				prevRow._NextOffset = _CurrentHeader._MyOffset;
				prevRow.Write( ref Stream );
				UpdateTableHeader();
				Read();	
			}

			return true;
		}

        internal int[] WhereMatches(string ColumnName, ColumnMatchOperation Op, object Value)
        {
            Collection<int> cReturnValues = new Collection<int>();
            Reset();
            while (Read())
            {
                switch (Op)
                {
                    case ColumnMatchOperation.IsNull:
                        {
                            if (this[ColumnName] == null)
                                cReturnValues.Add(RowIndex);
                        }break;
                    case ColumnMatchOperation.IsNotNull:
                        {
                            if (this[ColumnName] != null)
                                cReturnValues.Add(RowIndex);
                        }break;
                    case ColumnMatchOperation.Equal:
                        {
                            if (this[ColumnName] == null)
                                cReturnValues.Add(RowIndex);
                            else
                            {
                                switch (_Table[ColumnName].GetColumnType)
                                {
                                    case ColumnType.BOOL:
                                        {
                                            if (Convert.ToBoolean(Value) == Convert.ToBoolean(this[ColumnName]))
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                    case ColumnType.DATETIME:
                                        {
                                            if (Convert.ToDateTime(Value) == Convert.ToDateTime(this[ColumnName]))
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                    case ColumnType.FLOAT:
                                        {
                                            if (((float)Convert.ToDouble(Value)) == ((float)Convert.ToDouble(this[ColumnName])))
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                    case ColumnType.INTEGER:
                                        {
                                            if (Convert.ToInt64(Value) == Convert.ToInt64(this[ColumnName]))
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                    case ColumnType.TEXT:
                                        {
                                            if (string.Compare(Convert.ToString(Value), Convert.ToString(this[ColumnName])) == 0)
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                }
                            }
                        } break;
                    case ColumnMatchOperation.NotEqual:
                        {
                            if (this[ColumnName] != null)
                            {
                                switch (_Table[ColumnName].GetColumnType)
                                {
                                    case ColumnType.BOOL:
                                        {
                                            if (Convert.ToBoolean(Value) != Convert.ToBoolean(this[ColumnName]))
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                    case ColumnType.DATETIME:
                                        {
                                            if (Convert.ToDateTime(Value) != Convert.ToDateTime(this[ColumnName]))
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                    case ColumnType.FLOAT:
                                        {
                                            if (((float)Convert.ToDouble(Value)) != ((float)Convert.ToDouble(this[ColumnName])))
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                    case ColumnType.INTEGER:
                                        {
                                            if (Convert.ToInt64(Value) != Convert.ToInt64(this[ColumnName]))
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                    case ColumnType.TEXT:
                                        {
                                            if (string.Compare(Convert.ToString(Value), Convert.ToString(this[ColumnName])) != 0)
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                }
                            }
                        } break;
                    case ColumnMatchOperation.LessThan:
                        {
                            if (this[ColumnName] != null)
                            {
                                switch (_Table[ColumnName].GetColumnType)
                                {
                                    case ColumnType.BOOL:
                                        {
                                            throw new Exception(" > Test Invalid For Boolean Type.");
                                        }
                                    case ColumnType.DATETIME:
                                        {
                                            if (Convert.ToDateTime(Value) > Convert.ToDateTime(this[ColumnName]))
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                    case ColumnType.FLOAT:
                                        {
                                            if (((float)Convert.ToDouble(Value)) > ((float)Convert.ToDouble(this[ColumnName])))
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                    case ColumnType.INTEGER:
                                        {
                                            if (Convert.ToInt64(Value) > Convert.ToInt64(this[ColumnName]))
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                    case ColumnType.TEXT:
                                        {
                                            if (string.Compare(Convert.ToString(Value), Convert.ToString(this[ColumnName])) > 0)
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                }
                            }
                        } break;
                    case ColumnMatchOperation.LessThanOrEqualTo:
                        {
                            if (this[ColumnName] != null)
                            {
                                switch (_Table[ColumnName].GetColumnType)
                                {
                                    case ColumnType.BOOL:
                                        {
                                            throw new Exception(" >= Test Invalid For Boolean Type.");
                                        }
                                    case ColumnType.DATETIME:
                                        {
                                            if (Convert.ToDateTime(Value) >= Convert.ToDateTime(this[ColumnName]))
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                    case ColumnType.FLOAT:
                                        {
                                            if (((float)Convert.ToDouble(Value)) >= ((float)Convert.ToDouble(this[ColumnName])))
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                    case ColumnType.INTEGER:
                                        {
                                            if (Convert.ToInt64(Value) >= Convert.ToInt64(this[ColumnName]))
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                    case ColumnType.TEXT:
                                        {
                                            if (string.Compare(Convert.ToString(Value), Convert.ToString(this[ColumnName])) >= 0)
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                }
                            }
                        } break;
                    case ColumnMatchOperation.GreaterThan:
                        {
                            if (this[ColumnName] != null)
                            {
                                switch (_Table[ColumnName].GetColumnType)
                                {
                                    case ColumnType.BOOL:
                                        {
                                            throw new Exception(" < Test Invalid For Boolean Type.");
                                        }
                                    case ColumnType.DATETIME:
                                        {
                                            if (Convert.ToDateTime(Value) < Convert.ToDateTime(this[ColumnName]))
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                    case ColumnType.FLOAT:
                                        {
                                            if (((float)Convert.ToDouble(Value)) < ((float)Convert.ToDouble(this[ColumnName])))
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                    case ColumnType.INTEGER:
                                        {
                                            if (Convert.ToInt64(Value) < Convert.ToInt64(this[ColumnName]))
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                    case ColumnType.TEXT:
                                        {
                                            if (string.Compare(Convert.ToString(Value), Convert.ToString(this[ColumnName])) < 0)
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                }
                            }
                        } break;
                    case ColumnMatchOperation.GreaterThanOrEqualTo:
                        {
                            if (this[ColumnName] != null)
                            {
                                switch (_Table[ColumnName].GetColumnType)
                                {
                                    case ColumnType.BOOL:
                                        {
                                            throw new Exception(" <= Test Invalid For Boolean Type.");
                                        }
                                    case ColumnType.DATETIME:
                                        {
                                            if (Convert.ToDateTime(Value) <= Convert.ToDateTime(this[ColumnName]))
                                                cReturnValues.Add(RowIndex); ;
                                        }break;
                                    case ColumnType.FLOAT:
                                        {
                                            if (((float)Convert.ToDouble(Value)) <= ((float)Convert.ToDouble(this[ColumnName])))
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                    case ColumnType.INTEGER:
                                        {
                                            if (Convert.ToInt64(Value) <= Convert.ToInt64(this[ColumnName]))
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                    case ColumnType.TEXT:
                                        {
                                            if (string.Compare(Convert.ToString(Value), Convert.ToString(this[ColumnName])) <= 0)
                                                cReturnValues.Add(RowIndex);
                                        }break;
                                }
                            }
                        } break;
                }
            }

            int[] rValue = new int[cReturnValues.Count];
            cReturnValues.CopyTo(rValue, 0);
            return rValue;
        }

        internal static int[] OrResults(int[] Set1, int[] Set2)
        {
            Collection<int> Results = new Collection<int>();
            for (int i = 0; i < Set1.Length; i++)
                Results.Add(Set1[i]);
            for (int i = 0; i < Set2.Length; i++)
            {
                bool Found = false;
                for (int e = 0; e < Results.Count; e++)
                    if (Results[e] == Set2[i])
                        Found = true;
                if (!Found)
                    Results.Add(Set2[i]);
            }

            int[] rValue = new int[Results.Count];
            Results.CopyTo(rValue, 0);
            return rValue;
        }

        internal static int[] AndResults(int[] Set1, int[] Set2)
        {
            Collection<int> Results = new Collection<int>();
            if (Set1.Length > Set2.Length)
            {
                for (int i = 0; i < Set1.Length; i++)
                    for (int e = 0; e < Set2.Length; e++)
                        if (Set1[i] == Set2[e])
                            Results.Add(Set1[i]);
            }
            else
            {
                for (int i = 0; i < Set2.Length; i++)
                    for (int e = 0; e < Set1.Length; e++)
                        if (Set2[i] == Set1[e])
                            Results.Add(Set2[i]);
            }

            int[] rValue = new int[Results.Count];
            Results.CopyTo(rValue, 0);
            return rValue;
        }

        internal void ReBuildCache()
        {
            BuildingCache = true;
            _Table._TableCache.Clear();
            Reset();
            while (Read())
            {
                Collection<object> tRow = new Collection<object>();
                for (int i = 0; i < _Table._TableColumns.Count; i++)
                    tRow.Add(this[i]);
                _Table._TableCache.Add(tRow);
            }
            BuildingCache = false;

        }

        internal int[] WhereMatchesAnd(string ColumnName, ColumnMatchOperation Op, object Value, int[] PrevResults)
        {
            Collection<int> cReturnValues = new Collection<int>();
            int[] NewResults = WhereMatches(ColumnName, Op, Value);
            return DbTableIter.AndResults(NewResults, PrevResults);
        }

        internal int[] WhereMatchesOr(string ColumnName, ColumnMatchOperation Op, object Value, int[] PrevResults)
        {
            Collection<int> cReturnValues = new Collection<int>();
            int[] NewResults = WhereMatches(ColumnName, Op, Value);
            return DbTableIter.OrResults(NewResults, PrevResults);
        }

        internal bool MatchesValue(string ColumnName, ColumnMatchOperation Op, object Value)
		{
			switch( Op )
			{
			case ColumnMatchOperation.IsNull:
			{
				if( this[ColumnName] == null )
					return true;
				return false;
			}
			case ColumnMatchOperation.IsNotNull:
			{
				if( this[ColumnName] != null )
					return true;
				return false;				
			}
			case ColumnMatchOperation.Equal:
			{
				if( this[ColumnName] == null )
					return false;
				else
				{
					switch(_Table[ColumnName].GetColumnType)
					{
					case ColumnType.BOOL:
					{
						if( Convert.ToBoolean(Value) == Convert.ToBoolean(this[ColumnName]) )
							return true;
						return false;
					}
					case ColumnType.DATETIME:
					{
						if( Convert.ToDateTime(Value) == Convert.ToDateTime(this[ColumnName]) )
							return true;
						return false;
					}
					case ColumnType.FLOAT:
					{
						if( ((float)Convert.ToDouble(Value)) == ((float)Convert.ToDouble(this[ColumnName])) )
							return true;
						return false;
					}
					case ColumnType.INTEGER:
					{
						if( Convert.ToInt64(Value) == Convert.ToInt64(this[ColumnName]) )
							return true;
						return false;
					}
					case ColumnType.TEXT:
					{
						if( string.Compare( Convert.ToString(Value), Convert.ToString(this[ColumnName]) ) == 0 )
							return true;
						return false;
					}
					}
				}
			}break;
			case ColumnMatchOperation.NotEqual:
			{
				if( this[ColumnName] == null )
					return false;
				else
				{
					switch(_Table[ColumnName].GetColumnType)
					{
					case ColumnType.BOOL:
					{
						if( Convert.ToBoolean(Value) != Convert.ToBoolean(this[ColumnName]) )
							return true;
						return false;
					}
					case ColumnType.DATETIME:
					{
						if( Convert.ToDateTime(Value) != Convert.ToDateTime(this[ColumnName]) )
							return true;
						return false;
					}
					case ColumnType.FLOAT:
					{
						if( ((float)Convert.ToDouble(Value)) != ((float)Convert.ToDouble(this[ColumnName])) )
							return true;
						return false;
					}
					case ColumnType.INTEGER:
					{
						if( Convert.ToInt64(Value) != Convert.ToInt64(this[ColumnName]) )
							return true;
						return false;
					}
					case ColumnType.TEXT:
					{
						if( string.Compare( Convert.ToString(Value), Convert.ToString(this[ColumnName]) ) != 0 )
							return true;
						return false;
					}
					}
				}
			}break;
			case ColumnMatchOperation.LessThan:
			{
				if( this[ColumnName] == null )
					return false;
				else
				{
					switch(_Table[ColumnName].GetColumnType)
					{
					case ColumnType.BOOL:
					{
						throw new Exception(" > Test Invalid For Boolean Type.");
					}
					case ColumnType.DATETIME:
					{
						if( Convert.ToDateTime(Value) > Convert.ToDateTime(this[ColumnName]) )
							return true;
						return false;
					}
					case ColumnType.FLOAT:
					{
						if( ((float)Convert.ToDouble(Value)) > ((float)Convert.ToDouble(this[ColumnName])) )
							return true;
						return false;
					}
					case ColumnType.INTEGER:
					{
						if( Convert.ToInt64(Value) > Convert.ToInt64(this[ColumnName]) )
							return true;
						return false;
					}
					case ColumnType.TEXT:
					{
						if( string.Compare( Convert.ToString(Value), Convert.ToString(this[ColumnName]) ) > 0 )
							return true;
						return false;
					}
					}
				}
			}break;
			case ColumnMatchOperation.LessThanOrEqualTo:
			{
				if( this[ColumnName] == null )
					return false;
				else
				{
					switch(_Table[ColumnName].GetColumnType)
					{
					case ColumnType.BOOL:
					{
						throw new Exception(" >= Test Invalid For Boolean Type.");
					}
					case ColumnType.DATETIME:
					{
						if( Convert.ToDateTime(Value) >= Convert.ToDateTime(this[ColumnName]) )
							return true;
						return false;
					}
					case ColumnType.FLOAT:
					{
						if( ((float)Convert.ToDouble(Value)) >= ((float)Convert.ToDouble(this[ColumnName])) )
							return true;
						return false;
					}
					case ColumnType.INTEGER:
					{
						if( Convert.ToInt64(Value) >= Convert.ToInt64(this[ColumnName]) )
							return true;
						return false;
					}
					case ColumnType.TEXT:
					{
						if( string.Compare( Convert.ToString(Value), Convert.ToString(this[ColumnName]) ) >= 0 )
							return true;
						return false;
					}
					}
				}
			}break;
			case ColumnMatchOperation.GreaterThan:
			{
				if( this[ColumnName] == null )
					return false;
				else
				{
					switch(_Table[ColumnName].GetColumnType)
					{
					case ColumnType.BOOL:
					{
						throw new Exception(" < Test Invalid For Boolean Type.");
					}
					case ColumnType.DATETIME:
					{
						if( Convert.ToDateTime(Value) < Convert.ToDateTime(this[ColumnName]) )
							return true;
						return false;
					}
					case ColumnType.FLOAT:
					{
						if( ((float)Convert.ToDouble(Value)) < ((float)Convert.ToDouble(this[ColumnName])) )
							return true;
						return false;
					}
					case ColumnType.INTEGER:
					{
						if( Convert.ToInt64(Value) < Convert.ToInt64(this[ColumnName]) )
							return true;
						return false;
					}
					case ColumnType.TEXT:
					{
						if( string.Compare( Convert.ToString(Value), Convert.ToString(this[ColumnName]) ) < 0 )
							return true;
						return false;
					}
					}
				}
			}break;
			case ColumnMatchOperation.GreaterThanOrEqualTo:
			{
				if( this[ColumnName] == null )
					return false;
				else
				{
					switch(_Table[ColumnName].GetColumnType)
					{
					case ColumnType.BOOL:
					{
						throw new Exception(" <= Test Invalid For Boolean Type.");
					}
					case ColumnType.DATETIME:
					{
						if( Convert.ToDateTime(Value) <= Convert.ToDateTime(this[ColumnName]) )
							return true;
						return false;
					}
					case ColumnType.FLOAT:
					{
						if( ((float)Convert.ToDouble(Value)) <= ((float)Convert.ToDouble(this[ColumnName])) )
							return true;
						return false;
					}
					case ColumnType.INTEGER:
					{
						if( Convert.ToInt64(Value) <= Convert.ToInt64(this[ColumnName]) )
							return true;
						return false;
					}
					case ColumnType.TEXT:
					{
						if( string.Compare( Convert.ToString(Value), Convert.ToString(this[ColumnName]) ) <= 0 )
							return true;
						return false;
					}
					}
				}
			}break;
			}
			return false;
		}
	}
}
