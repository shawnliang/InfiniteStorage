import generateVersionInfo as gvi;
import s3_upload as s3

from os import listdir
from os.path import isfile, join
import os.path
import re
import sys


def parse_file_name(file_name):
	m = re.search(r'(\d+\.\d+\.\d+\.\d+)', file_name)
	return { "path" : file_name, "version" : m.group(1), "size" : os.path.getsize(file_name) }



# file path of infinite storage installer, eg, C:\InfiniteStorage\production-FavoriteHome-1.1.0.2342.exe
file_name = sys.argv[1]
# folder path of InfiniteStorage source code, eg, C:\InfiniteStorage
folder = sys.argv[2]	


file = parse_file_name(file_name)
print file['path']
print file['version']
print file['size']
version_file = join(folder, "Tools/versioninfo.xml")

gvi.generateVersionInfo(version_file, file['version'], file['size'], version_file)

s3.upload_s3(file['path'], 'FavoriteHomeInstaller.exe')
s3.upload_s3(version_file, 'versioninfo.xml')
s3.invalidate_cdn('FavoriteHomeInstaller.exe')
s3.invalidate_cdn('versioninfo.xml')
