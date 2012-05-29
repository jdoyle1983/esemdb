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

    internal struct SystemVar
    {
        public long VarId;
        public string VarName;
        public string VarValue;
    }

	public class EsEmDatabase
	{
		internal string _FilePath;
		internal Collection<EsEmTable> _Tables = new Collection<EsEmTable>();
		internal DbHeader _Header = new DbHeader();
        internal Collection<SystemVar> SysVars = new Collection<SystemVar>();
        private bool _CacheTables = false;

        public bool CacheTables
        {
            get
            {
                return _CacheTables;
            }
        }

		public int TableCount
		{
			get
			{
				return _Tables.Count;
			}
		}
		public EsEmTable this[int Index]
		{
			get
			{
				if(Index >= _Tables.Count || Index < 0 )
					throw new Exception("Table Index out of bounds");
				return _Tables[Index];
			}
		}
		public EsEmTable this[string TableName]
		{
			get
			{
				for( int i = 0; i < _Tables.Count; i++ )
					if(_Tables[i].Name == TableName)
						return _Tables[i];
				throw new Exception("Table '" + TableName + "' Not Found");
			}
		}
		
		
		public void OpenDatabase( string FilePath )
		{
			_FilePath = FilePath;
			FileStream Stream = null;
			try
			{
				Stream = File.OpenRead(FilePath);
				_Header.ReadHeader( ref Stream );
				if(_Header.Link.First != -1)
				{
					EsEmTable tbl = new EsEmTable();
					tbl.ReadTableHeader( ref Stream, _Header.Link.First );
					tbl._Database = this;
					_Tables.Add(tbl);
					while(tbl._TableLink.Next != -1)
					{
						long NewOffset = tbl._TableLink.Next;
						tbl = new EsEmTable();
						tbl.ReadTableHeader( ref Stream, NewOffset );
						tbl._Database = this;
						_Tables.Add(tbl);
					}
				}
				Stream.Close();

                string Query = "select * from $System";
                EsEmQuery q = CreateQuery(Query);
                EsEmResult Res = q.Execute();
                SysVars.Clear();
                while (Res.Read())
                {
                    SystemVar v = new SystemVar();
                    v.VarId = Convert.ToInt64(Res["id"]);
                    v.VarName = Convert.ToString(Res["varname"]);
                    v.VarValue = Convert.ToString(Res["varvalue"]);
                    SysVars.Add(v);
                }
                
                bool ShouldEnableCache = false;

                for (int i = 0; i < SysVars.Count; i++)
                    if (SysVars[i].VarName == "CacheRows")
                        if (SysVars[i].VarValue == "Yes")
                            ShouldEnableCache = true;

                if (ShouldEnableCache)
                {
                    EnableCacheTables();
                }
			}
			catch (Exception e)
			{
				if(e.Message != "NOTDB")
				{
					if(Stream != null)
						Stream.Close();
					Stream = File.OpenWrite(FilePath);
					_Header.DbVersionMajor = 1;
					_Header.DbVersionMinor = 0;
					_Header.WriteHeader(ref Stream);
					Stream.Close();
                    string Query = "CREATE TABLE $System ( id integer primary key auto increment, varname text, varvalue text );";
                    EsEmQuery q = CreateQuery(Query);
                    q.Execute();
                    Query = "INSERT INTO $System (varname,varvalue) values ('CacheRows','No')";
                    q = CreateQuery(Query);
                    q.Execute();
                    Query = "select * from $System";
                    q = CreateQuery(Query);
                    EsEmResult Res = q.Execute();
                    SysVars.Clear();
                    while (Res.Read())
                    {
                        SystemVar v = new SystemVar();
                        v.VarId = Convert.ToInt64(Res["id"]);
                        v.VarName = Convert.ToString(Res["varname"]);
                        v.VarValue = Convert.ToString(Res["varvalue"]);
                        SysVars.Add(v);
                    }
				}
				else
					throw new Exception("File Not EsEm Database!");
			}
			
			
		}

        public void DumpSchema()
        {
            for (int i = 0; i < _Tables.Count; i++)
            {
                if (!_Tables[i].Name.StartsWith("$"))
                {
                    Console.WriteLine("CREATE TABLE " + _Tables[i].Name);
                    Console.WriteLine("(");

                    for (int e = 0; e < _Tables[i]._TableColumns.Count; e++)
                    {
                        string OutLine = _Tables[i]._TableColumns[e].Name;
                        OutLine += " " + DbTools.GetColumnTypeName(_Tables[i]._TableColumns[e].GetColumnType);
                        if (_Tables[i]._TableColumns[e].IsPrimaryKey)
                            OutLine += " PRIMARY KEY";
                        if (_Tables[i]._TableColumns[e].IsAutoIncrement)
                            OutLine += " AUTO INCREMENT";
                        if (e < (_Tables[i]._TableColumns.Count - 1))
                            OutLine += ",";
                        Console.WriteLine(OutLine);
                    }
                    Console.WriteLine(");");
                }
            }
        }

        public EsEmTable CreateTable(string TableName)
		{
			for( int i = 0; i < _Tables.Count; i++ )
				if( _Tables[i].Name == TableName )
					throw new Exception("Table '" + TableName + "' Already Exists in Datbase.");
			EsEmTable t = EsEmTable.CreateTable( this, TableName );
			return t;
		}

        public void DropTable(string TableName)
        {
            for (int i = 0; i < _Tables.Count; i++)
            {
                if (_Tables[i].Name == TableName)
                {
                    if (_Tables[i]._TableLink.Prev != -1)
                    {
                        for (int e = 0; e < _Tables.Count; e++)
                            if (_Tables[e]._TableOffset == _Tables[i]._TableLink.Prev)
                                _Tables[e]._TableLink.Next = _Tables[i]._TableLink.Next;
                    }
                    else
                    {
                        _Header.Link.First = -1;
                    }
                    if (_Tables[i]._TableLink.Next != -1)
                    {
                        for (int e = 0; e < _Tables.Count; e++)
                            if (_Tables[e]._TableOffset == _Tables[i]._TableLink.Next)
                                _Tables[e]._TableLink.Prev = _Tables[i]._TableLink.Prev;
                    }
                    else
                    {
                        _Header.Link.Last = -1;
                    }
                    _Tables.RemoveAt(i);
                }
            }
            DbTools.WaitForFile();
            FileStream Stream = File.OpenWrite(_FilePath);
            WriteTableHeaders(ref Stream);
            Stream.Close();
            DbTools.ReleaseFile();
        }

        public EsEmQuery CreateQuery(string Sql)
		{
			return new EsEmQuery(Sql, this);
		}
		
		public void CompactDatabase()
		{
			//DbTools.WaitForFile();
			
			//string TempFile = Path.GetTempFileName();
			//Consolidate
			
			
			DbTools.ReleaseFile();
		}

        public void EnableCacheTables()
        {
            if (!_CacheTables)
            {
                _CacheTables = true;
                for (int i = 0; i < _Tables.Count; i++)
                {
                    EsEmTable tbl = this[_Tables[i].Name];
                    DbTableIter itr = new DbTableIter(ref tbl);
                    itr.ReBuildCache();
                    itr.Close();
                }

                string Sql = "update $System set varvalue='Yes' where varname='CacheRows'";
                EsEmQuery q = CreateQuery(Sql);
                q.Execute();
            }
        }

        public void DisableCacheTables()
        {
            if (_CacheTables)
            {
                _CacheTables = false;

                string Sql = "update $System set varvalue='No' where varname='CacheRows'";
                EsEmQuery q = CreateQuery(Sql);
                q.Execute();
            }
        }
		
		internal void WriteTableHeaders( ref FileStream Stream )
		{
			for( int i = 0; i < _Tables.Count; i++ )
			{
				_Tables[i]._TableLink.First = _Header.Link.First;
				_Tables[i]._TableLink.Last = _Header.Link.Last;
				_Tables[i].WriteTableHeader( ref Stream, _Tables[i]._TableOffset );
			}
			
			_Header.WriteHeader( ref Stream );
				
		}
	}
}
