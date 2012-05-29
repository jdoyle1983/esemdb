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
