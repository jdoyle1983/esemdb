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
using System.Text;
using System.Collections.ObjectModel;

namespace EsEmDb
{
    public enum ParseTokenType
    {
        UnKnown = 0,
        Select,
        Insert,
        Update,
        Delete,
        Where,
        From,
        OrderBy,
        Ascending,
        Descending,
        InnerJoin,
        On,
        Or,
        And,
        IsNotNull,
        IsNull,
        NotEqual,
        Equal,
        GreaterThan,
        LessThan,
        GreaterThanOrEqualTo,
        LessThanOrEqualTo,
        DropTable,
        CreateTable,
        StringLiteral,
        OpenBrace,
        CloseBrace,
        EndOfLine,
        Values,
        Integer,
        Text,
        Boolean,
        Float,
        DateTime,
        Raw,
        PrimaryKey,
        AutoIncrement,
        Set
    }

    public class ParseToken
    {
        public ParseTokenType TokenType;
        public string TokenValue;

        public ParseToken( ParseTokenType tType)
        {
            TokenType = tType;
        }

        public ParseToken( ParseTokenType tType, string tValue)
        {
            TokenType = tType;
            TokenValue = tValue;
        }
    }

	public class SimpleParser
	{
        public Collection<ParseToken> ParsedTokens = new Collection<ParseToken>();
        private int TokenIndex = -1;

        

        private string FromToken(ParseTokenType t)
        {
            switch(t)
            {
                case ParseTokenType.And:
                    {
                        return "and";
                    }
                case ParseTokenType.Ascending:
                    {
                        return "asc";
                    }
                case ParseTokenType.CloseBrace:
                    {
                        return ")";
                    }
                case ParseTokenType.CreateTable:
                    {
                        return "create table";
                    }
                case ParseTokenType.Delete:
                    {
                        return "delete from";
                    }
                case ParseTokenType.Descending:
                    {
                        return "desc";
                    }
                case ParseTokenType.DropTable:
                    {
                        return "drop table";
                    } 
                case ParseTokenType.Equal:
                    {
                        return "=";
                    }
                case ParseTokenType.From:
                    {
                        return "from";
                    } 
                case ParseTokenType.GreaterThan:
                    {
                        return ">";
                    }
                case ParseTokenType.GreaterThanOrEqualTo:
                    {
                        return ">=";
                    }
                case ParseTokenType.InnerJoin:
                    {
                        return "inner join";
                    }
                case ParseTokenType.Insert:
                    {
                        return "insert into";
                    } 
                case ParseTokenType.IsNotNull:
                    {
                        return "is not null";
                    }
                case ParseTokenType.IsNull:
                    {
                        return "is null";
                    }
                case ParseTokenType.LessThan:
                    {
                        return "<";
                    }
                case ParseTokenType.LessThanOrEqualTo:
                    {
                        return "<=";
                    }
                case ParseTokenType.NotEqual:
                    {
                        return "<>";
                    }
                case ParseTokenType.On:
                    {
                        return "on";
                    }
                case ParseTokenType.OpenBrace:
                    {
                        return "(";
                    }
                case ParseTokenType.Or:
                    {
                        return "or";
                    }
                case ParseTokenType.OrderBy:
                    {
                        return "order by";
                    }
                case ParseTokenType.Select:
                    {
                        return "select";
                    }
                case ParseTokenType.Update:
                    {
                        return "update";
                    }
                case ParseTokenType.Where:
                    {
                        return "where";
                    }
                case ParseTokenType.EndOfLine:
                    {
                        return ";";
                    }
                case ParseTokenType.Values:
                    {
                        return "values";
                    }
                case ParseTokenType.Integer:
                    {
                        return "integer";
                    }
                case ParseTokenType.Text:
                    {
                        return "text";
                    }
                case ParseTokenType.Boolean:
                    {
                        return "boolean";
                    }
                case ParseTokenType.Float:
                    {
                        return "float";
                    }
                case ParseTokenType.DateTime:
                    {
                        return "datetime";
                    }
                case ParseTokenType.Raw:
                    {
                        return "raw";
                    }
                case ParseTokenType.PrimaryKey:
                    {
                        return "primary key";
                    }
                case ParseTokenType.AutoIncrement:
                    {
                        return "auto increment";
                    }
                case ParseTokenType.Set:
                    {
                        return "set";
                    }
            }
            return "";
        }

