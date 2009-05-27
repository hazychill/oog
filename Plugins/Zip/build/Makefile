OOG_HOME = ..\..\..

CSC20 = "%windir%\Microsoft.NET\Framework\v2.0.50727\csc.exe"
CSC35 = "%windir%\Microsoft.NET\Framework\v3.5\csc.exe"
CSC = $(CSC35)

DIR_SRC = ..\src
DIR_SRC_SZL = ..\src\SharpZipLib
DIR_LIB = ..\lib

SRCS = \
	$(DIR_SRC)\EmptyExtractor.cs \
	$(DIR_SRC)\ZipExtractor.cs \
	$(DIR_SRC)\ZipExtractorFactory.cs \
	$(DIR_SRC)\ZipImageCreator.cs \

RESOURCES = \
	/resource:$(DIR_SRC)\Oog.Plugin.ZipExtractorFactory.resources \

SHAP_ZIP_LIB = ICSharpCode.SharpZipLib.dll

REFS_OOG = \
	/reference:OogPlugin.dll \
	/reference:$(SHAP_ZIP_LIB) \

REFS_CLR = \

REFS = \
	$(REFS_OOG) \
	$(REFS_CLR) \

release : OogZip.dll
debug : OogZip_debug.dll

OogZip.dll : $(SRCS) $(RESOURCES:/resource:=) $(REFS_OOG:/reference:=) $(SHAP_ZIP_LIB)
	$(CSC) /target:library /out:$@ $(REFS) $(RESOURCES) $(SRCS)

OogZip_debug.dll : $(SRCS) $(RESOURCES:/resource:=) $(REFS_OOG:/reference:=) $(SHAP_ZIP_LIB)
	$(CSC) /target:library /out:$@ $(REFS) $(RESOURCES) /debug /define:DEBUG $(SRCS)

$(SHAP_ZIP_LIB) : $(DIR_LIB)\$(SHAP_ZIP_LIB)
	copy $(DIR_LIB)\$(SHAP_ZIP_LIB) .\

OogPlugin.dll : $(OOG_HOME)\OogPlugin\OogPlugin.dll
	copy $** .\

clean :
	del *.dll
	del *.pdb
