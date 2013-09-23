import xml.etree.ElementTree as ET
from datetime import datetime

def generateVersionInfo(template, version, file_length, outfile):
	
	ET.register_namespace("sparkle", "http://www.andymatuschak.org/xml-namespaces/sparkle")
	ET.register_namespace("dc", "http://purl.org/dc/elements/1.1/")

	tree = ET.parse(template)

	pubdate = tree.find(".//pubDate")
	if (pubdate != None):
		pubdate.text = datetime.now().strftime("%Y-%m-%d %H:%M:%S %z")
		print 'publish date => ' + pubdate.text

	enclosure = tree.find(".//enclosure")
	if (enclosure != None):
		#for item in enclosure.keys():
		#	print item
		enclosure.set('length', "{0}".format(file_length))
		enclosure.set('{http://www.andymatuschak.org/xml-namespaces/sparkle}version', version)
		print "file_length => " + enclosure.get('length')
		print "version => " + enclosure.get('{http://www.andymatuschak.org/xml-namespaces/sparkle}version')


	tree.write(outfile, encoding="utf-8")
