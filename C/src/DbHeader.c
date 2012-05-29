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

int ReadHeader( DbHeader* Header, FILE* Stream )
{
    fread( Header->DbMagic, sizeof(char), 6, Stream );
    if( Header->DbMagic[0] == 'E' &&
        Header->DbMagic[1] == 's' &&
        Header->DbMagic[2] == 'E' &&
        Header->DbMagic[3] == 'm' &&
        Header->DbMagic[4] == 'D' &&
        Header->DbMagic[5] == 'b' )
    {
        fread( &Header->DbVersionMajor, sizeof(int), 1, Stream );
        fread( &Header->DbVersionMinor, sizeof(int), 1, Stream );
        ReadOffsetLink( Header->Link, Stream );
    }
    else
        return ESEMDB_ERROR;
    return ESEMDB_OK;
};

void WriteHeader( DbHeader* Header, FILE* Stream )
{
    fseek( Stream, 0, SEEK_SET );
    Header->DbMagic[0] = 'E';
    Header->DbMagic[1] = 's';
    Header->DbMagic[2] = 'E';
    Header->DbMagic[3] = 'm';
    Header->DbMagic[4] = 'D';
    Header->DbMagic[5] = 'b';
    Header->DbVersionMajor = 0;
    Header->DbVersionMinor = 0;

    fwrite( Header->DbMagic, sizeof(char), 6, Stream );
    fwrite( &Header->DbVersionMajor, sizeof(int), 1, Stream );
    fwrite( &Header->DbVersionMinor, sizeof(int), 1, Stream );
    WriteOffsetLink( Header->Link, Stream );
};
