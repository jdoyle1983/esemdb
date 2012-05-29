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

#ifndef _ESEMDB_H_
#define _ESEMDB_H_

#define bool char

enum
{
    ESEMDB_OK,
    ESEMDB_ERROR,
};

enum
{
	ESEMDB_INTEGER,
	ESEMDB_FLOAT,
	ESEMDB_TEXT,
	ESEMDB_RAW,
	ESEMDB_DATETIME,
	ESEMDB_BOOL,
	ESEMDB_INVALID,
};

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <EsEmColumn.h>
#include <EsEmTable.h>
#include <EsEmDatabase.h>
#include <EsEmQuery.h>
#include <EsEmResult.h>

#endif
