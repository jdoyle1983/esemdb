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
