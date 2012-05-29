#ifndef _DBHEADER_H_
#define _DBHEADER_H_

typedef struct
{
    char DbMagic[6];
    int DbVersionMajor;
    int DbVersionMinor;
    OffsetLink* Link;
} DbHeader;

int ReadHeader( DbHeader* Header, FILE* Stream );
void WriteHeader( DbHeader* Header, FILE* Stream );

#endif
