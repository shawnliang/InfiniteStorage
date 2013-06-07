using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Wpf_testHTTP
{
  
    #region Entities

    public class General_R
    {
        public string status { get; set; }
        public string session_token { get; set; }
        public string session_expires { get; set; }
        public string timestamp { get; set; }
        public string api_ret_code { get; set; }
        public string api_ret_message { get; set; }
    }

    public class Post
    {
        public string post_id { get; set; }
        public string creator_id { get; set; }
        public string code_name { get; set; }
        public string device_id { get; set; }
        public string group_id { get; set; }
        public string timestamp { get; set; }
        public string content { get; set; }
        public string type { get; set; }
        public string status { get; set; }

        public bool hidden { get; set; }

        public int comment_count { get; set; }
        public List<Comment> comments { get; set; }

        public int attachment_count { get; set; }
        public List<Attachment> attachments { get; set; }
        public List<string> attachment_id_array { get; set; }

        public Preview_OpenGraph preview { get; set; }
        public string soul { get; set; }

        public string update_time { get; set; }
        public string event_time { get; set; }
        public string cover_attach { get; set; }
        public string favorite { get; set; }
        public int seq_num { get; set; }

        //[JsonIgnore]
        public Dictionary<string, string> Sources { get; set; }

        public Post()
        {
            Sources = new Dictionary<string, string>();
        }

        public string getCoverImageId()
        {
            if (!string.IsNullOrEmpty(cover_attach))
                return cover_attach;
            else if (attachment_id_array != null && attachment_id_array.Count > 0)
                return attachment_id_array[0];
            else
                return null;
        }
    }

    public class Attachment
    {
        public string group_id { get; set; }
        public string description { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string file_name { get; set; }
        public string image { get; set; }
        public string object_id { get; set; }
        public string creator_id { get; set; }
        public string modify_time { get; set; }
        public string code_name { get; set; }
        public string file_size { get; set; }
        public string type { get; set; }
        public string device_id { get; set; }
        public string mime_type { get; set; }
        public string md5 { get; set; }

        public ImageMeta image_meta { get; set; }
    }

    public class ImageMeta
    {
        public ImageMetaItem small { get; set; }
        public ImageMetaItem medium { get; set; }
        public ImageMetaItem large { get; set; }
        public ImageMetaItem square { get; set; }
    }

    public class ImageMetaItem
    {
        public string url { get; set; }
        public string file_name { get; set; }
        public string height { get; set; }
        public string width { get; set; }
        public string modify_time { get; set; }
        public string file_size { get; set; }
        public string mime_type { get; set; }
        public string md5 { get; set; }
    }

    public class Comment
    {
        public string comment_id { get; set; }
        public string creator_id { get; set; }
        public string post_id { get; set; }
        public string code_name { get; set; }
        public string timestamp { get; set; }
        public string content { get; set; }
    }

    public class Device
    {
        public string device_id { get; set; }
        public string last_visit { get; set; }
        public string device_type { get; set; }
        public string device_name { get; set; }
    }

    public class SNS1
    {
        public bool enabled { get; set; }
        public string snsid { get; set; }
        public string status { get; set; }
        public string type { get; set; }
        public string lastSync { get; set; }
        public string toDate { get; set; }
    }

    public class SNS2
    {
        public bool enabled { get; set; }
        public string snsid { get; set; }
        public List<string> status { get; set; }
        public string type { get; set; }
        public string lastSync { get; set; }
        public string toDate { get; set; }
    }

    public class User
    {
        public string user_id { get; set; }
        public bool subscribed { get; set; }
        public int since { get; set; }
        public List<Device> devices { get; set; }
        public string state { get; set; }
        public string avatar_url { get; set; }
        public List<SNS2> sns { get; set; }
        public bool verified { get; set; }
        public string nickname { get; set; }
        public string email { get; set; }
    }

    public class Group
    {
        public string group_id { get; set; }
        public string creator_id { get; set; }
        public string station_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class WFStorageUsage
    {
        public long dropbox_objects { get; set; }
        public long origin_sizes { get; set; }
        public long total_objects { get; set; }
        public long month_total_objects { get; set; }
        public long origin_files { get; set; }
        public long month_doc_objects { get; set; }
        public long doc_objects { get; set; }
        public long meta_files { get; set; }
        public long meta_sizes { get; set; }
        public long total_files { get; set; }
        public long month_image_objects { get; set; }
        public long image_objects { get; set; }
        public long total_sizes { get; set; }
    }

    public class WFStorageAvailable
    {
        public long avail_month_image_objects { get; set; }
        public long avail_month_total_objects { get; set; }
        public long avail_month_doc_objects { get; set; }
    }

    public class WFStorageQuota
    {
        public long dropbox_objects { get; set; }
        public long origin_sizes { get; set; }
        public long total_objects { get; set; }
        public long month_total_objects { get; set; }
        public long origin_files { get; set; }
        public long month_doc_objects { get; set; }
        public long meta_files { get; set; }
        public long meta_sizes { get; set; }
        public long total_files { get; set; }
        public long month_image_objects { get; set; }
        public long image_objects { get; set; }
        public long total_sizes { get; set; }
    }

    public class QuotaInterval
    {
        public long quota_interval_end { get; set; }
        public long quota_interval_begin { get; set; }
        public int quota_interval_left_days { get; set; }
    }

    public class WFStorage
    {
        public WFStorageUsage usage;
        public WFStorageAvailable available;
        public WFStorageQuota quota;
        public QuotaInterval interval;
        public bool over_quota;
    }

    public class Storages
    {
        public WFStorage waveface;
    }

    public class DiskUsage
    {
        public long avail { get; set; }
        public string group_id { get; set; }
        public long used { get; set; }
    }

    public class Station
    {
        public string station_id { get; set; }
        public string creator_id { get; set; }
        public string timestamp { get; set; }
        public string last_seen { get; set; }
        public string location { get; set; }
        public List<DiskUsage> diskusage { get; set; }
        public string status { get; set; }
        public string accessible { get; set; }
        public int last_ping { get; set; }
        public string public_location { get; set; }
        public string version { get; set; }
        public string computer_name { get; set; }
        public bool upnp { get; set; }
        public string type { get; set; }
    }

    public class CloudStorage
    {
        public string type { get; set; }
        public bool connected { get; set; }
        public long quota { get; set; }
        public long used { get; set; }
        public string account { get; set; }
    }

    public class StationDetail
    {
        public string location { get; set; }
        public List<DiskUsage> diskusage { get; set; }
        public UPnPInfo upnp { get; set; }
        public string computer_name { get; set; }
        public string version { get; set; }
    }

    public class UPnPInfo
    {
        public bool status { get; set; }
        public string public_addr { get; set; }
        public int public_port { get; set; }
    }

    public class Preview_OpenGraph
    {
        public string provider_display { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public string thumbnail_url { get; set; }
        public string type { get; set; }
        public string favicon_url { get; set; }

        public List<OGS_Image> images { get; set; }

        /*
        public string provider_url { get; set; }
        public string provider_name { get; set; }
        public string thumbnail_width { get; set; }
        public string thumbnail_height { get; set; }
        public string keywords { get; set; }
        public string images { get; set; }
        public string embeds { get; set; }
        public string safe { get; set; }
        public string author_name { get; set; }
        public string original_url { get; set; }
        public string place { get; set; }
        public string cache_age { get; set; }
        public string author_url { get; set; }
        */
    }

    public class Preview_AdvancedOpenGraph
    {
        public string provider_url { get; set; }
        public string description { get; set; }
        public string original_url { get; set; }
        public string url { get; set; }
        public List<OGS_Image> images { get; set; }
        public bool safe { get; set; }
        public string provider_display { get; set; }
        public string author_name { get; set; }
        public string content { get; set; }
        public string favicon_url { get; set; }

        public string author_url { get; set; }
        public string title { get; set; }
        public string provider_name { get; set; }
        public int cache_age { get; set; }
        public string type { get; set; }

        /*
        public OGS_Object @object { get; set; }
        public List<object> place { get; set; }
        public List<object> embeds { get; set; }
        public List<object> @event { get; set; }
        */
    }

    public class OGS_Image
    {
        public string url { get; set; }
        // public string width { get; set; }
        // public string height { get; set; }
    }

    public class Object_File
    {
        public string file_type { get; set; }
        public string file_name { get; set; }
        public string file_path { get; set; }
        public string mime_type { get; set; }
        public string file_size { get; set; }
    }

    public class Image_Meta
    {
    }

    public class Fetch_Filter
    {
        public string timestamp { get; set; }
        public string creator_id { get; set; }
        public string tag { get; set; }
        public string filter_id { get; set; }
        public string filter_name { get; set; }
        public JObject filter_entity { get; set; }
    }

    public class Stations
    {
        public string status { get; set; }
        public string timestamp { get; set; }
        public string station_id { get; set; }
        public string creator_id { get; set; }
        public string location { get; set; }
        public string last_seen { get; set; }
    }

    public class Apikey
    {
        public string apikey { get; set; }
        public string name { get; set; }
    }

    public class LastScan
    {
        public string timestamp { get; set; }
        public string user_id { get; set; }
        public string group_id { get; set; }
        public string post_id { get; set; }
    }

    public class LastRead
    {
        public string timestamp { get; set; }
        public string user_id { get; set; }
        public string group_id { get; set; }
        public string post_id { get; set; }

        public string attachment_id { get; set; }
        public string comment_timestamp { get; set; }
        public string text_position { get; set; }
    }

    public class LastReadInput
    {
        public string post_id { get; set; }
        public string attachment_id { get; set; }
        public string comment_timestamp { get; set; }
        public string text_position { get; set; }
    }

    #endregion

    #region MR_auth

    public class MR_auth_signup : General_R
    {
        public User user { get; set; }
    }

    public class MR_auth_login : General_R
    {
        public User user { get; set; }
        public Device device { get; set; }
        public List<Group> groups { get; set; }
        public List<Station> stations { get; set; }
        public Apikey apikey { get; set; }
    }

    public class MR_auth_logout
    {
        public string status { get; set; }
        public string timestamp { get; set; }
        public string api_ret_code { get; set; }
    }

    #endregion

    #region MR_user

    public class MR_users_get : General_R
    {
        public User user { get; set; }
        public List<Group> groups { get; set; }
        public List<Station> stations { get; set; }
        public List<SNS1> sns { get; set; }
        public Device device { get; set; }
        public Storages storages { get; set; }
    }


    public class MR_FB_Disconnect : General_R
    {
        public User user { get; set; }
    }

    public class MR_users_update : General_R
    {
        public User user { get; set; }
    }

    public class MR_users_passwd : General_R
    {
    }

    public class MR_users_findMyStation : General_R
    {
        public List<Group> groups { get; set; }
        public List<Station> stations { get; set; }
    }

    #endregion

    #region MR_groups

    public class MR_groups_create : General_R
    {
        public Group group { get; set; }
    }

    public class MR_groups_get : General_R
    {
        public Group group { get; set; }
        public List<User> active_members { get; set; }
    }

    public class MR_groups_update : General_R
    {
        public Group group { get; set; }
    }

    public class MR_groups_delete : General_R
    {
    }

    public class MR_groups_inviteUser : General_R
    {
    }

    public class MR_groups_kickUser : General_R
    {
    }

    #endregion

    #region MR_posts

    public class MR_posts_getSingle : General_R
    {
        public string group_id { get; set; }

        public Post post { get; set; }
    }

    public class MR_posts_update : General_R
    {
        public string group_id { get; set; }

        public Post post { get; set; }
    }

    public class MR_posts_get : General_R
    {
        public string group_id { get; set; }

        public int get_count { get; set; }
        public int remaining_count { get; set; }
        public int newer_count { get; set; }

        public List<Post> posts { get; set; }
    }

    public class MR_posts_getLatest : General_R
    {
        public string group_id { get; set; }

        public int get_count { get; set; }
        public int total_count { get; set; }

        public List<Post> posts { get; set; }
        //public List<User> users { get; set; }
    }

    public class MR_posts_new : General_R
    {
        public string group_id { get; set; }

        public Post post { get; set; }
    }

    #endregion

    #region MR_comments

    public class MR_posts_newComment : General_R
    {
        public string group_id { get; set; }
        public string post_id { get; set; }

        public int comment_count { get; set; }
        public string comment_id { get; set; }
        public List<Comment> comments { get; set; }
    }

    public class MR_posts_getComments : General_R
    {
        public string group_id { get; set; }
        public string post_id { get; set; }

        public int comment_count { get; set; }
        public List<Comment> comments { get; set; }
    }

    #endregion

    #region MR_previews

    public class MR_previews_get : General_R
    {
        public Preview_OpenGraph preview { get; set; }
    }

    public class MR_previews_get_adv : General_R
    {
        public Preview_AdvancedOpenGraph preview { get; set; }
    }

    #endregion

    #region MR_attachments

    public class MR_attachments_upload : General_R
    {
        public string object_id { get; set; }
    }

    public class MR_attachments_get : General_R
    {
        public string description { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string image { get; set; }
        public string modify_time { get; set; }
        public string object_id { get; set; }
        public string device_name { get; set; }
        public string creator_id { get; set; }

        public Image_Meta image_meta { get; set; }

        public string file_size { get; set; }
        public string type { get; set; }
        public string mime_type { get; set; }
    }

    public class MR_attachments_delete : General_R
    {
    }

    #endregion

    #region MR_footprints

    public class MR_footprints_LastScan : General_R
    {
        public LastScan last_scan { get; set; }
    }

    public class MR_footprints_LastRead : General_R
    {
        public int last_read_count { get; set; }
        public List<LastRead> last_reads { get; set; }
    }

    #endregion

    #region MR_searchfilters

    public class MR_fetchfilters_item : General_R
    {
        public Fetch_Filter fetch_filter { get; set; }
    }

    public class MR_fetchfilters_list : General_R
    {
        public List<Fetch_Filter> fetch_filters { get; set; }
        public int fetch_filter_count { get; set; }
    }

    #endregion

    #region MR_hide

    public class MR_posts_hide_ret : General_R
    {
        public string post_id { get; set; }
    }

    #endregion

    #region MR_storages

    public class MR_storages_usage : General_R
    {
        public Storages storages { get; set; }
    }

    #endregion

    #region MR_cloudstorage

    public class MR_cloudstorage_list : General_R
    {
        public List<CloudStorage> cloudstorages { get; set; }
    }

    public class MR_cloudstorage_dropbox_oauth : General_R
    {
        public string oauth_url { get; set; }
    }

    public class MR_cloudstorage_dropbox_connect : General_R
    {
    }

    public class MR_cloudstorage_dropbox_update : General_R
    {
    }

    public class MR_cloudstorage_dropbox_disconnect : General_R
    {
    }

    #endregion

    #region MR_station

    public class MR_station_status : General_R
    {
        public StationDetail station_status { get; set; }
    }

    #endregion

    #region MR_usertracks

    public class UT_Action
    {
        public string action { get; set; }
        public string target_type { get; set; }
        public string post_id { get; set; }
    }

    public class UT_UsertrackList
    {
        public string group_id { get; set; }
        public string user_id { get; set; }
        public string timestamp { get; set; }
        public string target_id { get; set; }
        public string target_type { get; set; }
        public List<UT_Action> actions { get; set; }
    }

    public class MR_usertracks_get : General_R
    {
        public int get_count { get; set; }
        public string group_id { get; set; }
        public string latest_timestamp { get; set; }
        public List<string> post_id_list { get; set; }
        public int remaining_count { get; set; }

        public List<UT_UsertrackList> usertrack_list { get; set; }
    }

    #endregion

    #region MR_changelogs

    public class CL_PostItem
    {
        public string post_id { get; set; }
        public int seq_num { get; set; }
        public string update_time { get; set; }
    }

    public class MR_changelogs_get : General_R
    {
        public string group_id { get; set; }
        public int next_seq_num { get; set; }
        public int remaining_count { get; set; }
        public List<CL_PostItem> post_list { get; set; }
        public List<UT_UsertrackList> changelog_list { get; set; }
    }

    #endregion
}
