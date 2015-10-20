#!/bin/bash

DIR=$(dirname "$0")
VERSION=$(git describe --abbrev=0 --tags)
REVISION=$(git log "$VERSION..HEAD" --oneline | wc -l)

function update_ai {
	f="$1"
	lead='^\/\/ TRAVIS\-CI: START REMOVE$'
	tail='^\/\/ TRAVIS\-CI: END REMOVE$'
	C=$(sed -e "/$lead/,/$tail/{ /$lead/{p; r insert_file
        }; /$tail/p; d }" $f/Properties/AssemblyInfo.cs)
	echo "$C" > $f/Properties/AssemblyInfo.cs
	echo "[assembly: AssemblyVersion(\"$VERSION_STR.$REVISION\")]" >> $f/Properties/AssemblyInfo.cs
	echo "[assembly: AssemblyFileVersion(\"$VERSION_STR.$REVISION\")]" >> $f/Properties/AssemblyInfo.cs
	
	for nuspec in $f/../*.nuspec; do
		if [[ -f "$nuspec" ]]; then
			echo "Processing nuspec file: $nuspec"
			if [[ $VERSION_STR =~ "^([1-9])" ]]; then
				sed -i.bak "s/\\\$version\\\$/$VERSION_STR/g" $nuspec
			else
				padded=$(printf "%04d" $REVISION)
				sed -i.bak "s/\\\$version\\\$/$VERSION_STR-cibuild$padded/g" $nuspec
			fi
		fi
	done
}

re="([0-9]+\.[0-9]+\.[0-9]+)"
if [[ $VERSION =~ $re ]]; then
	VERSION_STR="${BASH_REMATCH[1]}"
	echo "Version of $1 is now: $VERSION_STR"
	update_ai $DIR/../$1
fi