pushd .
cd Core\build
nmake
copy *.exe ..\..\
copy *.dll ..\..\
copy *.pdb ..\..\
popd

pushd .
cd Plugins\Zip\build
nmake
popd .

pushd .
cd Plugins\Lha\build
nmake
popd .

pushd .
cd Plugins\Rar\build
nmake
popd .

pushd .
cd Plugins\SpiWrapper\Util\build
nmake
popd .

pushd .
cd Plugins\SpiWrapper\Tlg\build
nmake
popd .

pushd .
cd Plugins\SpiWrapper\Xp3\build
nmake
popd .

