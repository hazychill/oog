CSC35 = "%windir%\Microsoft.NET\Framework\v3.5\csc.exe"
CSC = $(CSC35)

DIR_SRC = ..\src
DIR_SRC_DEFAULTPLUGIN = $(DIR_SRC)\DefaultPlugin
DIR_SRC_IMAGING = $(DIR_SRC)\Imaging
DIR_SRC_THUMBNAILVIEWER = $(DIR_SRC)\ThumbnailViewer
DIR_SRC_TREEVIEW = $(DIR_SRC)\TreeView
DIR_SRC_VIEWER = $(DIR_SRC)\Viewer

SRC_MAIN = \
	$(DIR_SRC)\AssemblyInfo.cs \
	$(DIR_SRC)\SettingsForm.cs \
	$(DIR_SRC)\InterpolationModeConverter.cs \
	$(DIR_SRC)\MainForm.cs \
	$(DIR_SRC)\MainForm.Designer.cs \
	$(DIR_SRC)\OogSettings.cs \
	$(DIR_SRC)\PercentProgressBar.cs \
	$(DIR_SRC)\program.cs \
	$(DIR_SRC)\AddJumpForm.cs \

SRC_DEFAULTPLUGIN = \
	$(DIR_SRC_DEFAULTPLUGIN)\DefaultImageCreator.cs \
	$(DIR_SRC_DEFAULTPLUGIN)\FolderExtractor.cs \
	$(DIR_SRC_DEFAULTPLUGIN)\FolderExtractorFactory.cs \

SRC_IMAGING = \
	$(DIR_SRC_IMAGING)\ImageResizer.cs \
	$(DIR_SRC_IMAGING)\ResizerConverter.cs \

SRC_THUMBNAILVIEWER = \
	$(DIR_SRC_THUMBNAILVIEWER)\ThumbnailViewer.cs \
	$(DIR_SRC_THUMBNAILVIEWER)\ThumbnailSetting.cs \
	$(DIR_SRC_THUMBNAILVIEWER)\Thumbnail.cs \

SRC_TREEVIEW = \
	$(DIR_SRC_TREEVIEW)\DirectoryTreeView.cs \
	$(DIR_SRC_TREEVIEW)\OogTreeNode.cs \
	$(DIR_SRC_TREEVIEW)\Comparers.cs \

SRC_VIEWER = \
	$(DIR_SRC_VIEWER)\FullScreenViewerSettings.cs \
	$(DIR_SRC_VIEWER)\FullScreenViewer.designer.cs \
	$(DIR_SRC_VIEWER)\FullScreenViewer.Events.cs \
	$(DIR_SRC_VIEWER)\FullScreenViewer.Imaging.cs \
	$(DIR_SRC_VIEWER)\FullScreenViewer.cs \

SRCS = \
	$(SRC_MAIN) \
	$(SRC_DEFAULTPLUGIN) \
	$(SRC_IMAGING) \
	$(SRC_THUMBNAILVIEWER) \
	$(SRC_TREEVIEW) \
	$(SRC_VIEWER) \

RESOURCES = \
	/resource:$(DIR_SRC)\Oog.MainForm.resources \
	/resource:$(DIR_SRC_DEFAULTPLUGIN)\Oog.Plugin.FolderExtractorFactory.resources \
	/resource:$(DIR_SRC_TREEVIEW)\Oog.DirectoryTreeView.resources \


OOG_PLUGIN = .\OogPlugin.dll

ICON = ..\data\oog.ico

REFS_OOG = \
	/reference:$(OOG_PLUGIN) \

REFS_CLR = \

REFS = \
	$(REFS_OOG) \
	$(REFS_CLR) \

release : Oog.exe
debug : Oog_debug.exe

Oog.exe : $(SRCS) $(RESOURCES:/resource:=) $(REFS_OOG:/reference:=) $(ICON)
	$(CSC) /target:winexe /out:$@ /win32icon:$(ICON) $(REFS) $(RESOURCES) $(SRCS)

Oog_debug.exe : $(SRCS) $(RESOURCES:/resource:=) $(REFS_OOG:/reference:=) $(ICON)
	$(CSC) /target:winexe /out:$@ /win32icon:$(ICON) $(REFS) $(RESOURCES) /debug /define:DEBUG $(SRCS)

$(OOG_PLUGIN) : ..\..\OogPlugin\OogPlugin.dll
	copy ..\..\OogPlugin\OogPlugin.dll .\

clean :
	del *.exe
	del *.dll
	del *.pdb
