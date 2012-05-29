CC = gcc

WIN_RM = del
LIN_RM = rm -f

BINLIBS = -lEsEmDb
INCS = -I"include"

CC_PARMS = -c -Wall $(INCS)

WIN_LIBOBJ = obj\DbTableIter.o obj\EsEmColumn.o obj\EsEmDb.o obj\EsEmQuery.o obj\EsEmTable.o obj\SimpleParser.o obj\DbHeader.o obj\DbTools.o obj\EsEmDatabase.o obj\EsEmDbLib.o obj\EsEmResult.o obj\OffsetLink.o
LIN_LIBOBJ = obj/DbTableIter.o obj/EsEmColumn.o obj/EsEmDb.o obj/EsEmQuery.o obj/EsEmTable.o obj/SimpleParser.o obj/DbHeader.o obj/DbTools.o obj/EsEmDatabase.o obj/EsEmDbLib.o obj/EsEmResult.o obj/OffsetLink.o
WIN_BINOBJ =
WIN_BINOBJ =

WIN_LIB = lib\libEsEmDb.a
LIN_LIB = lib/libEsEmDb.a
WIN_BIN = bin\EsEmDbTest.exe
LIN_BIN = bin/EsEmDbTest

.PHONY: all all-before all-after clean clean-custom

all:
	echo "make (windows | linux | windowslib | linuxlib | windowstest | linuxtest | windowsclean | linuxclean)"


windows: $(WIN_LIB) $(WIN_BIN)
windowslib: $(WIN_LIB)
windowstest: $(WIN_LIB) $(WIN_BIN)
windowsclean:
	$(WIN_RM) $(WIN_LIBOBJ) $(WIN_BINOBJ) $(WIN_LIB) $(WIN_BIN)

linux: $(LIN_LIB) $(LIN_BIN)
linuxlib: $(LIN_LIB)
linuxtest: $(LIN_LIB) $(LIN_BIN)
linuxclean:
	$(LIN_RM) $(LIN_LIBOBJ) $(LIN_BINOBJ) $(LIN_LIB) $(LIN_BIN)

$(WIN_LIB) : $(WIN_LIBOBJ)
	ar rcs $(WIN_LIB) $(WIN_LIBOBJ)
	ranlib $(WIN_LIB)

$(WIN_BIN) : $(WIN_BINOBJ)
	$(CC) -o $(WIN_BIN) $(WIN_BINOBJ)

$(LIN_LIB) : $(LIN_LIBOBJ)
	ar rcs $(LIN_LIB) $(LIN_LIBOBJ)
	ranlib $(LIN_LIB)

$(LIN_BIN) : $(LIN_BINOBJ)
	$(CC) -o $(LIN_BIN) $(LIN_BINOBJ)









obj\DbTableIter.o : src\DbTableIter.c
	$(CC) $(CC_PARMS) src\DbTableIter.c -o obj\DbTableIter.o

obj\EsEmColumn.o : src\EsEmColumn.c
	$(CC) $(CC_PARMS) src\EsEmColumn.c -o obj\EsEmColumn.o

obj\EsEmDb.o : src\EsEmDb.c
	$(CC) $(CC_PARMS) src\EsEmDb.c -o obj\EsEmDb.o

obj\EsEmQuery.o : src\EsEmQuery.c
	$(CC) $(CC_PARMS) src\EsEmQuery.c -o obj\EsEmQuery.o

obj\EsEmTable.o : src\EsEmTable.c
	$(CC) $(CC_PARMS) src\EsEmTable.c -o obj\EsEmTable.o

obj\SimpleParser.o : src\SimpleParser.c
	$(CC) $(CC_PARMS) src\SimpleParser.c -o obj\SimpleParser.o

obj\DbHeader.o : src\DbHeader.c
	$(CC) $(CC_PARMS) src\DbHeader.c -o obj\DbHeader.o

obj\DbTools.o : src\DbTools.c
	$(CC) $(CC_PARMS) src\DbTools.c -o obj\DbTools.o

obj\EsEmDatabase.o : src\EsEmDatabase.c
	$(CC) $(CC_PARMS) src\EsEmDatabase.c -o obj\EsEmDatabase.o

obj\EsEmDbLib.o : src\EsEmDbLib.c
	$(CC) $(CC_PARMS) src\EsEmDbLib.c -o obj\EsEmDbLib.o

obj\EsEmResult.o : src\EsEmResult.c
	$(CC) $(CC_PARMS) src\EsEmResult.c -o obj\EsEmResult.o

obj\OffsetLink.o : src\OffsetLink.c
	$(CC) $(CC_PARMS) src\OffsetLink.c -o obj\OffsetLink.o











obj/DbTableIter.o : src/DbTableIter.c
	$(CC) $(CC_PARMS) src/DbTableIter.c -o obj/DbTableIter.o

obj/EsEmColumn.o : src/EsEmColumn.c
	$(CC) $(CC_PARMS) src/EsEmColumn.c -o obj/EsEmColumn.o

obj/EsEmDb.o : src/EsEmDb.c
	$(CC) $(CC_PARMS) src/EsEmDb.c -o obj/EsEmDb.o

obj/EsEmQuery.o : src/EsEmQuery.c
	$(CC) $(CC_PARMS) src/EsEmQuery.c -o obj/EsEmQuery.o

obj/EsEmTable.o : src/EsEmTable.c
	$(CC) $(CC_PARMS) src/EsEmTable.c -o obj/EsEmTable.o

obj/SimpleParser.o : src/SimpleParser.c
	$(CC) $(CC_PARMS) src/SimpleParser.c -o obj/SimpleParser.o

obj/DbHeader.o : src/DbHeader.c
	$(CC) $(CC_PARMS) src/DbHeader.c -o obj/DbHeader.o

obj/DbTools.o : src/DbTools.c
	$(CC) $(CC_PARMS) src/DbTools.c -o obj/DbTools.o

obj/EsEmDatabase.o : src/EsEmDatabase.c
	$(CC) $(CC_PARMS) src/EsEmDatabase.c -o obj/EsEmDatabase.o

obj/EsEmDbLib.o : src/EsEmDbLib.c
	$(CC) $(CC_PARMS) src/EsEmDbLib.c -o obj/EsEmDbLib.o

obj/EsEmResult.o : src/EsEmResult.c
	$(CC) $(CC_PARMS) src/EsEmResult.c -o obj/EsEmResult.o

obj/OffsetLink.o : src/OffsetLink.c
	$(CC) $(CC_PARMS) src/OffsetLink.c -o obj/OffsetLink.o
