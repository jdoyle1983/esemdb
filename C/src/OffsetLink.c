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

void WriteOffsetLink( OffsetLink* Link, FILE* Stream )
{
    fwrite( &Link->Next, sizeof(long), 1, Stream );
    fwrite( &Link->Prev, sizeof(long), 1, Stream );
    fwrite( &Link->First, sizeof(long), 1, Stream );
    fwrite( &Link->Last, sizeof(long), 1, Stream );
}

void ReadOffsetLink( OffsetLink* Link, FILE* Stream )
{
    fread( &Link->Next, sizeof(long), 1, Stream );
    fread( &Link->Prev, sizeof(long), 1, Stream );
    fread( &Link->First, sizeof(long), 1, Stream );
    fread( &Link->Last, sizeof(long), 1, Stream );
}
