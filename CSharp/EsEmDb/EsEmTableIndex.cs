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

