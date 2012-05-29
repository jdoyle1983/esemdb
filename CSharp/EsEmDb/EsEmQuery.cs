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
using System.IO;
using System.Collections.ObjectModel;
using System.Xml;


namespace EsEmDb
{
	internal class MatchClause
	{
		public string ColumnName = "";
		public ColumnMatchOperation Operation = ColumnMatchOperation.Invalid;
		public object Value = null;
	}
		
	public class EsEmQuery
	{		
		private EsEmDatabase _Database;
		private string _QueryText;
		
		public string QueryStr
		{
			get
			{
				return _QueryText;
			}
			set
			{
				_QueryText = value;
			}
		}
		public EsEmDatabase Database
		{
			get
			{
				return _Database;
			}
			set
			{
				_Database = value;
			}
		}
		
		private void InvalidQuery()
		{
			throw new Exception("Invalid Query");
		}
		
		private void InvalidQuery( string Reason )
		{
			throw new Exception("Invalid Query: " + Reason);
		}
		
		public EsEmQuery()
		{
		}
		
		public EsEmQuery( string QueryStr )
		{
			_QueryText = QueryStr;
		}
		
		public EsEmQuery( string QueryStr, EsEmDatabase Database )
		{
			_QueryText = QueryStr;
			_Database = Database;
		}

        internal int[] _RecurseParseWhereClause(ref EsEmTable tbl, ref SimpleParser p)
        {
            int[] Set1 = null;
            ParseToken Tok = p.NextToken();
            while (Tok != null && Tok.TokenType != ParseTokenType.CloseBrace)
            {
                int[] Set2 = null;
                if (Set1 == null)
                {
                    ParseToken pTok = p.PeekToken();
                    if (pTok.TokenType == ParseTokenType.OpenBrace)
                        Set1 = _RecurseParseWhereClause(ref tbl, ref p);
                    else
                    {
                        string ColumnName = "";
                        ColumnMatchOperation Op = ColumnMatchOperation.Invalid;
                        string strValue = "";
                        Tok = p.NextToken();
                        ColumnName = Tok.TokenValue;
                        Tok = p.NextToken();
                        switch (Tok.TokenType)
                        {
                            case ParseTokenType.Equal:
                                {
                                    Op = ColumnMatchOperation.Equal;
                                } break;
                            case ParseTokenType.GreaterThan:
                                {
                                    Op = ColumnMatchOperation.GreaterThan;
                                } break;
                            case ParseTokenType.GreaterThanOrEqualTo:
                                {
                                    Op = ColumnMatchOperation.GreaterThanOrEqualTo;
                                } break;
                            case ParseTokenType.IsNotNull:
                                {
                                    Op = ColumnMatchOperation.IsNotNull;
                                } break;
                            case ParseTokenType.IsNull:
                                {
                                    Op = ColumnMatchOperation.IsNull;
                                } break;
                            case ParseTokenType.LessThan:
                                {
                                    Op = ColumnMatchOperation.LessThan;
                                } break;
                            case ParseTokenType.LessThanOrEqualTo:
                                {
                                    Op = ColumnMatchOperation.LessThanOrEqualTo;
                                } break;
                            case ParseTokenType.NotEqual:
                                {
                                    Op = ColumnMatchOperation.NotEqual;
                                } break;
                        }
                        Tok = p.NextToken();
                        strValue = Tok.TokenValue;
                        DbTableIter itr = new DbTableIter(ref tbl);
                        itr.Read();
                        Set1 = itr.WhereMatches(ColumnName, Op, strValue);
                        itr.Close();
                    }
                }

                bool AdditionalOp = false;
                bool IsAnd = false;
                ParseToken tokOp = p.PeekToken();
                if (tokOp != null && (tokOp.TokenType == ParseTokenType.And || tokOp.TokenType == ParseTokenType.Or))
                {
                    AdditionalOp = true;
                    Tok = p.NextToken();
                    if (Tok.TokenType == ParseTokenType.And)
                        IsAnd = true;
                    else
                        IsAnd = false;
                }

                if (AdditionalOp)
                {
                    ParseToken pTok = p.PeekToken();
                    if (pTok.TokenType == ParseTokenType.OpenBrace)
                    {
                        Set2 = _RecurseParseWhereClause(ref tbl, ref p);
                        if (IsAnd)
                            Set1 = DbTableIter.AndResults(Set1, Set2);
                        else
                            Set1 = DbTableIter.OrResults(Set1, Set2);
                    }
                    else
                    {
                        string ColumnName = "";
                        ColumnMatchOperation Op = ColumnMatchOperation.Invalid;
                        string strValue = "";
                        Tok = p.NextToken();
                        ColumnName = Tok.TokenValue;
                        Tok = p.NextToken();
                        switch (Tok.TokenType)
                        {
                            case ParseTokenType.Equal:
                                {
                                    Op = ColumnMatchOperation.Equal;
                                } break;
                            case ParseTokenType.GreaterThan:
                                {
                                    Op = ColumnMatchOperation.GreaterThan;
                                } break;
                            case ParseTokenType.GreaterThanOrEqualTo:
                                {
                                    Op = ColumnMatchOperation.GreaterThanOrEqualTo;
                                } break;
                            case ParseTokenType.IsNotNull:
                                {
                                    Op = ColumnMatchOperation.IsNotNull;
                                } break;
                            case ParseTokenType.IsNull:
                                {
                                    Op = ColumnMatchOperation.IsNull;
                                } break;
                            case ParseTokenType.LessThan:
                                {
                                    Op = ColumnMatchOperation.LessThan;
                                } break;
                            case ParseTokenType.LessThanOrEqualTo:
                                {
                                    Op = ColumnMatchOperation.LessThanOrEqualTo;
                                } break;
                            case ParseTokenType.NotEqual:
                                {
                                    Op = ColumnMatchOperation.NotEqual;
                                } break;
                        }
                        Tok = p.NextToken();
                        strValue = Tok.TokenValue;
                        DbTableIter itr = new DbTableIter(ref tbl);
                        itr.Read();
                        Set2 = itr.WhereMatches(ColumnName, Op, strValue);
                        itr.Close();

                        if (IsAnd)
                            Set1 = DbTableIter.AndResults(Set1, Set2);
                        else
                            Set1 = DbTableIter.OrResults(Set1, Set2);
                    }
                }
                Tok = p.NextToken();
            }
            return Set1;
        }

