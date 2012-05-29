#ifndef _ESEMTABLE_H_
#define _ESEMTABLE_H_

typedef struct
{
    void* _Database;
    bool _TableLocked;
    char* _TableName;
    void** _TableColumns;
    long _TableOffset;
    void* _TableLink;
    void* _RowLink;
    void** _TableCache;
} EsEmTable;

#endif
