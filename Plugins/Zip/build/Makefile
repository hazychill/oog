OOG_HOME = ..\..\..

DIR_SRC = ..\src
DIR_SRC_SZL = ..\src\SharpZipLib
DIR_LIB = ..\lib

SRCS_OOG = \
	$(DIR_SRC)\EmptyExtractor.cs \
	$(DIR_SRC)\ZipExtractor.cs \
	$(DIR_SRC)\ZipExtractorFactory.cs \
	$(DIR_SRC)\ZipImageCreator.cs \

SRCS_SZL = \
	$(DIR_SRC_SZL)\BZip2\BZip2.cs \
	$(DIR_SRC_SZL)\BZip2\BZip2Constants.cs \
	$(DIR_SRC_SZL)\BZip2\BZip2Exception.cs \
	$(DIR_SRC_SZL)\BZip2\BZip2InputStream.cs \
	$(DIR_SRC_SZL)\BZip2\BZip2OutputStream.cs \
	$(DIR_SRC_SZL)\Checksums\Adler32.cs \
	$(DIR_SRC_SZL)\Checksums\CRC32.cs \
	$(DIR_SRC_SZL)\Checksums\IChecksum.cs \
	$(DIR_SRC_SZL)\Checksums\StrangeCRC.cs \
	$(DIR_SRC_SZL)\Core\FileSystemScanner.cs \
	$(DIR_SRC_SZL)\Core\INameTransform.cs \
	$(DIR_SRC_SZL)\Core\IScanFilter.cs \
	$(DIR_SRC_SZL)\Core\NameFilter.cs \
	$(DIR_SRC_SZL)\Core\PathFilter.cs \
	$(DIR_SRC_SZL)\Core\StreamUtils.cs \
	$(DIR_SRC_SZL)\Encryption\PkzipClassic.cs \
	$(DIR_SRC_SZL)\GZip\GZIPConstants.cs \
	$(DIR_SRC_SZL)\GZip\GZipException.cs \
	$(DIR_SRC_SZL)\GZip\GzipInputStream.cs \
	$(DIR_SRC_SZL)\GZip\GzipOutputStream.cs \
	$(DIR_SRC_SZL)\SharpZipBaseException.cs \
	$(DIR_SRC_SZL)\Tar\InvalidHeaderException.cs \
	$(DIR_SRC_SZL)\Tar\TarArchive.cs \
	$(DIR_SRC_SZL)\Tar\TarBuffer.cs \
	$(DIR_SRC_SZL)\Tar\TarEntry.cs \
	$(DIR_SRC_SZL)\Tar\TarException.cs \
	$(DIR_SRC_SZL)\Tar\TarHeader.cs \
	$(DIR_SRC_SZL)\Tar\TarInputStream.cs \
	$(DIR_SRC_SZL)\Tar\TarOutputStream.cs \
	$(DIR_SRC_SZL)\Zip\Compression\Deflater.cs \
	$(DIR_SRC_SZL)\Zip\Compression\DeflaterConstants.cs \
	$(DIR_SRC_SZL)\Zip\Compression\DeflaterEngine.cs \
	$(DIR_SRC_SZL)\Zip\Compression\DeflaterHuffman.cs \
	$(DIR_SRC_SZL)\Zip\Compression\DeflaterPending.cs \
	$(DIR_SRC_SZL)\Zip\Compression\Inflater.cs \
	$(DIR_SRC_SZL)\Zip\Compression\InflaterDynHeader.cs \
	$(DIR_SRC_SZL)\Zip\Compression\InflaterHuffmanTree.cs \
	$(DIR_SRC_SZL)\Zip\Compression\PendingBuffer.cs \
	$(DIR_SRC_SZL)\Zip\Compression\Streams\DeflaterOutputStream.cs \
	$(DIR_SRC_SZL)\Zip\Compression\Streams\InflaterInputStream.cs \
	$(DIR_SRC_SZL)\Zip\Compression\Streams\OutputWindow.cs \
	$(DIR_SRC_SZL)\Zip\Compression\Streams\StreamManipulator.cs \
	$(DIR_SRC_SZL)\Zip\FastZip.cs \
	$(DIR_SRC_SZL)\Zip\IEntryFactory.cs \
	$(DIR_SRC_SZL)\Zip\ZipConstants.cs \
	$(DIR_SRC_SZL)\Zip\ZipEntry.cs \
	$(DIR_SRC_SZL)\Zip\ZipEntryFactory.cs \
	$(DIR_SRC_SZL)\Zip\ZipException.cs \
	$(DIR_SRC_SZL)\Zip\ZipExtraData.cs \
	$(DIR_SRC_SZL)\Zip\ZipFile.cs \
	$(DIR_SRC_SZL)\Zip\ZipHelperStream.cs \
	$(DIR_SRC_SZL)\Zip\ZipInputStream.cs \
	$(DIR_SRC_SZL)\Zip\ZipNameTransform.cs \
	$(DIR_SRC_SZL)\Zip\ZipOutputStream.cs \

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
	csc /target:library /out:$@ $(REFS) $(RESOURCES) $(SRCS)

OogZip_debug.dll : $(SRCS) $(RESOURCES:/resource:=) $(REFS_OOG:/reference:=)
	csc /target:library /out:$@ $(REFS) $(RESOURCES) /debug /define:DEBUG $(SRCS)

OogPlugin.dll : $(OOG_HOME)\OogPlugin\OogPlugin.dll
	copy $** .\

clean :
	del *.dll
	del *.pdb
