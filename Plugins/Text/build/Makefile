OOG_HOME = ..\..\..

CSC20 = "%windir%\Microsoft.NET\Framework\v2.0.50727\csc.exe"
CSC35 = "%windir%\Microsoft.NET\Framework\v3.5\csc.exe"
CSC = $(CSC35)

DIR_SRC = ..\src

SRCS = \
	$(DIR_SRC)\TextImageCreator.cs \


REFS_OOG = \
	/reference:OogPlugin.dll \

REFS_CLR = \

REFS = \
	$(REFS_OOG) \
	$(REFS_CLR) \

release : OogText.dll
debug : OogText_debug.dll

OogText.dll : $(SRCS) $(REFS_OOG:/reference:=)
	csc /target:library /out:$@ $(REFS) $(SRCS)

OogText_debug.dll : $(SRCS) $(REFS_OOG:/reference:=)
	csc /target:library /out:$@ $(REFS) /debug /define:DEBUG $(SRCS)

OogPlugin.dll : $(OOG_HOME)\OogPlugin\OogPlugin.dll
	copy $** .\

clean :
	del *.dll
	del *.pdb
