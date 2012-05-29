#ifndef _OFFSETLINK_H_
#define _OFFSETLINK_H_

typedef struct
{
    long Next;
    long Prev;
    long First;
    long Last;
} OffsetLink;

void WriteOffsetLink( OffsetLink* Link, FILE* Stream );
void ReadOffsetLink( OffsetLink* Link, FILE* Stream );

#endif
