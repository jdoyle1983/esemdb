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

#ifndef _ESEMCOLUMN_H_
#define _ESEMCOLUMN_H_

typedef struct
{
    void* _Table;
    bool _AutoIncrement;
    bool _PrimaryKey;
    long _NextAutoIncrementId;
    int _ColumnType;
    char _ColumnName[128];
} EsEmColumn;

EsEmColumn* CreateColumn( void* Table, char* ColumnName, int DataType, bool IsAutoIncrement, bool IsPrimaryKey );
void ReadColumnHeader( EsEmColumn* Column, FILE* Stream );
void WriteColumnHeader( EsEmColumn* Column, FILE* Stream );

#endif
