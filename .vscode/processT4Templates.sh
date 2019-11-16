#!/bin/bash
# buildEffects
# Compiles all .fx files found in the project's Content directory.
# Intended for usage with VS Code Build Tasks tooling.
# You may need to change the path to fxc.exe depending on your installation.

printf "Starting T4 processing...\n"
_root="T4"

echo Rot.Game Rot.Data | tr ' ' '\n' |
while read _dir; do
    pushd "${_dir}"

    mkdir -p "${_root}"/Output

    for file in `find ./"${_root}"/** -name "*.tt"` ;
    do
        t4 -r=System.dll -r=mscorlib.dll -r=netstandard.dll -r=System.IO.FileSystem.dll \
            -r=System.Collections.dll -r=System.Core.dll -r=netstandard.dll \
            -r=System.Linq.dll -r=System.Text.RegularExpressions.dll `dirname $file`/`basename $file` \
            -o `dirname $file`/Output/`basename $file .tt`.cs

        echo "Built `basename $file`"
    done

    popd > /dev/null
done
