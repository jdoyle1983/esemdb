
using System;
using System.IO;
using System.Collections.ObjectModel;

namespace EsEmDb
{
	internal class DbHeader
	{
		public byte[] DbMagic = new byte[6];
		public int DbVersionMajor = 0;
		public int DbVersionMinor = 0;
		public OffsetLink Link = new OffsetLink();
		
		public void ReadHeader( ref FileStream Stream )
		{
			BinaryReader Reader = new BinaryReader(Stream);
			DbMagic = Reader.ReadBytes(6);
			if(	DbMagic[0] == Convert.ToByte('E') &&
			   	DbMagic[1] == Convert.ToByte('s') &&
			   	DbMagic[2] == Convert.ToByte('E') &&
			   	DbMagic[3] == Convert.ToByte('m') &&
			   	DbMagic[4] == Convert.ToByte('D') &&
			   	DbMagic[5] == Convert.ToByte('b') )
			{
				DbVersionMajor = Reader.ReadInt32();
				DbVersionMinor = Reader.ReadInt32();
				Link.Read( ref Stream );
				
				/*Console.WriteLine("First Table Offset   : " + Link.First.ToString());
				Console.WriteLine("Last Table Offset    : " + Link.Last.ToString());*/
			}
			else
				throw new Exception("NOTDB");                        
		}		
		
		public void WriteHeader( ref FileStream Stream )
		{
			Stream.Seek(0,SeekOrigin.Begin);
			BinaryWriter Writer = new BinaryWriter( Stream );
			DbMagic[0] = Convert.ToByte('E');
			DbMagic[1] = Convert.ToByte('s');
			DbMagic[2] = Convert.ToByte('E');
			DbMagic[3] = Convert.ToByte('m');
			DbMagic[4] = Convert.ToByte('D');
			DbMagic[5] = Convert.ToByte('b');
			Writer.Write(DbMagic);
			Writer.Write(DbVersionMajor);
			Writer.Write(DbVersionMinor);
			Link.Write( ref Stream );
		}
	}
}
