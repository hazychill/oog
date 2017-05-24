OOG_HOME = ..\..\..

CSC40 = "%windir%\Microsoft.NET\Framework\v4.0.30319\csc.exe"
CSC = $(CSC40)
CSC64bit = "%windir%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"

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

SHAP_ZIP_LIB_64 = ICSharpCode.SharpZipLib.dll

REFS_OOG = \
	/reference:OogPlugin.dll \
	/reference:$(SHAP_ZIP_LIB) \

REFS_OOG_64 = \
	/reference:OogPlugin64.dll \
	/reference:$(SHAP_ZIP_LIB_64) \

REFS_CLR = \

REFS_CLR_64 = \

REFS = \
	$(REFS_OOG) \
	$(REFS_CLR) \

REFS_64 = \
	$(REFS_OOG_64) \
	$(REFS_CLR_64) \

release : OogZip.dll
debug : OogZip_debug.dll
dotnet64 : OogZip64.dll

OogZip.dll : $(SRCS) $(RESOURCES:/resource:=) $(REFS_OOG:/reference:=) $(SHAP_ZIP_LIB)
	$(CSC) /target:library /out:$@ $(REFS) $(RESOURCES) $(SRCS)

OogZip64.dll : $(SRCS) $(RESOURCES:/resource:=) $(REFS_OOG_64:/reference:=) $(SHAP_ZIP_LIB_64)
	$(CSC64bit) /target:library /out:$@ $(REFS_64) $(RESOURCES) $(SRCS)

OogZip_debug.dll : $(SRCS) $(RESOURCES:/resource:=) $(REFS_OOG:/reference:=) $(SHAP_ZIP_LIB)
	$(CSC) /target:library /out:$@ $(REFS) $(RESOURCES) /debug /define:DEBUG $(SRCS)

$(SHAP_ZIP_LIB) : $(DIR_LIB)\$(SHAP_ZIP_LIB)
	copy $(DIR_LIB)\$(SHAP_ZIP_LIB) .\

$(SHAP_ZIP_LIB_64) : $(DIR_LIB)\$(SHAP_ZIP_LIB_64)
	copy $(DIR_LIB)\$(SHAP_ZIP_LIB_64) .\

OogPlugin.dll : $(OOG_HOME)\OogPlugin\OogPlugin.dll
	copy $** .\

OogPlugin64.dll : $(OOG_HOME)\OogPlugin\OogPlugin64.dll
	copy $** .\

clean :
	del *.dll
	del *.pdb
