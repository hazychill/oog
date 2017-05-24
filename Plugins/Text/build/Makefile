OOG_HOME = ..\..\..

CSC40 = "%windir%\Microsoft.NET\Framework\v4.0.30319\csc.exe"
CSC = $(CSC40)
CSC64bit = "%windir%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"

DIR_SRC = ..\src

SRCS = \
	$(DIR_SRC)\TextImageCreator.cs \


REFS_OOG = \
	/reference:OogPlugin.dll \

REFS_OOG_64 = \
	/reference:OogPlugin64.dll \

REFS_CLR = \

REFS_CLR_64 = \

REFS = \
	$(REFS_OOG) \
	$(REFS_CLR) \

REFS_64 = \
	$(REFS_OOG_64) \
	$(REFS_CLR_64) \

release : OogText.dll
debug : OogText_debug.dll
dotnet64 : OogText64.dll

OogText.dll : $(SRCS) $(REFS_OOG:/reference:=)
	$(CSC) /target:library /out:$@ $(REFS) $(SRCS)

OogText_debug.dll : $(SRCS) $(REFS_OOG:/reference:=)
	$(CSC) /target:library /out:$@ $(REFS) /debug /define:DEBUG $(SRCS)

OogText64.dll : $(SRCS) $(REFS_OOG_64:/reference:=)
	$(CSC64bit) /target:library /out:$@ $(REFS_64) $(SRCS)

OogPlugin.dll : $(OOG_HOME)\OogPlugin\OogPlugin.dll
	copy $** .\

OogPlugin64.dll : $(OOG_HOME)\OogPlugin\OogPlugin64.dll
	copy $** .\

clean :
	del *.dll
	del *.pdb
