OOG_HOME = ..\..\..

CSC40 = "%windir%\Microsoft.NET\Framework\v4.0.21006\csc.exe"
CSC = $(CSC40)

DIR_SRC = ..\src
DIR_LIB = ..\lib

SRCS = \
	$(DIR_SRC)\XacRettExtractor.cs \
	$(DIR_SRC)\XacRettExtractorFactory.cs \

RESOURCES = \
	/resource:$(DIR_SRC)\Oog.Plugin.XacRettExtractorFactory.resources \

REFS_OOG = \
	/reference:OogPlugin.dll \

REFS_CLR = \

REFS = \
	$(REFS_OOG) \
	$(REFS_CLR) \

release : OogXacRett.dll
debug : OogXacRett_debug.dll

OogXacRett.dll : $(SRCS) $(RESOURCES:/resource:=) $(REFS_OOG:/reference:=)
	$(CSC) /target:library /out:$@ $(REFS) $(RESOURCES) $(SRCS)

OogXacRett_debug.dll : $(SRCS) $(RESOURCES:/resource:=) $(REFS_OOG:/reference:=)
	$(CSC) /target:library /out:$@ $(REFS) $(RESOURCES) /debug /define:DEBUG $(SRCS)

OogPlugin.dll : $(OOG_HOME)\OogPlugin\OogPlugin.dll
	copy $** .\

clean :
	del *.dll
	del *.pdb
