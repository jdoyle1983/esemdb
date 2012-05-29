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

#include <EsEmDbLib.h>

EsEmColumn* CreateColumn( void* Table, char* ColumnName, int DataType, bool IsAutoIncrement, bool IsPrimaryKey )
{
    EsEmColumn* c = malloc(sizeof(EsEmColumn));
    c->_Table = Table;
    if((IsAutoIncrement || IsPrimaryKey) && DataType != ESEMDB_INTEGER)
    {
        free( c );
        return NULL;
    }
    c->_AutoIncrement = IsAutoIncrement;
    if( strlen( ColumnName ) > 127 )
    {
        free( c );
        return NULL;
    }
    strcpy( c->_ColumnName, ColumnName );
    c->_ColumnType = DataType;
    c->_NextAutoIncrementId = 1;
    c->_PrimaryKey = IsPrimaryKey;
    return c;
};

void ReadColumnHeader( EsEmColumn* Column, FILE* Stream )
{
    fread( Column->_ColumnName, sizeof(char), 128, Stream );
    fread( &Column->_ColumnType, sizeof(int), 1, Stream );
    fread( &Column->_AutoIncrement, sizeof(bool), 1, Stream );
    fread( &Column->_PrimaryKey, sizeof(bool), 1, Stream );
    if( Column->_AutoIncrement != 0 )
        fread( &Column->_NextAutoIncrementId, sizeof(long), 1, Stream );
};

void WriteColumnHeader( EsEmColumn* Column, FILE* Stream )
{
    fwrite( Column->_ColumnName, sizeof(char), 128, Stream );
    fwrite( &Column->_ColumnType, sizeof(int), 1, Stream );
    fwrite( &Column->_AutoIncrement, sizeof(bool), 1, Stream );
    fwrite( &Column->_PrimaryKey, sizeof(bool), 1, Stream );
    if( Column->_AutoIncrement != 0 )
        fwrite( &Column->_NextAutoIncrementId, sizeof(long), 1, Stream );
};
