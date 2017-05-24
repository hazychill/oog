OOG_HOME = ..\..\..\..

CSC40 = "%windir%\Microsoft.NET\Framework\v4.0.30319\csc.exe"
CSC = $(CSC40)
CSC64bit = "%windir%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"

DIR_SRC = ..\src
DIR_LIB = ..\lib

SRCS = \
	$(DIR_SRC)\TlgImageCreator.cs \

REFS_OOG = \
	/reference:OogPlugin.dll \
	/reference:SpiUtility.dll \

REFS_OOG_64 = \
	/reference:OogPlugin64.dll \
	/reference:SpiUtility64.dll \

REFS_OOG_DEBUG = \
	/reference:OogPlugin.dll \
	/reference:SpiUtility_debug.dll \

REFS_OOG_DEBUG_64 = \
	/reference:OogPlugin64.dll \
	/reference:SpiUtility64_debug.dll \

REFS_CLR = \

REFS_CLR_64 = \

REFS = \
	$(REFS_OOG) \
	$(REFS_CLR) \

REFS_64 = \
	$(REFS_OOG_64) \
	$(REFS_CLR_64) \

REFS_DEBUG = \
	$(REFS_OOG_DEBUG) \
	$(REFS_CLR) \

REFS_DEBUG_64 = \
	$(REFS_OOG_DEBUG_64) \
	$(REFS_CLR_64) \

SPI = if_tlg.spi

release : OogTlg.dll
debug : OogTlg_debug.dll
dotnet64 : OogTlg64.dll

OogTlg.dll : $(SRCS) $(REFS_OOG:/reference:=) $(SPI)
	$(CSC) /target:library /out:$@ /unsafe $(REFS) $(SRCS)

OogTlg_debug.dll : $(SRCS) $(REFS_OOG_DEBUG:/reference:=)  $(SPI)
	$(CSC) /target:library /out:$@ /unsafe $(REFS_DEBUG) /debug /define:DEBUG $(SRCS)

OogTlg64.dll : $(SRCS) $(REFS_OOG_64:/reference:=) $(SPI)
	$(CSC64bit) /target:library /out:$@ /unsafe $(REFS_64) $(SRCS)

$(SPI) : $(DIR_LIB)\$(SPI)
	copy $(DIR_LIB)\$(SPI) .\

OogPlugin.dll : $(OOG_HOME)\OogPlugin\OogPlugin.dll
	copy $(OOG_HOME)\OogPlugin\OogPlugin.dll .\

OogPlugin64.dll : $(OOG_HOME)\OogPlugin\OogPlugin64.dll
	copy $(OOG_HOME)\OogPlugin\OogPlugin64.dll .\

SpiUtility.dll : $(OOG_HOME)\Plugins\SpiWrapper\Util\SpiUtility.dll
	copy $(OOG_HOME)\Plugins\SpiWrapper\Util\SpiUtility.dll .\

SpiUtility64.dll : $(OOG_HOME)\Plugins\SpiWrapper\Util\SpiUtility64.dll
	copy $(OOG_HOME)\Plugins\SpiWrapper\Util\SpiUtility64.dll .\

SpiUtility_debug.dll : $(OOG_HOME)\Plugins\SpiWrapper\Util\SpiUtility.dll
	copy $(OOG_HOME)\Plugins\SpiWrapper\Util\SpiUtility_debug.* .\

SpiUtility64_debug.dll : $(OOG_HOME)\Plugins\SpiWrapper\Util\SpiUtility64.dll
	copy $(OOG_HOME)\Plugins\SpiWrapper\Util\SpiUtility64_debug.* .\

clean :
	del *.dll
	del *.pdb
	del *.spi
