OOG_HOME = ..\..\..\..

CSC40 = "%windir%\Microsoft.NET\Framework\v4.0.21006\csc.exe"
CSC = $(CSC40)

DIR_SRC = ..\src
DIR_LIB = ..\lib

SRCS = \
	$(DIR_SRC)\ProgressCallback.cs \
	$(DIR_SRC)\BitmapInfo.cs \
	$(DIR_SRC)\BytePointerStream.cs \
	$(DIR_SRC)\DataLocation.cs \
	$(DIR_SRC)\ErrorCode.cs \
	$(DIR_SRC)\fileInfo.cs \
	$(DIR_SRC)\PictureInfo.cs \
	$(DIR_SRC)\ApiFlags.cs \

RELEASE = ..\SpiUtility.dll
DEBUG = ..\SpiUtility_debug.dll

release : $(RELEASE)
debug : $(DEBUG)

$(RELEASE) : $(SRCS)
	$(CSC) /target:library /out:$@ /unsafe $(SRCS)

$(DEBUG) : $(SRCS)
	$(CSC) /target:library /out:$@ /unsafe /debug /define:DEBUG $(SRCS)

clean :
	del $(RELEASE)
	del $(DEBUG)
	del $(DEBUG:dll=pdb)
