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
