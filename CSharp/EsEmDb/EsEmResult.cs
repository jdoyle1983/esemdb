
using System;
using System.Collections.ObjectModel;

namespace EsEmDb
{
	public class EsEmResult
	{
		internal Collection<string> _ColumnNames = new Collection<string>();
		internal Collection<ColumnType> _ColumnTypes = new Collection<ColumnType>();
		internal Collection<Collection<object>> _ColumnValues = new Collection<Collection<object>>();
		private long InternalIndex = -1;

        internal TimeSpan _TotalQueryTime = new TimeSpan(0);
        internal TimeSpan _ParseTime = new TimeSpan(0);
        internal TimeSpan _BuildTime = new TimeSpan(0);
        internal TimeSpan _FilterTime = new TimeSpan(0);

        public TimeSpan TotalQueryTime
        {
            get
            {
                return _TotalQueryTime;
            }
        }
        public TimeSpan ParseTime
        {
            get
            {
                return _ParseTime;
            }
        }
        public TimeSpan BuildTime
        {
            get
            {
                return _BuildTime;
            }
        }
        public TimeSpan FilterTime
        {
            get
            {
                return _FilterTime;
            }
        }
		
		public object this[int Index]
		{
			get
			{
                if (InternalIndex > -1 && InternalIndex < _ColumnValues.Count)
                    return _ColumnValues[(int)InternalIndex][Index];
				throw new Exception("No Result Row To Display");
			}
		}
		
		public object this[string Column]
		{
			get
			{
                for (int i = 0; i < _ColumnNames.Count; i++)
                    if (_ColumnNames[i] == Column)
                        return _ColumnValues[(int)InternalIndex][i];
				throw new Exception("Column '" + Column + "' Not Found In Results");
			}
		}
		
		public bool HasRows
		{
			get
			{
				if(_ColumnValues.Count > 0)
					return true;
				return false;
			}
		}

        public string[] GetColumnNames()
        {
            string[] rValue = new string[_ColumnNames.Count];
            _ColumnNames.CopyTo(rValue, 0);
            return rValue;
        }
		
		public ColumnType GetColumnType(string ColumnName)
		{
			for( int i = 0; i < _ColumnNames.Count; i++ )
				if( _ColumnNames[i] == ColumnName )
					return _ColumnTypes[i];
			return ColumnType.INVALID;
		}
		
		public bool Read()
		{
			InternalIndex++;
			if(InternalIndex >= _ColumnValues.Count)
				return false;
			return true;
		}

        internal void OrderBy(string ColumnName, bool Descending)
        {
        }
		
	}
}