        internal EsEmResult _ExecuteCreateTable(ref SimpleParser p, DateTime QueryStart, TimeSpan ParseTime)
        {
            EsEmResult Res = new EsEmResult();
            Res._ColumnNames.Add("Result");
            Res._ColumnTypes.Add(ColumnType.TEXT);
            Collection<object> Results = new Collection<object>();
            bool HasError = false;
            string ErrorMessage = "";

            Res._ParseTime = ParseTime;
            DateTime BuildStartTime = DateTime.Now;

            ParseToken CreateTableTok = p.NextToken();
            ParseToken TableNameTok = p.NextToken();
            ParseToken OpenBrace = p.NextToken();

            string TableName = TableNameTok.TokenValue;

            bool TableFound = false;
            for (int i = 0; i < _Database._Tables.Count; i++)
            {
                if (_Database._Tables[i].Name == TableName)
                {
                    TableFound = true;
                    break;
                }
            }

            if (!TableFound)
            {

                EsEmTable tbl = _Database.CreateTable(TableName);

                ParseToken tok = p.NextToken();
                while (tok != null && tok.TokenType != ParseTokenType.CloseBrace)
                {
                    string ColumnName = tok.TokenValue;
                    ColumnType cType = ColumnType.INVALID;
                    bool PrimaryKey = false;
                    bool AutoIncrement = false;
                    tok = p.NextToken();
                    switch (tok.TokenType)
                    {
                        case ParseTokenType.Boolean:
                            {
                                cType = ColumnType.BOOL;
                            } break;
                        case ParseTokenType.DateTime:
                            {
                                cType = ColumnType.DATETIME;
                            } break;
                        case ParseTokenType.Float:
                            {
                                cType = ColumnType.FLOAT;
                            } break;
                        case ParseTokenType.Integer:
                            {
                                cType = ColumnType.INTEGER;
                            } break;
                        case ParseTokenType.Raw:
                            {
                                cType = ColumnType.RAW;
                            } break;
                        case ParseTokenType.Text:
                            {
                                cType = ColumnType.TEXT;
                            } break;
                    }

                    tok = p.NextToken();
                    if (tok.TokenType == ParseTokenType.PrimaryKey || tok.TokenType == ParseTokenType.AutoIncrement)
                    {
                        if (tok.TokenType == ParseTokenType.PrimaryKey)
                            PrimaryKey = true;
                        else
                            AutoIncrement = true;
                        tok = p.NextToken();
                    }
                    if (tok.TokenType == ParseTokenType.PrimaryKey || tok.TokenType == ParseTokenType.AutoIncrement)
                    {
                        if (tok.TokenType == ParseTokenType.PrimaryKey)
                            PrimaryKey = true;
                        else
                            AutoIncrement = true;
                        tok = p.NextToken();
                    }
                    tbl.CreateColumn(ColumnName, cType, AutoIncrement, PrimaryKey);
                }

                tbl.CommitTable();
                Res._BuildTime = DateTime.Now - BuildStartTime;
                Res._TotalQueryTime = DateTime.Now - QueryStart;
            }
            else
            {
                HasError = true;
                ErrorMessage = "Table '" + TableName + "' Already Exists";
            }

            if (HasError)
            {
                Res._ColumnNames.Add((string)"Error");
                Res._ColumnTypes.Add(ColumnType.TEXT);
                Results.Add((string)"Error");
                Results.Add(ErrorMessage);
            }
            else
                Results.Add((string)"OK");
            Res._ColumnValues.Add(Results);
            return Res;
        }

