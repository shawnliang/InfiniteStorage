import boto
import sys
import os

IAM_ACCESS = "AKIAIIBDUMDENJPEGSDA"
IAM_SECRET = "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"

S3_BUCKET = 'wammer-station'

# Cloudfront Distribution ID (cdn.waveface.com) : 
CDN_DIST_ID = "E2G2EKW1P9VL2N"


def mk_uri(target):
    """
    Build canonical target uri for AWS s3 / cloudfront
    """
    return "WindowsStation/%s" % target

def upload_s3(source, target):
    """
    Upload source to s3
    Returns True on success, False otherwise (not implemented, probably not needed)
    """
    s3 = boto.connect_s3(IAM_ACCESS, IAM_SECRET)
    bucket = s3.get_bucket(S3_BUCKET)
      
    key = bucket.new_key(mk_uri(target))
    key.set_contents_from_filename(source, replace = True)
    key.set_acl('public-read')
    
    return True

def invalidate_cdn(targets):
    """
    Invalidate cloudfront cache(s)
    """
    if not type(targets) is list:
        targets = list((targets,))
        
    conn = boto.connect_cloudfront(IAM_ACCESS, IAM_SECRET)
    conn.create_invalidation_request(CDN_DIST_ID, [mk_uri(t) for t in list(targets)])
    
    return True

if __name__ == "__main__":

    source_name = sys.argv[1]
    target_name = sys.argv[2]

    #print "Upload {0} to S3 as {1}".format(source_name, target_name)

    # cascade task chain
    upload_s3(source_name, target_name) or \
    invalidate_cdn(target_name)
