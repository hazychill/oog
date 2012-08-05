@echo off

setlocal

set NMAKE=nmake

pushd .
cd Core\build
%NMAKE%
copy *.exe ..\..\
copy *.dll ..\..\
copy *.pdb ..\..\
popd

pushd .
cd Plugins\Zip\build
%NMAKE%
popd .

pushd .
cd Plugins\Lha\build
%NMAKE%
popd .

pushd .
cd Plugins\Rar\build
%NMAKE%
popd .

pushd .
cd Plugins\XacRett\build
%NMAKE%
popd .

pushd .
cd Plugins\Text\build
%NMAKE%
popd .

pushd .
cd Plugins\SpiWrapper\Util\build
%NMAKE%
popd .

pushd .
cd Plugins\SpiWrapper\Tlg\build
%NMAKE%
popd .

pushd .
cd Plugins\SpiWrapper\Xp3\build
%NMAKE%
popd .

endlocal

pause
