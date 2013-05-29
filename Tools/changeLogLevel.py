import os
import sys
import re
from tempfile import mkstemp
from shutil import move
from os import remove, close

LOGLEVEL_PATTERN = re.compile('<level\s+value="DEBUG"\s+/>')


def find_and_replace(target, loglevel, pattern):
    newstring = '<level value="{0}" />'.format(loglevel)

    fh, abs_path = mkstemp()
    new_file = open(abs_path, 'w')
    old_file = open(target)

    for line in old_file:
        new_line = pattern.sub(newstring, line)
        new_file.write(new_line)
    new_file.close()
    close(fh)
    old_file.close()
    remove(target)
    move(abs_path, target)


def dir_traverse(dest, loglevel):
    for dirname, dirnames, filenames in os.walk(dest):
        for filename in filenames:
            if (filename.lower() == 'app.config'):
                target = os.path.join(dirname, filename)
                find_and_replace(target, loglevel, LOGLEVEL_PATTERN)


if __name__ == "__main__":
    dest = sys.argv[1]
    loglevel = sys.argv[2]
    print "[Waveface] Change log level in {0} to {1}".format(dest, loglevel)
    dir_traverse(dest, loglevel)
    print "[Waveface] Change log level done."
