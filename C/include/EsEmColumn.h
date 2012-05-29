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
