OOG_HOME = ..\..\..

CSC40 = "%windir%\Microsoft.NET\Framework\v4.0.30319\csc.exe"
CSC = $(CSC40)
CSC64bit = "%windir%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"

DIR_SRC = ..\src
DIR_LIB = ..\lib

SRCS = \
	$(DIR_SRC)\RarExtractor.cs \
	$(DIR_SRC)\RarExtractorFactory.cs \

RESOURCES = \
	/resource:$(DIR_SRC)\Oog.Plugin.RarExtractorFactory.resources \

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

release : OogRar.dll
debug : OogRar_debug.dll
dotnet64 : OogRar64.dll

OogRar.dll : $(SRCS) $(RESOURCES:/resource:=) $(REFS_OOG:/reference:=)
	$(CSC) /target:library /out:$@ $(REFS) $(RESOURCES) $(SRCS)

OogRar_debug.dll : $(SRCS) $(RESOURCES:/resource:=) $(REFS_OOG:/reference:=)
	$(CSC) /target:library /out:$@ $(REFS) $(RESOURCES) /debug /define:DEBUG $(SRCS)

OogRar64.dll : $(SRCS) $(RESOURCES:/resource:=) $(REFS_OOG_64:/reference:=)
	$(CSC64bit) /target:library /out:$@ $(REFS_64) $(RESOURCES) $(SRCS)

OogPlugin.dll : $(OOG_HOME)\OogPlugin\OogPlugin.dll
	copy $** .\

OogPlugin64.dll : $(OOG_HOME)\OogPlugin\OogPlugin64.dll
	copy $** .\

clean :
	del *.dll
	del *.pdb
