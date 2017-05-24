CSC20 = %windir%\Microsoft.NET\Framework\v2.0.50727\csc.exe
CSC35 = %windir%\Microsoft.NET\Framework\v3.5\csc.exe
CSC = $(CSC35)
CSC64bit = "%windir%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"

DIR_SRC = ..\src

SRCS = \
	$(DIR_SRC)\PluginHelper.cs \
	$(DIR_SRC)\IExtractor.cs \
	$(DIR_SRC)\IExtractor2.cs \
	$(DIR_SRC)\IExtractorFactory.cs \
	$(DIR_SRC)\IImageCreator.cs \
	$(DIR_SRC)\AssemblyInfo.cs \

release : OogPlugin.dll
dotnet64 : OogPlugin64.dll

OogPlugin.dll : $(SRCS)
	$(CSC) /target:library /out:$@ $**

OogPlugin64.dll : $(SRCS)
	$(CSC64bit) /target:library /out:$@ $**