        internal EsEmResult _ExecuteDropTable(ref SimpleParser p, DateTime QueryStart, TimeSpan ParseTime)
        {
            EsEmResult Res = new EsEmResult();
            Res._ColumnNames.Add("Result");
            Res._ColumnTypes.Add(ColumnType.TEXT);
            Collection<object> Results = new Collection<object>();
            bool HasError = false;
            string ErrorMessage = "";

            Res._ParseTime = ParseTime;
            DateTime BuildStartTime = DateTime.Now;

            ParseToken DropTableTok = p.NextToken();
            ParseToken TableNameTok = p.NextToken();

            string TableName = TableNameTok.TokenValue;

            bool TableFound = false;

            for (int i = 0; i < _Database._Tables.Count; i++)
                if (_Database._Tables[i].Name == TableName)
                    TableFound = true;

            if (TableFound)
            {
                _Database.DropTable(TableName);
                Res._BuildTime = DateTime.Now - BuildStartTime;
                Res._TotalQueryTime = DateTime.Now - QueryStart;
            }
            else
            {
                HasError = true;
                ErrorMessage = "Table '" + TableName + "' Doesn't Exists";
            }

            if (HasError)
            {
                Res._ColumnNames.Add((string)"Error");
                Res._ColumnTypes.Add(ColumnType.TEXT);
                Results.Add((string)"Error");
                Results.Add(ErrorMessage);
            }
            else
                Results.Add((string)"OK");
            Res._ColumnValues.Add(Results);
            return Res;
        }

