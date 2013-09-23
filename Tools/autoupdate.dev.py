import generateVersionInfo as gvi;
import s3_upload as s3

from os import listdir
from os.path import isfile, join
import os.path
import re
import sys

def getmtime(path):
	return os.path.getmtime(path)

def getLatestVersion(folder):
	files = [ f for f in listdir(folder) if isfile(join(folder, f)) ]
	dev_files = [ join(folder, f) for f in files if f.startswith("development-FavoriteHome-") ]
	latest_file = max(dev_files, key = getmtime)

	m = re.search(r'(\d+\.\d+\.\d+\.\d+)', latest_file)

	return { "path" : latest_file, "version" : m.group(1), "size" : os.path.getsize(latest_file) }



folder = sys.argv[1]


file = getLatestVersion(folder)
print file['path']
print file['version']
print file['size']
version_file = join(folder, "Tools/versioninfo.dev.xml")

gvi.generateVersionInfo(version_file, file['version'], file['size'], version_file)

s3.upload_s3(file['path'], 'PiaryPhotos.dev.exe')
s3.upload_s3(version_file, 'versioninfo.dev.xml')
s3.invalidate_cdn('PiaryPhotos.dev.exe')
s3.invalidate_cdn('versioninfo.dev.xml')
