OOG_HOME = ..\..\..

CSC20 = "%windir%\Microsoft.NET\Framework\v2.0.50727\csc.exe"
CSC35 = "%windir%\Microsoft.NET\Framework\v3.5\csc.exe"
CSC = $(CSC35)

DIR_SRC = ..\src
DIR_SRC_SZL = ..\src\SharpZipLib
DIR_LIB = ..\lib

SRCS_OOG = \
	$(DIR_SRC)\EmptyExtractor.cs \
	$(DIR_SRC)\ZipExtractor.cs \
	$(DIR_SRC)\ZipExtractorFactory.cs \
	$(DIR_SRC)\ZipImageCreator.cs \

SRCS_SZL = \
	$(DIR_SRC_SZL)\BZip2\*.cs \
	$(DIR_SRC_SZL)\Checksums\*.cs \
	$(DIR_SRC_SZL)\Core\*.cs \
	$(DIR_SRC_SZL)\Encryption\*.cs \
	$(DIR_SRC_SZL)\GZip\*.cs \
	$(DIR_SRC_SZL)\*.cs \
	$(DIR_SRC_SZL)\Tar\*.cs \
	$(DIR_SRC_SZL)\Zip\Compression\*.cs \
	$(DIR_SRC_SZL)\Zip\Compression\Streams\*.cs \
	$(DIR_SRC_SZL)\Zip\*.cs \

SRCS = \
	$(SRCS_OOG) \
	$(SRCS_SZL) \

RESOURCES = \
	/resource:$(DIR_SRC)\Oog.Plugin.ZipExtractorFactory.resources \

REFS_OOG = \
	/reference:OogPlugin.dll \

REFS_CLR = \

REFS = \
	$(REFS_OOG) \
	$(REFS_CLR) \

release : OogZip.dll
debug : OogZip_debug.dll

OogZip.dll : $(SRCS) $(RESOURCES:/resource:=) $(REFS_OOG:/reference:=)
	$(CSC) /target:library /out:$@ $(REFS) $(RESOURCES) $(SRCS)

OogZip_debug.dll : $(SRCS) $(RESOURCES:/resource:=) $(REFS_OOG:/reference:=)
	$(CSC) /target:library /out:$@ $(REFS) $(RESOURCES) /debug /define:DEBUG $(SRCS)

OogPlugin.dll : $(OOG_HOME)\OogPlugin\OogPlugin.dll
	copy $** .\

clean :
	del *.dll
	del *.pdb
