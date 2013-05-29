# -*- coding: utf-8 -*-

import os
import sys
import re
from tempfile import mkstemp
from shutil import move
from os import remove, close

CURRENT_COPYRIGHT = 'Copyright © 2011-2012 Waveface Inc.'
PRODUCT_NAME = 'Waveface FavoriteHome'

VER_PATTERN = re.compile('(?<=AssemblyVersion\(").*?(?="\)\])')
FILE_VER_PATTERN = re.compile('(?<=AssemblyFileVersion\(").*?(?="\)\])')
COPYRIGHT_PATTERN = re.compile('(?<=AssemblyCopyright\(").*?(?="\)\])')
PRODUCT_PATTERN = re.compile('(?<=AssemblyProduct\(").*?(?="\)\])')

def find_and_replace(target, version, pattern):

    fh, abs_path = mkstemp()
    new_file = open(abs_path, 'w')
    old_file = open(target)

    for line in old_file:
        new_line = pattern.sub(version, line)
        new_file.write(new_line)
    new_file.close()
    close(fh)
    old_file.close()
    remove(target)
    move(abs_path, target)


def dir_traverse(dest, version):
    for dirname, dirnames, filenames in os.walk(dest):
        for filename in filenames:
            if (filename == 'AssemblyInfo.cs'):
                target = os.path.join(dirname, filename)
                find_and_replace(target, version, VER_PATTERN)
                find_and_replace(target, version, FILE_VER_PATTERN)
                find_and_replace(target, CURRENT_COPYRIGHT, COPYRIGHT_PATTERN)
                find_and_replace(target, PRODUCT_NAME, PRODUCT_PATTERN)

if __name__ == "__main__":
    print "[Waveface] Replace version in {0} to {1}".format(sys.argv[1], sys.argv[2])
    print "[Waveface] Replace copyright to {0}".format(CURRENT_COPYRIGHT)
    dir_traverse(sys.argv[1], sys.argv[2])
    print "[Waveface] Version replacement done."
