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
