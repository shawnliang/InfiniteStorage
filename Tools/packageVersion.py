import sys
import re
from tempfile import mkstemp
from shutil import move
from os import remove, close


def find_and_replace(target, buildNum):
    buildNum = '<?define AppVersionBuild = "{0}" ?>'.format(buildNum)
    pattern = re.compile('\<\?define AppVersionBuild = "0" \?\>')

    fh, abs_path = mkstemp()
    new_file = open(abs_path, 'w')
    old_file = open(target)

    for line in old_file:
        new_file.write(pattern.sub(buildNum, line))
    new_file.close()
    close(fh)
    old_file.close()
    remove(target)
    move(abs_path, target)

if __name__ == "__main__":
    print "[Waveface] Replace build number in {0} to {1}".format(sys.argv[1], sys.argv[2])
    find_and_replace(sys.argv[1], sys.argv[2])
    print "[Waveface] Package build number replacement done."