        private ParseToken EvaluateTokenType(string Input)
        {
            Input = Input.Trim();
            if (Input.ToLower() == FromToken(ParseTokenType.Select))
                return new ParseToken(ParseTokenType.Select);
            else if (Input.ToLower() == FromToken(ParseTokenType.Update))
                return new ParseToken(ParseTokenType.Update);
            else if (Input.ToLower() == FromToken(ParseTokenType.Insert))
                return new ParseToken(ParseTokenType.Insert);
            else if (Input.ToLower() == FromToken(ParseTokenType.Delete))
                return new ParseToken(ParseTokenType.Delete);
            else if (Input.ToLower() == FromToken(ParseTokenType.DropTable))
                return new ParseToken(ParseTokenType.DropTable);
            else if (Input.ToLower() == FromToken(ParseTokenType.CreateTable))
                return new ParseToken(ParseTokenType.CreateTable);
            else if (Input.ToLower() == FromToken(ParseTokenType.OpenBrace))
                return new ParseToken(ParseTokenType.OpenBrace);
            else if (Input.ToLower() == FromToken(ParseTokenType.CloseBrace))
                return new ParseToken(ParseTokenType.CloseBrace);
            else if (Input.ToLower() == FromToken(ParseTokenType.Where))
                return new ParseToken(ParseTokenType.Where);
            else if (Input.ToLower() == FromToken(ParseTokenType.From))
                return new ParseToken(ParseTokenType.From);
            else if (Input.ToLower() == FromToken(ParseTokenType.OrderBy))
                return new ParseToken(ParseTokenType.OrderBy);
            else if (Input.ToLower() == FromToken(ParseTokenType.Ascending))
                return new ParseToken(ParseTokenType.Ascending);
            else if (Input.ToLower() == FromToken(ParseTokenType.Descending))
                return new ParseToken(ParseTokenType.Descending);
            else if (Input.ToLower() == FromToken(ParseTokenType.InnerJoin))
                return new ParseToken(ParseTokenType.InnerJoin);
            else if (Input.ToLower() == FromToken(ParseTokenType.On))
                return new ParseToken(ParseTokenType.On);
            else if (Input.ToLower() == FromToken(ParseTokenType.Or))
                return new ParseToken(ParseTokenType.Or);
            else if (Input.ToLower() == FromToken(ParseTokenType.And))
                return new ParseToken(ParseTokenType.And);
            else if (Input.ToLower() == FromToken(ParseTokenType.IsNotNull))
                return new ParseToken(ParseTokenType.IsNotNull);
            else if (Input.ToLower() == FromToken(ParseTokenType.IsNull))
                return new ParseToken(ParseTokenType.IsNull);
            else if (Input.ToLower() == FromToken(ParseTokenType.Equal))
                return new ParseToken(ParseTokenType.Equal);
            else if (Input.ToLower() == FromToken(ParseTokenType.GreaterThanOrEqualTo))
                return new ParseToken(ParseTokenType.GreaterThanOrEqualTo);
            else if (Input.ToLower() == FromToken(ParseTokenType.GreaterThan))
                return new ParseToken(ParseTokenType.GreaterThan);
            else if (Input.ToLower() == FromToken(ParseTokenType.NotEqual))
                return new ParseToken(ParseTokenType.NotEqual);
            else if (Input.ToLower() == FromToken(ParseTokenType.LessThanOrEqualTo))
                return new ParseToken(ParseTokenType.LessThanOrEqualTo);
            else if (Input.ToLower() == FromToken(ParseTokenType.LessThan))
                return new ParseToken(ParseTokenType.LessThan);
            else if (Input.ToLower() == FromToken(ParseTokenType.EndOfLine))
                return new ParseToken(ParseTokenType.EndOfLine);
            else if (Input.ToLower() == FromToken(ParseTokenType.Values))
                return new ParseToken(ParseTokenType.Values);
            else if (Input.ToLower() == FromToken(ParseTokenType.Integer))
                return new ParseToken(ParseTokenType.Integer);
            else if (Input.ToLower() == FromToken(ParseTokenType.Text))
                return new ParseToken(ParseTokenType.Text);
            else if (Input.ToLower() == FromToken(ParseTokenType.Boolean))
                return new ParseToken(ParseTokenType.Boolean);
            else if (Input.ToLower() == FromToken(ParseTokenType.Float))
                return new ParseToken(ParseTokenType.Float);
            else if (Input.ToLower() == FromToken(ParseTokenType.DateTime))
                return new ParseToken(ParseTokenType.DateTime);
            else if (Input.ToLower() == FromToken(ParseTokenType.Raw))
                return new ParseToken(ParseTokenType.Raw);
            else if (Input.ToLower() == FromToken(ParseTokenType.PrimaryKey))
                return new ParseToken(ParseTokenType.PrimaryKey);
            else if (Input.ToLower() == FromToken(ParseTokenType.AutoIncrement))
                return new ParseToken(ParseTokenType.AutoIncrement);
            else if (Input.ToLower() == FromToken(ParseTokenType.Set))
                return new ParseToken(ParseTokenType.Set);
            else
                return new ParseToken(ParseTokenType.StringLiteral, Input);
        }

