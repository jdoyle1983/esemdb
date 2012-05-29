
using System;
using System.Text;
using System.Threading;

namespace EsEmDb
{
	internal class DbTools
	{	
		private static Mutex DbFileLocker = new Mutex();
		
		public static byte[] DbItemNameToByteArray(string Name, int ArrayLen)
		{
			byte[] bytes = new byte[ArrayLen];
			for( int i = 0; i < ArrayLen; i++ )
			{
				if(i < Name.Length)
					bytes[i] = Convert.ToByte(Name[i]);
				else
					bytes[i] = 0;
			}
			return bytes;
		}
		
		public static string ByteArrayToDbItemName(byte[] Bytes)
		{
			string rValue = "";
			for( int i = 0; i < Bytes.Length; i++)
				if(Bytes[i] != 0)
					rValue += Convert.ToChar(Bytes[i]);
			return rValue.Trim();
		}
		
		public static byte[] StringToByteArray(string Src)
		{
			ASCIIEncoding Encoding = new ASCIIEncoding();
			return Encoding.GetBytes(Src);
		}
		
		public static string ByteArrayToString(byte[] Src)
		{
			ASCIIEncoding Encoding = new ASCIIEncoding();
			return Encoding.GetString(Src);
		}
		
		public static Int16 GetColumnTypeId( ColumnType cType )
		{
			switch(cType)
			{
			case ColumnType.INTEGER:
				return 1;
			case ColumnType.FLOAT:
				return 2;
			case ColumnType.TEXT:
				return 3;
			case ColumnType.RAW:
				return 4;
			case ColumnType.DATETIME:
				return 5;
			case ColumnType.BOOL:
				return 6;
			}
			return -1;
		}
		
		public static ColumnType GetColumnType( Int16 TypeId )
		{
			switch(TypeId)
			{
			case 1:
				return ColumnType.INTEGER;
			case 2:
				return ColumnType.FLOAT;
			case 3:
				return ColumnType.TEXT;
			case 4:
				return ColumnType.RAW;
			case 5:
				return ColumnType.DATETIME;
			case 6:
				return ColumnType.BOOL;
			}
			return ColumnType.RAW;
		}

        public static ColumnType GetColumnType(string TypeName)
        {
            switch (TypeName.ToLower())
            {
                case "integer":
                    return ColumnType.INTEGER;
                case "float":
                    return ColumnType.FLOAT;
                case "text":
                    return ColumnType.TEXT;
                case "raw":
                    return ColumnType.RAW;
                case "datetime":
                    return ColumnType.DATETIME;
                case "bool":
                    return ColumnType.BOOL;
            }
            return ColumnType.INVALID;
        }

        public static string GetColumnTypeName(ColumnType t)
        {
            switch (t)
            {
                case ColumnType.BOOL:
                    return "BOOL";
                case ColumnType.DATETIME:
                    return "DATETIME";
                case ColumnType.FLOAT:
                    return "FLOAT";
                case ColumnType.INTEGER:
                    return "INTEGER";
                case ColumnType.RAW:
                    return "RAW";
                case ColumnType.TEXT:
                    return "TEXT";
            }
            return "INVALID";
        }
		
		public static void WaitForFile()
		{
			DbFileLocker.WaitOne();
		}
		
		public static void ReleaseFile()
		{
			DbFileLocker.ReleaseMutex();
		}
		
		public static string GetBlockofTextBetween( string Src, int StartIndex, char Term, out int NewIndex )
		{
			if(StartIndex + 1 < Src.Length)
			{
				int EndIndex = Src.IndexOf( Term, StartIndex + 1 );
				if(EndIndex > 0)
				{
					NewIndex = EndIndex + 1;
					return Src.Substring(StartIndex, EndIndex - StartIndex);
				}
			}
			NewIndex = -1;
			return "";
		}
	}
}
