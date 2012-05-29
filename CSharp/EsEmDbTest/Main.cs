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
using EsEmDb;
using System.IO;

namespace EsEmDbTest
{
	class MainClass
	{
		public static void Main(string[] args)
		{
            bool ExitFlag = false;
            if (args.Length > 0)
            {
                EsEmDatabase db = new EsEmDatabase();
                db.OpenDatabase(args[0]);
                string PrevLine = "";
                while (!ExitFlag)
                {
                    Console.Write("ESEMDB>");
                    string InputLine = Console.ReadLine();
                    if (InputLine.ToLower().StartsWith(".exit") || InputLine.ToLower().StartsWith(".quit"))
                        ExitFlag = true;
                    else if (InputLine.ToLower().StartsWith(".schema"))
                        db.DumpSchema();
                    else if (InputLine.ToLower().StartsWith(".enablecache"))
                    {
                        Console.WriteLine("Enabling Table Caching...");
                        db.EnableCacheTables();
                    }
                    else if (InputLine.ToLower().StartsWith(".disablecache"))
                    {
                        Console.WriteLine("Disabling Table Caching...");
                        db.DisableCacheTables();
                    }
                    else if (InputLine.ToLower().StartsWith(".cachestatus"))
                    {
                        if (db.CacheTables)
                            Console.WriteLine("Table Caching Is ON");
                        else
                            Console.WriteLine("Table Caching Is OFF");
                    }
                    else if (InputLine.ToLower().StartsWith(".speedtest"))
                    {
                        DateTime TotalStartTime = DateTime.Now;
                        Console.WriteLine("Creating Table...");
                        string strQuery = "CREATE TABLE SpeedTestTable (id integer primary key auto increment, TextField text, DateTimeField, datetime )";
                        EsEmQuery CreateQuery = db.CreateQuery(strQuery);
                        EsEmResult CreateResult = CreateQuery.Execute();

                        Console.WriteLine("Inserting Rows...");
                        string qString = "INSERT INTO SpeedTestTable(TextField,DateTimeField) values ('aaa','" + DateTime.Now.ToString() + "')";
                        for (int i = 1; i < 5000; i++)
                            qString += ",('aaaaaa','" + DateTime.Now.ToString() + "')";
                        EsEmQuery InsertQuery = db.CreateQuery(qString);
                        EsEmResult InsertResult = InsertQuery.Execute();

                        Console.WriteLine("Selecting All...");
                        EsEmQuery SelectPlainQuery = db.CreateQuery("SELECT * FROM SpeedTestTable");
                        EsEmResult SelectPlainResult = SelectPlainQuery.Execute();

                        Console.WriteLine("Select With Where...");
                        EsEmQuery SelectWhereQuery = db.CreateQuery("SELECT * FROM SpeedTestTable WHERE id > 2500");
                        EsEmResult SelectWhereResult = SelectWhereQuery.Execute();

                        Console.WriteLine("Updating Rows...");
                        EsEmQuery UpdateQuery = db.CreateQuery("Update SpeedTestTable SET DateTimeField='" + DateTime.Now.ToString() + "' WHERE id > 2500");
                        EsEmResult UpdateResult = UpdateQuery.Execute();

                        Console.WriteLine("Deleting Rows...");
                        EsEmQuery DeleteQuery = db.CreateQuery("Delete from SpeedTestTable WHERE id > 2500");
                        EsEmResult DeleteResult = DeleteQuery.Execute();

                        Console.WriteLine("Dropping Table...");
                        EsEmQuery DropQuery = db.CreateQuery("Drop Table SpeedTestTable");
                        EsEmResult DropResult = DropQuery.Execute();

                        DateTime TotalEndTime = DateTime.Now;

                        TimeSpan TotalTime = TotalEndTime - TotalStartTime;

                        Console.WriteLine("RESULTS");
                        Console.WriteLine("====================");
                        Console.WriteLine("Create Table Time                  : " + CreateResult.TotalQueryTime.TotalMilliseconds.ToString() + "ms");
                        Console.WriteLine("    Parse : " + CreateResult.ParseTime.TotalMilliseconds.ToString() + "ms");
                        Console.WriteLine("    Build : " + CreateResult.BuildTime.TotalMilliseconds.ToString() + "ms");
                        Console.WriteLine("    Filter: " + CreateResult.FilterTime.TotalMilliseconds.ToString() + "ms");

                        Console.WriteLine("Insert Time (5000 Rows)            : " + InsertResult.TotalQueryTime.TotalMilliseconds.ToString() + "ms");
                        Console.WriteLine("    Parse : " + InsertResult.ParseTime.TotalMilliseconds.ToString() + "ms");
                        Console.WriteLine("    Build : " + InsertResult.BuildTime.TotalMilliseconds.ToString() + "ms");
                        Console.WriteLine("    Filter: " + InsertResult.FilterTime.TotalMilliseconds.ToString() + "ms");

                        Console.WriteLine("Select All Time (5000 Rows)        : " + SelectPlainResult.TotalQueryTime.TotalMilliseconds.ToString() + "ms");
                        Console.WriteLine("    Parse : " + SelectPlainResult.ParseTime.TotalMilliseconds.ToString() + "ms");
                        Console.WriteLine("    Build : " + SelectPlainResult.BuildTime.TotalMilliseconds.ToString() + "ms");
                        Console.WriteLine("    Filter: " + SelectPlainResult.FilterTime.TotalMilliseconds.ToString() + "ms");

                        Console.WriteLine("Select With Where Time (2500 Rows) : " + SelectWhereResult.TotalQueryTime.TotalMilliseconds.ToString() + "ms");
                        Console.WriteLine("    Parse : " + SelectWhereResult.ParseTime.TotalMilliseconds.ToString() + "ms");
                        Console.WriteLine("    Build : " + SelectWhereResult.BuildTime.TotalMilliseconds.ToString() + "ms");
                        Console.WriteLine("    Filter: " + SelectWhereResult.FilterTime.TotalMilliseconds.ToString() + "ms");

                        Console.WriteLine("Update Time (2500 Rows)            : " + UpdateResult.TotalQueryTime.TotalMilliseconds.ToString() + "ms");
                        Console.WriteLine("    Parse : " + UpdateResult.ParseTime.TotalMilliseconds.ToString() + "ms");
                        Console.WriteLine("    Build : " + UpdateResult.BuildTime.TotalMilliseconds.ToString() + "ms");
                        Console.WriteLine("    Filter: " + UpdateResult.FilterTime.TotalMilliseconds.ToString() + "ms");

                        Console.WriteLine("Delete Time (2500 Rows)            : " + DeleteResult.TotalQueryTime.TotalMilliseconds.ToString() + "ms");
                        Console.WriteLine("    Parse : " + DeleteResult.ParseTime.TotalMilliseconds.ToString() + "ms");
                        Console.WriteLine("    Build : " + DeleteResult.BuildTime.TotalMilliseconds.ToString() + "ms");
                        Console.WriteLine("    Filter: " + DeleteResult.FilterTime.TotalMilliseconds.ToString() + "ms");

                        Console.WriteLine("Drop Table Time                    : " + DropResult.TotalQueryTime.TotalMilliseconds.ToString() + "ms");
                        Console.WriteLine("    Parse : " + DropResult.ParseTime.TotalMilliseconds.ToString() + "ms");
                        Console.WriteLine("    Build : " + DropResult.BuildTime.TotalMilliseconds.ToString() + "ms");
                        Console.WriteLine("    Filter: " + DropResult.FilterTime.TotalMilliseconds.ToString() + "ms");

                        Console.WriteLine("Total Run Time                     : " + TotalTime.TotalMilliseconds.ToString() + "ms");


                    }
                    else
                    {
                        while (!InputLine.Contains(";"))
                            InputLine += Console.ReadLine();
                        string[] Results = InputLine.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string q in Results)
                        {
                            EsEmQuery query = db.CreateQuery(q);
                            EsEmResult Res = query.Execute();
                            string[] ColumnNames = Res.GetColumnNames();
                            for (int i = 0; i < ColumnNames.Length; i++)
                                Console.Write(ColumnNames[i] + "|");
                            Console.WriteLine();
                            while (Res.Read())
                            {
                                foreach (string cn in ColumnNames)
                                {
                                    switch (Res.GetColumnType(cn))
                                    {
                                        case ColumnType.BOOL:
                                            {
                                                Console.Write(((bool)Res[cn]).ToString() + "|");
                                            } break;
                                        case ColumnType.DATETIME:
                                            {
                                                Console.Write(((DateTime)Res[cn]).ToString() + "|");
                                            } break;
                                        case ColumnType.FLOAT:
                                            {
                                                Console.Write(((float)Res[cn]).ToString() + "|");
                                            } break;
                                        case ColumnType.INTEGER:
                                            {
                                                Console.Write(((long)Res[cn]).ToString() + "|");
                                            } break;
                                        case ColumnType.INVALID:
                                            {
                                                Console.Write("INVALID|");
                                            } break;
                                        case ColumnType.RAW:
                                            {
                                                Console.Write("RAW|");
                                            } break;
                                        case ColumnType.TEXT:
                                            {
                                                Console.Write(((string)Res[cn]) + "|");
                                            } break;
                                    }
                                }
                                Console.WriteLine();

                            }
                        }
                    }
                }
                    
            }			
		}
	}
}