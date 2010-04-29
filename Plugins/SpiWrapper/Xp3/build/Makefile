OOG_HOME = ..\..\..\..

CSC40 = "%windir%\Microsoft.NET\Framework\v4.0.30319\csc.exe"
CSC = $(CSC40)

DIR_SRC = ..\src
DIR_LIB = ..\lib

SRCS = \
	$(DIR_SRC)\Xp3Extractor.cs \
	$(DIR_SRC)\Xp3ExtractorFactory.cs \

RESOURCES = \
	/resource:$(DIR_SRC)\OogXp3.Xp3ExtractorFactory.resources \

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

SPI = ax_xp3.spi

release : OogXp3.dll
debug : OogXp3_debug.dll

OogXp3.dll : $(SRCS) $(RESOURCES:/resource:=) $(REFS_OOG:/reference:=) $(SPI)
	$(CSC) /target:library /out:$@ /unsafe $(REFS) $(RESOURCES) $(SRCS)

OogXp3_debug.dll : $(SRCS) $(RESOURCES:/resource:=) $(REFS_OOG_DEBUG:/reference:=)  $(SPI)
	$(CSC) /target:library /out:$@ /unsafe $(REFS_DEBUG) $(RESOURCES) /debug /define:DEBUG $(SRCS)

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