        internal EsEmResult _ExecuteInsert(ref SimpleParser p, DateTime QueryStart, TimeSpan ParseTime)
        {
            int InsertedRowCount = 0;
            EsEmResult Res = new EsEmResult();
            Res._ColumnNames.Add("Result");
            Res._ColumnTypes.Add(ColumnType.TEXT);
            Collection<object> Results = new Collection<object>();
            bool HasError = false;
            string ErrorMessage = "";

            Res._ParseTime = ParseTime;
            DateTime BuildStartTime = DateTime.Now;

            ParseToken InsertTok = p.NextToken();
            ParseToken TableNameTok = p.NextToken();
            string TableName = TableNameTok.TokenValue;

            bool TableFound = false;
            for (int i = 0; i < _Database._Tables.Count; i++)
            {
                if (_Database._Tables[i].Name == TableName)
                {
                    TableFound = true;
                    break;
                }
            }

            if (TableFound)
            {
                ParseToken OpenBrace = p.NextToken();
                ParseToken Tok = p.NextToken();
                Collection<string> ColumnNames = new Collection<string>();
                while (Tok.TokenType != ParseTokenType.CloseBrace)
                {
                    ColumnNames.Add(Tok.TokenValue);
                    Tok = p.NextToken();
                }
                string InvalidColumnName = "";
                EsEmTable tbl = _Database[TableName];
                for (int e = 0; e < ColumnNames.Count && InvalidColumnName == ""; e++)
                {
                    bool cFound = false;
                    for (int i = 0; i < tbl._TableColumns.Count; i++)
                    {
                        if (tbl._TableColumns[i].Name == ColumnNames[e])
                        {
                            cFound = true;
                            break;
                        }
                    }
                    if (!cFound)
                        InvalidColumnName = ColumnNames[e];
                }
                if (InvalidColumnName == "")
                {
                    Tok = p.NextToken();
                    if (Tok.TokenType == ParseTokenType.Values)
                    {
                        Tok = p.NextToken();
                        while (Tok != null)
                        {
                            Collection<string> ColumnValues = new Collection<string>();
                            if (Tok.TokenType == ParseTokenType.OpenBrace)
                            {
                                Tok = p.NextToken();
                                while (Tok.TokenType != ParseTokenType.CloseBrace)
                                {
                                    if (Tok.TokenType == ParseTokenType.StringLiteral)
                                        ColumnValues.Add(Tok.TokenValue);
                                    Tok = p.NextToken();
                                }
                                Tok = p.NextToken();
                            }
                            if (ColumnValues.Count == ColumnNames.Count)
                            {
                                DbTableIter itr = new DbTableIter(ref tbl);
                                itr.Read();
                                if (itr.StartNew())
                                {
                                    for(int i = 0; i < ColumnNames.Count; i++)
                                    {
                                        switch (tbl[ColumnNames[i]].GetColumnType)
                                        {
                                            case ColumnType.BOOL:
                                                {
                                                    itr[ColumnNames[i]] = Convert.ToBoolean(ColumnValues[i]);
                                                }break;
                                            case ColumnType.DATETIME:
                                                {
                                                    itr[ColumnNames[i]] = Convert.ToDateTime(ColumnValues[i]);
                                                }break;
                                            case ColumnType.FLOAT:
                                                {
                                                    itr[ColumnNames[i]] = (float)Convert.ToDouble(ColumnValues[i]);
                                                }break;
                                            case ColumnType.INTEGER:
                                                {
                                                    itr[ColumnNames[i]] = Convert.ToInt64(ColumnValues[i]);
                                                }break;
                                            case ColumnType.TEXT:
                                                {
                                                    itr[ColumnNames[i]] = ColumnValues[i];
                                                }break;
                                        }
                                    }
                                    if (itr.CommitNew())
                                    {
                                        InsertedRowCount++;
                                    }
                                    else
                                    {
                                        HasError = true;
                                        ErrorMessage = "Failed To Commit Row";
                                    }
                                    itr.Close();
                                    Res._BuildTime = DateTime.Now - BuildStartTime;
                                    Res._TotalQueryTime = DateTime.Now - QueryStart;
                                }
                                else
                                {
                                    HasError = true;
                                    ErrorMessage = "Failed To Insert Row";
                                }
                            }
                        }
                    }
                    else
                    {
                        HasError = true;
                        ErrorMessage = "Expecting 'Values' in Insert Statement";
                    }
                }
                else
                {
                    HasError = true;
                    ErrorMessage = "Column '" + InvalidColumnName + "' Doesn't Exists In Table '" + TableName + "'";
                }

            }
            else
            {
                HasError = true;
                ErrorMessage = "Table '" + TableName + "' Doesn't Exists";
            }

            if (HasError)
            {
                Res._ColumnNames.Add((string)"Error");
                Res._ColumnTypes.Add(ColumnType.TEXT);
                Results.Add((string)"Error");
                Results.Add(ErrorMessage);
            }
            else
                Results.Add((string)"Inserted " + InsertedRowCount.ToString() + " Row(s)");
            Res._ColumnValues.Add(Results);
            return Res;
        }

