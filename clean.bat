@echo off

setlocal

set NMAKE=nmake

pushd .
cd Core\build
%NMAKE% clean
popd

pushd .
cd Plugins\Zip\build
%NMAKE% clean
popd .

pushd .
cd Plugins\Lha\build
%NMAKE% clean
popd .

pushd .
cd Plugins\Rar\build
%NMAKE% clean
popd .

pushd .
cd Plugins\XacRett\build
%NMAKE% clean
popd .

pushd .
cd Plugins\Text\build
%NMAKE% clean
popd .

pushd .
cd Plugins\SpiWrapper\Util\build
%NMAKE% clean
popd .

pushd .
cd Plugins\SpiWrapper\Tlg\build
%NMAKE% clean
popd .

pushd .
cd Plugins\SpiWrapper\Xp3\build
%NMAKE% clean
popd .

endlocal