        private void AddStringLiteral(ParseToken t)
        {
            bool CanAdd = true;
            if (ParsedTokens.Count > 1)
            {
                ParseToken Sec1Tok = ParsedTokens[ParsedTokens.Count - 2];
                ParseToken Sec2Tok = ParsedTokens[ParsedTokens.Count - 1];
                if (Sec1Tok.TokenType == ParseTokenType.StringLiteral && Sec2Tok.TokenType == ParseTokenType.StringLiteral)
                {
                    string tmp = Sec1Tok.TokenValue + " " + Sec2Tok.TokenValue + " " + t.TokenValue;
                    tmp = tmp.ToLower();
                    ParseToken tmpt = EvaluateTokenType(tmp);
                    if (tmpt.TokenType != ParseTokenType.StringLiteral)
                    {
                        ParsedTokens.RemoveAt(ParsedTokens.Count - 1);
                        ParsedTokens.RemoveAt(ParsedTokens.Count - 1);
                        ParsedTokens.Add(tmpt);
                        CanAdd = false;
                    }
                    else
                    {
                        tmp = Sec2Tok.TokenValue + " " + t.TokenValue;
                        tmp = tmp.ToLower();
                        tmpt = EvaluateTokenType(tmp);
                        if (tmpt.TokenType != ParseTokenType.StringLiteral)
                        {
                            ParsedTokens.RemoveAt(ParsedTokens.Count - 1);
                            ParsedTokens.Add(tmpt);
                            CanAdd = false;
                        }
                    }
                }
                else
                {
                    string tmp = Sec2Tok.TokenValue + " " + t.TokenValue;
                    tmp = tmp.ToLower();
                    ParseToken tmpt = EvaluateTokenType(tmp);
                    if (tmpt.TokenType != ParseTokenType.StringLiteral)
                    {
                        ParsedTokens.RemoveAt(ParsedTokens.Count - 1);
                        ParsedTokens.Add(tmpt);
                        CanAdd = false;
                    }
                }
            }
            else if (ParsedTokens.Count > 0)
            {
                ParseToken prev = ParsedTokens[ParsedTokens.Count - 1];
                if (prev.TokenType == ParseTokenType.StringLiteral)
                {
                    string tmp = prev.TokenValue + " " + t.TokenValue;
                    tmp = tmp.ToLower();
                    ParseToken tmpt = EvaluateTokenType(tmp);
                    if (tmpt.TokenType != ParseTokenType.StringLiteral)
                    {
                        ParsedTokens.RemoveAt(ParsedTokens.Count - 1);
                        ParsedTokens.Add(tmpt);
                        CanAdd = false;
                    }
                }
            }

            if (CanAdd)
                ParsedTokens.Add(t);
        }