        internal EsEmResult _ExecuteUpdate(ref SimpleParser p, DateTime QueryStart, TimeSpan ParseTime)
        {
            int UpdatedRows = 0;
            EsEmResult Res = new EsEmResult();
            Res._ColumnNames.Add("Result");
            Res._ColumnTypes.Add(ColumnType.TEXT);
            Collection<object> Results = new Collection<object>();
            bool HasError = false;
            string ErrorMessage = "";

            Res._ParseTime = ParseTime;
            DateTime BuildStartTime = DateTime.Now;

            ParseToken Tok = p.NextToken();
            Tok = p.NextToken();
            string TableName = Tok.TokenValue;

            bool TableFound = false;
            for (int i = 0; i < _Database._Tables.Count; i++)
            {
                if (_Database._Tables[i].Name == TableName)
                {
                    TableFound = true;
                    break;
                }
            }

            if (TableFound)
            {
                EsEmTable tbl = _Database[TableName];
                Tok = p.NextToken();
                if (Tok.TokenType == ParseTokenType.Set)
                {
                    Collection<string> Columns = new Collection<string>();
                    Collection<string> Values = new Collection<string>();
                    Tok = p.NextToken();
                    while (Tok != null && Tok.TokenType != ParseTokenType.Where)
                    {
                        Columns.Add(Tok.TokenValue);
                        Tok = p.NextToken();
                        Tok = p.NextToken();
                        Values.Add(Tok.TokenValue);
                        Tok = p.NextToken();
                    }
                    
                    string ErrorColumn = "";
                    for (int i = 0; i < Columns.Count; i++)
                    {
                        bool ColumnFound = false;
                        for (int e = 0; e < tbl._TableColumns.Count; e++)
                        {
                            if (tbl._TableColumns[e].Name == Columns[i])
                            {
                                ColumnFound = true;
                                break;
                            }
                        }
                        if (!ColumnFound)
                            ErrorColumn = Columns[i];
                    }

                    if (ErrorColumn == "")
                    {
                        if (Tok != null && Tok.TokenType == ParseTokenType.Where)
                        {
                            p.Back();
                            DateTime FilterStartTime = DateTime.Now;
                            int[] eRows = _RecurseParseWhereClause(ref tbl, ref p);
                            Res._FilterTime = DateTime.Now - FilterStartTime;
                            UpdatedRows = eRows.Length;
                            DbTableIter itr = new DbTableIter(ref tbl);
                            itr.Read();
                            for (int i = 0; i < eRows.Length; i++)
                            {
                                itr.SeekRowIndex(eRows[i]);
                                for (int e = 0; e < Columns.Count; e++)
                                {
                                    switch (tbl[Columns[e]].GetColumnType)
                                    {
                                        case ColumnType.BOOL:
                                            {
                                                itr[Columns[e]] = Convert.ToBoolean(Values[e]);
                                            } break;
                                        case ColumnType.DATETIME:
                                            {
                                                itr[Columns[e]] = Convert.ToDateTime(Values[e]);
                                            } break;
                                        case ColumnType.FLOAT:
                                            {
                                                itr[Columns[e]] = (float)Convert.ToDouble(Values[e]);
                                            } break;
                                        case ColumnType.INTEGER:
                                            {
                                                itr[Columns[e]] = Convert.ToInt64(Values[e]);
                                            } break;
                                        case ColumnType.TEXT:
                                            {
                                                itr[Columns[e]] = Values[e];
                                            } break;
                                    }
                                }
                                itr.Update();
                            }
                            itr.Close();
                            Res._BuildTime = DateTime.Now - BuildStartTime;
                            Res._TotalQueryTime = DateTime.Now - QueryStart;
                        }
                        else
                        {
                            DbTableIter itr = new DbTableIter(ref tbl);
                            while (itr.Read())
                            {
                                for (int e = 0; e < Columns.Count; e++)
                                {
                                    switch (tbl[Columns[e]].GetColumnType)
                                    {
                                        case ColumnType.BOOL:
                                            {
                                                itr[Columns[e]] = Convert.ToBoolean(Values[e]);
                                            } break;
                                        case ColumnType.DATETIME:
                                            {
                                                itr[Columns[e]] = Convert.ToDateTime(Values[e]);
                                            } break;
                                        case ColumnType.FLOAT:
                                            {
                                                itr[Columns[e]] = (float)Convert.ToDouble(Values[e]);
                                            } break;
                                        case ColumnType.INTEGER:
                                            {
                                                itr[Columns[e]] = Convert.ToInt64(Values[e]);
                                            } break;
                                        case ColumnType.TEXT:
                                            {
                                                itr[Columns[e]] = Values[e];
                                            } break;
                                    }
                                }
                                itr.Update();
                                UpdatedRows++;
                            }
                            itr.Close();
                            Res._BuildTime = DateTime.Now - BuildStartTime;
                            Res._TotalQueryTime = DateTime.Now - QueryStart;
                        }
                    }
                    else
                    {
                        HasError = true;
                        ErrorMessage = "Column '" + ErrorColumn + "' Not Found In Table '" + TableName + "'";
                    }
                }
                else
                {
                    HasError = true;
                    ErrorMessage = "Expecting Set In Update Query";
                }
            }
            else
            {
                HasError = true;
                ErrorMessage = "Table '" + TableName + "' Doesn't Exists";
            }

            if (HasError)
            {
                Res._ColumnNames.Add((string)"Error");
                Res._ColumnTypes.Add(ColumnType.TEXT);
                Results.Add((string)"Error");
                Results.Add(ErrorMessage);
            }
            else
                Results.Add((string)"Updated " + UpdatedRows.ToString() + " Row(s)");
            Res._ColumnValues.Add(Results);
            return Res;
        }

