import boto
import sys
import os


def upload_s3(source, target):
    s3 = boto.connect_s3("AKIAIIBDUMDENJPEGSDA", "UA082SmNXWe7HL1kmLNRCJGxJMq5KAU/UffaNz1Z")
    bucket = s3.get_bucket('wammer-station')
      
    key = bucket.new_key('WindowsStation/' + target)
    key.set_contents_from_filename(source)
    key.set_acl('public-read')



if __name__ == "__main__":

	source_name = sys.argv[1]
	target_name = sys.argv[2]

    #print "Upload {0} to S3 as {1}".format(source_name, target_name)
	#upload_s3(source_name, target_name)