        public SimpleParser(string InputString)
        {
            string BuildingToken = "";

            bool WaitingForCloseLiteral = false;
            char LiteralToken = ' ';

            for (int i = 0; i < InputString.Length; i++)
            {
                if (WaitingForCloseLiteral)
                {
                    if (InputString[i] == LiteralToken)
                    {
                        ParsedTokens.Add(new ParseToken(ParseTokenType.StringLiteral, BuildingToken));
                        BuildingToken = "";
                        WaitingForCloseLiteral = false;
                    }
                    else
                        BuildingToken += InputString[i];
                }
                else
                {
                    ParseToken t = EvaluateTokenType(InputString[i].ToString());
                    if (t.TokenType != ParseTokenType.StringLiteral)
                    {
                        ParseToken tb = EvaluateTokenType(BuildingToken.ToLower());
                        if (tb.TokenType != ParseTokenType.StringLiteral)
                            ParsedTokens.Add(tb);
                        else
                            if (BuildingToken.Trim() != "")
                                AddStringLiteral(new ParseToken(ParseTokenType.StringLiteral, BuildingToken.Trim()));
                        if (t.TokenType == ParseTokenType.Equal)
                        {
                            if (ParsedTokens[ParsedTokens.Count - 1].TokenType == ParseTokenType.LessThan)
                            {
                                ParsedTokens.RemoveAt(ParsedTokens.Count - 1);
                                ParsedTokens.Add(new ParseToken(ParseTokenType.LessThanOrEqualTo));
                            }
                            else if (ParsedTokens[ParsedTokens.Count - 1].TokenType == ParseTokenType.GreaterThan)
                            {
                                ParsedTokens.RemoveAt(ParsedTokens.Count - 1);
                                ParsedTokens.Add(new ParseToken(ParseTokenType.GreaterThanOrEqualTo));
                            }
                            else
                                ParsedTokens.Add(t);

                        }
                        else if(t.TokenType == ParseTokenType.GreaterThan)
                        {
                            if (ParsedTokens[ParsedTokens.Count - 1].TokenType == ParseTokenType.LessThan)
                            {
                                ParsedTokens.RemoveAt(ParsedTokens.Count - 1);
                                ParsedTokens.Add(new ParseToken(ParseTokenType.NotEqual));
                            }
                            else
                                ParsedTokens.Add(t);
                        }
                        else
                            ParsedTokens.Add(t);
                        BuildingToken = "";
                    }
                    else if (InputString[i] == ' ' || InputString[i] == ',')
                    {
                        if (BuildingToken.Trim() != "")
                        {
                            ParseToken tb = EvaluateTokenType(BuildingToken.ToLower());
                            if (tb.TokenType != ParseTokenType.StringLiteral)
                            {
                                if (ParsedTokens.Count > 0 && ParsedTokens[ParsedTokens.Count - 1].TokenType == ParseTokenType.StringLiteral && ParsedTokens[ParsedTokens.Count - 1].TokenValue.ToLower() == "delete")
                                {
                                    ParsedTokens.RemoveAt(ParsedTokens.Count - 1);
                                    ParsedTokens.Add(new ParseToken(ParseTokenType.Delete));
                                }
                                else
                                    ParsedTokens.Add(tb);
                            }
                            else
                                AddStringLiteral(new ParseToken(ParseTokenType.StringLiteral, BuildingToken.Trim()));
                        }
                        BuildingToken = "";
                    }
                    else if (InputString[i] == '\"' || InputString[i] == '\'')
                    {
                        LiteralToken = InputString[i];
                        WaitingForCloseLiteral = true;
                        if (BuildingToken.Trim() != "")
                        {
                            ParseToken tb = EvaluateTokenType(BuildingToken.ToLower());
                            if(tb.TokenType != ParseTokenType.StringLiteral)
                                ParsedTokens.Add(tb);
                            else
                                AddStringLiteral(new ParseToken(ParseTokenType.StringLiteral, BuildingToken.Trim()));
                            BuildingToken = "";
                        }
                    }
                    else
                        BuildingToken += InputString[i];
                    /*
                    ParseTokenType t = GetStartsWith(InputString.Substring(i));
                    if (t != ParseTokenType.UnKnown)
                    {
                        if (BuildingToken.Trim() != "")
                        {
                            ParsedTokens.Add(new ParseToken(ParseTokenType.StringLiteral, BuildingToken.Trim()));
                            BuildingToken = "";
                        }
                        ParsedTokens.Add(new ParseToken(t));
                        string ToRemove = FromToken(t);
                        i = i + (ToRemove.Length - 1);
                    }
                    else if (InputString[i] == ' ' || InputString[i] == ',')
                    {
                        if(BuildingToken.Trim() != "")
                            ParsedTokens.Add(new ParseToken(ParseTokenType.StringLiteral, BuildingToken.Trim()));
                        BuildingToken = "";
                    }
                    else if (InputString[i] == '\"' || InputString[i] == '\'')
                    {
                        LiteralToken = InputString[i];
                        WaitingForCloseLiteral = true;
                        if (BuildingToken.Trim() != "")
                        {
                            ParsedTokens.Add(new ParseToken(ParseTokenType.StringLiteral, BuildingToken.Trim()));
                            BuildingToken = "";
                        }
                    }
                    else
                        BuildingToken += InputString[i];
                  */
                }
            }
            if (BuildingToken.Trim() != "")
            {
                AddStringLiteral(new ParseToken(ParseTokenType.StringLiteral, BuildingToken.Trim()));
                BuildingToken = "";
            }
            /*ParseTokenType lt = GetStartsWith(InputString.Substring(InputString.Length - 1));
            if (lt != ParseTokenType.UnKnown)
                ParsedTokens.Add(new ParseToken(lt));*/
        }

        public void Back()
        {
            if (TokenIndex >= 0)
                TokenIndex--;
        }

        public void Reset()
        {
            TokenIndex = -1;
        }

        public ParseToken NextToken()
        {
            TokenIndex++;
            if (TokenIndex >= ParsedTokens.Count)
                return null;
            return ParsedTokens[TokenIndex];
        }

        public ParseToken PeekToken()
        {
            if (TokenIndex + 1 >= ParsedTokens.Count)
                return null;
            return ParsedTokens[TokenIndex + 1];
        }
	}
}