        internal EsEmResult _ExecuteDelete(ref SimpleParser p, DateTime QueryStart, TimeSpan ParseTime)
        {
            int DeletedRows = 0;
            EsEmResult Res = new EsEmResult();
            Res._ColumnNames.Add("Result");
            Res._ColumnTypes.Add(ColumnType.TEXT);
            Collection<object> Results = new Collection<object>();
            bool HasError = false;
            string ErrorMessage = "";

            Res._ParseTime = ParseTime;
            DateTime BuildStartTime = DateTime.Now;

            ParseToken Tok = p.NextToken();
            Tok = p.NextToken();
            string TableName = Tok.TokenValue;

            bool TableFound = false;
            for (int i = 0; i < _Database._Tables.Count; i++)
            {
                if (_Database._Tables[i].Name == TableName)
                {
                    TableFound = true;
                    break;
                }
            }
            if (TableFound)
            {
                EsEmTable tbl = _Database[TableName];
                Tok = p.NextToken();
                if (Tok != null && Tok.TokenType == ParseTokenType.Where)
                {
                    p.Back();
                    DateTime FilterStartTime = DateTime.Now;
                    int[] dRows = _RecurseParseWhereClause(ref tbl, ref p);
                    Res._FilterTime = DateTime.Now - FilterStartTime;
                    DeletedRows = dRows.Length;
                    DbTableIter itr = new DbTableIter(ref tbl);
                    itr.Read();
                    itr.DeleteSet(dRows);
                    itr.Close();
                    Res._BuildTime = DateTime.Now - BuildStartTime;
                    Res._TotalQueryTime = DateTime.Now - QueryStart;
                }
                else
                {
                    DbTableIter itr = new DbTableIter(ref tbl);
                    while (itr.Read())
                    {
                        itr.Delete();
                        DeletedRows++;
                    }
                    itr.Close();
                    Res._BuildTime = DateTime.Now - BuildStartTime;
                    Res._TotalQueryTime = DateTime.Now - QueryStart;
                }
            }
            else
            {
                HasError = true;
                ErrorMessage = "Table '" + TableName + " Doesn't Exists";
            }

            if (HasError)
            {
                Res._ColumnNames.Add((string)"Error");
                Res._ColumnTypes.Add(ColumnType.TEXT);
                Results.Add((string)"Error");
                Results.Add(ErrorMessage);
            }
            else
                Results.Add((string)"Deleted " + DeletedRows.ToString() + " Row(s)");
            Res._ColumnValues.Add(Results);
            return Res;
        }

