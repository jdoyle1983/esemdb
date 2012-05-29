
using System;
using System.IO;

namespace EsEmDb
{
	
	
	public class OffsetLink
	{
		public long Next = -1;
		public long Prev = -1;
		public long First = -1;
		public long Last = -1;	
		
		public void Write( ref FileStream Stream )
		{
			BinaryWriter Writer = new BinaryWriter( Stream );
			Writer.Write(Next);
			Writer.Write(Prev);
			Writer.Write(First);
			Writer.Write(Last);
		}
		
		public void Read( ref FileStream Stream )
		{
			BinaryReader Reader = new BinaryReader( Stream );
			Next = Reader.ReadInt64();
			Prev = Reader.ReadInt64();
			First = Reader.ReadInt64();
			Last = Reader.ReadInt64();
		}
	}
}
