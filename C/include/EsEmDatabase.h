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

#ifndef _ESEMDATABASE_H_
#define _ESEMDATABASE_H_

typedef struct
{
    long VarId;
    char* VarName;
    char* VarValue;
} SystemVar;

typedef struct
{
    char* _FilePath;
    void** _Tables;
    void* _Header;
    void** SysVars;
    bool _CacheTables;
} EsEmDatabase;

#endif
