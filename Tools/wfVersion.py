import os
import sys
import re
import buildVersion
import packageVersion
import changeLogLevel
import shutil
import os.path

def get_product_version_from_wix_config_file(solution_dir) :
	file_path = os.path.join(solution_dir, r"Setup\Package\Product.wxs")
	config_file = open(file_path, 'r')
	
	major_pattern = re.compile(r'<?define\s+AppVersionMajor')
	minor_pattern = re.compile(r'<?define\s+AppVersionMinor')
	patch_pattern = re.compile(r'<?define\s+AppVersionPatch')
	
	major = ""
	minor = ""
	patch = ""
	
	for line in config_file:
		if major_pattern.search(line) :
			major = re.search(r'\d+', line).group(0)
		if minor_pattern.search(line) :
			minor = re.search(r'\d+', line).group(0)
		if patch_pattern.search(line) :
			patch = re.search(r'\d+', line).group(0)
	
	config_file.close()
	
	if major == "" or minor == "" or patch == "" :
		raise Exception("Cannot extract product version from " + file_path)
	
	return r"{0}.{1}.{2}".format(major, minor, patch)

	
if __name__ == "__main__":
	version = get_product_version_from_wix_config_file(sys.argv[1])
	print version