        internal EsEmResult _ExecuteSelect(ref SimpleParser p, DateTime QueryStart, TimeSpan ParseTime)
        {
            EsEmResult Res = new EsEmResult();
            Res._ColumnNames.Add("Result");
            Res._ColumnTypes.Add(ColumnType.TEXT);
            Collection<object> Results = new Collection<object>();
            bool HasError = false;
            string ErrorMessage = "";

            Res._ParseTime = ParseTime;
            DateTime BuildStartTime = DateTime.Now;
            

            ParseToken Tok = p.NextToken();
            Collection<string> Columns = new Collection<string>();
            Tok = p.NextToken();
            while (Tok.TokenType != ParseTokenType.From)
            {
                Columns.Add(Tok.TokenValue);
                Tok = p.NextToken();
            }
            Tok = p.NextToken();
            string TableName = Tok.TokenValue;
            Tok = p.NextToken();

            if (Columns.Count > 0)
            {

                bool TableFound = false;
                for (int i = 0; i < _Database._Tables.Count; i++)
                {
                    if (_Database._Tables[i].Name == TableName)
                    {
                        TableFound = true;
                        break;
                    }
                }
                if (TableFound)
                {
                    EsEmTable tbl = _Database[TableName];
                    if (Columns[0] == "*")
                    {
                        Columns.Clear();
                        for (int i = 0; i < tbl._TableColumns.Count; i++)
                            Columns.Add(tbl._TableColumns[i].Name);
                    }

                    string ErrorColumn = "";
                    for (int i = 0; i < Columns.Count; i++)
                    {
                        bool ColumnFound = false;
                        for (int e = 0; e < tbl._TableColumns.Count; e++)
                        {
                            if (Columns[i] == tbl._TableColumns[e].Name)
                            {
                                ColumnFound = true;
                                break;
                            }
                        }
                        if (!ColumnFound)
                            ErrorColumn = Columns[i];
                    }

                    if (ErrorColumn == "")
                    {
                        EsEmResult qResult = new EsEmResult();
                        qResult._ParseTime = Res._ParseTime;
                        for (int i = 0; i < Columns.Count; i++)
                        {
                            qResult._ColumnNames.Add(Columns[i]);
                            qResult._ColumnTypes.Add(tbl[Columns[i]].GetColumnType);
                        }

                        if (Tok != null && Tok.TokenType == ParseTokenType.Where)
                        {
                            p.Back();
                            DateTime FilterTimeStart = DateTime.Now;
                            int[] SelRows = _RecurseParseWhereClause(ref tbl, ref p);
                            qResult._FilterTime = DateTime.Now - FilterTimeStart;
                            DbTableIter itr = new DbTableIter(ref tbl);
                            itr.Read();
                            for (int i = 0; i < SelRows.Length; i++)
                            {
                                itr.SeekRowIndex(SelRows[i]);
                                Collection<object> tRow = new Collection<object>();
                                for (int e = 0; e < Columns.Count; e++)
                                    tRow.Add(itr[Columns[e]]);
                                qResult._ColumnValues.Add(tRow);
                            }
                            itr.Close();
                            qResult._BuildTime = DateTime.Now - BuildStartTime;
                            qResult._TotalQueryTime = DateTime.Now - QueryStart;
                        }
                        else
                        {
                            DbTableIter itr = new DbTableIter(ref tbl);
                            while (itr.Read())
                            {
                                Collection<object> tRow = new Collection<object>();
                                for (int i = 0; i < Columns.Count; i++)
                                    tRow.Add(itr[Columns[i]]);
                                qResult._ColumnValues.Add(tRow);
                            }
                            itr.Close();
                            qResult._BuildTime = DateTime.Now - BuildStartTime;
                            qResult._TotalQueryTime = DateTime.Now - QueryStart;
                        }
                        return qResult;
                    }
                    else
                    {
                        HasError = true;
                        ErrorMessage = "Column '" + ErrorColumn + "' Doesn't Exists In Table '" + TableName + "'";
                    }

                }
                else
                {
                    HasError = true;
                    ErrorMessage = "Table '" + TableName + "' Not Found";
                }
            }
            else
            {
                HasError = true;
                ErrorMessage = "No Columns Selected";
            }

            if (HasError)
            {
                Res._ColumnNames.Add((string)"Error");
                Res._ColumnTypes.Add(ColumnType.TEXT);
                Results.Add((string)"Error");
                Results.Add(ErrorMessage);
            }
            else
                Results.Add((string)"OK");
            Res._ColumnValues.Add(Results);
            return Res;
        }

