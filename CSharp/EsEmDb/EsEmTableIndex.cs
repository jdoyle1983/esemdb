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

namespace EsEmDb
{
	internal struct IndexNode
	{
		public long RowOffset;
		public int CacheOffset;
		public object Value;
	}
	
	internal class EsEmTableIndex
	{
		EsEmTable _Table = null;
		private Collection<Collection<IndexNode>> _Indices = new Collection<Collection<IndexNode>>();
		private Collection<bool> _IsIndexed = new Collection<bool>();
		
		public EsEmTableIndex (ref EsEmTable Table)
		{
			_Table = Table;
		}
		
		public void BuildIndex()
		{
			_Indices = new Collection<Collection<IndexNode>>();
			//_IsIndexed =  false;
			
			for(int i = 0; i < _Table.ColumnCount; i++)
			{
				switch(_Table[i].GetColumnType)
				{
				case ColumnType.BOOL:
				{
					}break;
				case ColumnType.DATETIME:
				{
					}break;
				case ColumnType.FLOAT:
				{
					}break;
				case ColumnType.INTEGER:
				{
					}break;
				case ColumnType.RAW:
				{
					_Indices.Add(new Collection<IndexNode>());
					_IsIndexed.Add(false);
					}break;
				case ColumnType.TEXT:
				{
					}break;
				}
			}
		}
	}
}

