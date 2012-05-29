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