        public EsEmResult Execute()
        {
            DateTime QueryStart = DateTime.Now;
            DateTime ParseStart = QueryStart;
            SimpleParser p = new SimpleParser(_QueryText);
            TimeSpan ParseTime = DateTime.Now - ParseStart;
            ParseToken tok = p.PeekToken();
            switch (tok.TokenType)
            {
                case ParseTokenType.CreateTable:
                    {
                        return _ExecuteCreateTable(ref p, QueryStart, ParseTime);
                    }
                case ParseTokenType.Delete:
                    {
                        return _ExecuteDelete(ref p, QueryStart, ParseTime);
                    }
                case ParseTokenType.DropTable:
                    {
                        return _ExecuteDropTable(ref p, QueryStart, ParseTime);
                    }
                case ParseTokenType.Insert:
                    {
                        return _ExecuteInsert(ref p, QueryStart, ParseTime);
                    }
                case ParseTokenType.Select:
                    {
                        return _ExecuteSelect(ref p, QueryStart, ParseTime);
                    }
                case ParseTokenType.Update:
                    {
                        return _ExecuteUpdate(ref p, QueryStart, ParseTime);
                    }
            }

            EsEmResult Res = new EsEmResult();
            Res._ColumnNames.Add("Result");
            Res._ColumnTypes.Add(ColumnType.TEXT);
            Collection<object> Results = new Collection<object>();
            Res._ColumnNames.Add((string)"Error");
            Res._ColumnTypes.Add(ColumnType.TEXT);
            Results.Add((string)"Error");
            Results.Add((string)"Invalid Query");
            return Res;
        }
		
	}
}
