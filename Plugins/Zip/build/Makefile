OOG_HOME = ..\..\..

DIR_SRC = ..\src
DIR_LIB = ..\lib

SRCS = \
	$(DIR_SRC)\EmptyExtractor.cs \
	$(DIR_SRC)\ZipExtractor.cs \
	$(DIR_SRC)\ZipExtractorFactory.cs \

RESOURCES = \
	/resource:$(DIR_SRC)\Oog.Plugin.ZipExtractorFactory.resources \

REFS_OOG = \
	/reference:OogPlugin.dll \
	/reference:ICSharpCode.SharpZipLib.dll \

REFS_CLR = \

REFS = \
	$(REFS_OOG) \
	$(REFS_CLR) \

release : OogZip.dll
debug : OogZip_debug.dll

OogZip.dll : $(SRCS) $(RESOURCES:/resource:=) $(REFS_OOG:/reference:=)
	csc /target:library /out:$@ $(REFS) $(RESOURCES) $(SRCS)

OogZip_debug.dll : $(SRCS) $(RESOURCES:/resource:=) $(REFS_OOG:/reference:=)
	csc /target:library /out:$@ $(REFS) $(RESOURCES) /debug /define:DEBUG $(SRCS)

OogPlugin.dll : $(OOG_HOME)\OogPlugin\OogPlugin.dll
	copy $** .\

ICSharpCode.SharpZipLib.dll : $(DIR_LIB)\ICSharpCode.SharpZipLib.dll
	copy $** .\

clean :
	del *.dll
	del *.pdb

# OogZip : ZipExtractor.cs ZipExtractorFactory.cs EmptyExtractor.cs
# 	csc.exe /target:library /out:OogZip.dll /r:OogPlugin.dll /r:ICSharpCode.SharpZipLib.dll /res:Oog.Plugin.ZipExtractorFactory.resources ZipExtractor.cs ZipExtractorFactory.cs EmptyExtractor.cs
# 	copy OogZip.dll ..\..\
