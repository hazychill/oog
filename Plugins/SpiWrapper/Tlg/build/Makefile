OOG_HOME = ..\..\..\..

CSC20 = "%windir%\Microsoft.NET\Framework\v2.0.50727\csc.exe"
CSC35 = "%windir%\Microsoft.NET\Framework\v3.5\csc.exe"
CSC = $(CSC35)

DIR_SRC = ..\src
DIR_LIB = ..\lib

SRCS = \
	$(DIR_SRC)\TlgImageCreator.cs \

REFS_OOG = \
	/reference:OogPlugin.dll \
	/reference:SpiUtility.dll \

REFS_OOG_DEBUG = \
	/reference:OogPlugin.dll \
	/reference:SpiUtility_debug.dll \

REFS_CLR = \

REFS = \
	$(REFS_OOG) \
	$(REFS_CLR) \

REFS_DEBUG = \
	$(REFS_OOG_DEBUG) \
	$(REFS_CLR) \

SPI = if_tlg.spi

release : OogTlg.dll
debug : OogTlg_debug.dll

OogTlg.dll : $(SRCS) $(REFS_OOG:/reference:=) $(SPI)
	$(CSC) /target:library /out:$@ /unsafe $(REFS) $(SRCS)

OogTlg_debug.dll : $(SRCS) $(REFS_OOG_DEBUG:/reference:=)  $(SPI)
	$(CSC) /target:library /out:$@ /unsafe $(REFS_DEBUG) /debug /define:DEBUG $(SRCS)

$(SPI) : $(DIR_LIB)\$(SPI)
	copy $(DIR_LIB)\$(SPI) .\

OogPlugin.dll : $(OOG_HOME)\OogPlugin\OogPlugin.dll
	copy $(OOG_HOME)\OogPlugin\OogPlugin.dll .\

SpiUtility.dll : $(OOG_HOME)\Plugins\SpiWrapper\Util\SpiUtility.dll
	copy $(OOG_HOME)\Plugins\SpiWrapper\Util\SpiUtility.dll .\

SpiUtility_debug.dll : $(OOG_HOME)\Plugins\SpiWrapper\Util\SpiUtility.dll
	copy $(OOG_HOME)\Plugins\SpiWrapper\Util\SpiUtility_debug.* .\

clean :
	del *.dll
	del *.pdb
	del *.spi
