OOG_HOME = ..\..\..

CSC20 = "%windir%\Microsoft.NET\Framework\v2.0.50727\csc.exe"
CSC35 = "%windir%\Microsoft.NET\Framework\v3.5\csc.exe"
CSC = $(CSC35)

DIR_SRC = ..\src
DIR_LIB = ..\lib

SRCS = \
	$(DIR_SRC)\RarExtractor.cs \
	$(DIR_SRC)\RarExtractorFactory.cs \

RESOURCES = \
	/resource:$(DIR_SRC)\Oog.Plugin.RarExtractorFactory.resources \

REFS_OOG = \
	/reference:OogPlugin.dll \

REFS_CLR = \

REFS = \
	$(REFS_OOG) \
	$(REFS_CLR) \

release : OogRar.dll
debug : OogRar_debug.dll

OogRar.dll : $(SRCS) $(RESOURCES:/resource:=) $(REFS_OOG:/reference:=)
	$(CSC) /target:library /out:$@ $(REFS) $(RESOURCES) $(SRCS)

OogRar_debug.dll : $(SRCS) $(RESOURCES:/resource:=) $(REFS_OOG:/reference:=)
	$(CSC) /target:library /out:$@ $(REFS) $(RESOURCES) /debug /define:DEBUG $(SRCS)

OogPlugin.dll : $(OOG_HOME)\OogPlugin\OogPlugin.dll
	copy $** .\

clean :
	del *.dll
	del *.pdb
