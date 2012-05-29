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
