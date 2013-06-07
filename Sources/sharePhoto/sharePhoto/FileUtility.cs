using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Wpf_testHTTP
{
    public class FileUtility
    {
        #region MimeType

        public static string GetMimeType(FileInfo fileInfo)
        {
            string _mimeType = "application/unknown";

            RegistryKey _regKey = Registry.ClassesRoot.OpenSubKey(
                fileInfo.Extension.ToLower()
                );

            if (_regKey != null)
            {
                object _contentType = _regKey.GetValue("Content Type");

                if (_contentType != null)
                    _mimeType = _contentType.ToString();
            }

            if (_mimeType == "application/unknown")
                _mimeType = GetMimeType2(fileInfo.FullName);

            return _mimeType;
        }

        private static string GetMimeType2(string strFileName)
        {
            string _ret;

            switch (Path.GetExtension(strFileName).ToLower())
            {
                case ".3dm":
                    _ret = "x-world/x-3dmf";
                    break;
                case ".3dmf":
                    _ret = "x-world/x-3dmf";
                    break;
                case ".a":
                    _ret = "application/octet-stream";
                    break;
                case ".aab":
                    _ret = "application/x-authorware-bin";
                    break;
                case ".aam":
                    _ret = "application/x-authorware-map";
                    break;
                case ".aas":
                    _ret = "application/x-authorware-seg";
                    break;
                case ".abc":
                    _ret = "text/vnd.abc";
                    break;
                case ".acgi":
                    _ret = "text/html";
                    break;
                case ".afl":
                    _ret = "video/animaflex";
                    break;
                case ".ai":
                    _ret = "application/postscript";
                    break;
                case ".aif":
                    _ret = "audio/aiff";
                    break;
                case ".aifc":
                    _ret = "audio/aiff";
                    break;
                case ".aiff":
                    _ret = "audio/aiff";
                    break;
                case ".aim":
                    _ret = "application/x-aim";
                    break;
                case ".aip":
                    _ret = "text/x-audiosoft-intra";
                    break;
                case ".ani":
                    _ret = "application/x-navi-animation";
                    break;
                case ".aos":
                    _ret = "application/x-nokia-9000-communicator-add-on-software";
                    break;
                case ".aps":
                    _ret = "application/mime";
                    break;
                case ".arc":
                    _ret = "application/octet-stream";
                    break;
                case ".arj":
                    _ret = "application/arj";
                    break;
                case ".art":
                    _ret = "image/x-jg";
                    break;
                case ".asf":
                    _ret = "video/x-ms-asf";
                    break;
                case ".asm":
                    _ret = "text/x-asm";
                    break;
                case ".asp":
                    _ret = "text/asp";
                    break;
                case ".asx":
                    _ret = "video/x-ms-asf";
                    break;
                case ".au":
                    _ret = "audio/basic";
                    break;
                case ".avi":
                    _ret = "video/avi";
                    break;
                case ".avs":
                    _ret = "video/avs-video";
                    break;
                case ".bcpio":
                    _ret = "application/x-bcpio";
                    break;
                case ".bin":
                    _ret = "application/octet-stream";
                    break;
                case ".bm":
                    _ret = "image/bmp";
                    break;
                case ".bmp":
                    _ret = "image/bmp";
                    break;
                case ".boo":
                    _ret = "application/book";
                    break;
                case ".book":
                    _ret = "application/book";
                    break;
                case ".boz":
                    _ret = "application/x-bzip2";
                    break;
                case ".bsh":
                    _ret = "application/x-bsh";
                    break;
                case ".bz":
                    _ret = "application/x-bzip";
                    break;
                case ".bz2":
                    _ret = "application/x-bzip2";
                    break;
                case ".c":
                    _ret = "text/plain";
                    break;
                case ".c++":
                    _ret = "text/plain";
                    break;
                case ".cat":
                    _ret = "application/vnd.ms-pki.seccat";
                    break;
                case ".cc":
                    _ret = "text/plain";
                    break;
                case ".ccad":
                    _ret = "application/clariscad";
                    break;
                case ".cco":
                    _ret = "application/x-cocoa";
                    break;
                case ".cdf":
                    _ret = "application/cdf";
                    break;
                case ".cer":
                    _ret = "application/pkix-cert";
                    break;
                case ".cha":
                    _ret = "application/x-chat";
                    break;
                case ".chat":
                    _ret = "application/x-chat";
                    break;
                case ".class":
                    _ret = "application/java";
                    break;
                case ".com":
                    _ret = "application/octet-stream";
                    break;
                case ".conf":
                    _ret = "text/plain";
                    break;
                case ".cpio":
                    _ret = "application/x-cpio";
                    break;
                case ".cpp":
                    _ret = "text/x-c";
                    break;
                case ".cpt":
                    _ret = "application/x-cpt";
                    break;
                case ".crl":
                    _ret = "application/pkcs-crl";
                    break;
                case ".crt":
                    _ret = "application/pkix-cert";
                    break;
                case ".csh":
                    _ret = "application/x-csh";
                    break;
                case ".css":
                    _ret = "text/css";
                    break;
                case ".cxx":
                    _ret = "text/plain";
                    break;
                case ".dcr":
                    _ret = "application/x-director";
                    break;
                case ".deepv":
                    _ret = "application/x-deepv";
                    break;
                case ".def":
                    _ret = "text/plain";
                    break;
                case ".der":
                    _ret = "application/x-x509-ca-cert";
                    break;
                case ".dif":
                    _ret = "video/x-dv";
                    break;
                case ".dir":
                    _ret = "application/x-director";
                    break;
                case ".dl":
                    _ret = "video/dl";
                    break;
                case ".doc":
                    _ret = "application/msword";
                    break;
                case ".dot":
                    _ret = "application/msword";
                    break;
                case ".dp":
                    _ret = "application/commonground";
                    break;
                case ".drw":
                    _ret = "application/drafting";
                    break;
                case ".dump":
                    _ret = "application/octet-stream";
                    break;
                case ".dv":
                    _ret = "video/x-dv";
                    break;
                case ".dvi":
                    _ret = "application/x-dvi";
                    break;
                case ".dwf":
                    _ret = "model/vnd.dwf";
                    break;
                case ".dwg":
                    _ret = "image/vnd.dwg";
                    break;
                case ".dxf":
                    _ret = "image/vnd.dwg";
                    break;
                case ".dxr":
                    _ret = "application/x-director";
                    break;
                case ".el":
                    _ret = "text/x-script.elisp";
                    break;
                case ".elc":
                    _ret = "application/x-elc";
                    break;
                case ".env":
                    _ret = "application/x-envoy";
                    break;
                case ".eps":
                    _ret = "application/postscript";
                    break;
                case ".es":
                    _ret = "application/x-esrehber";
                    break;
                case ".etx":
                    _ret = "text/x-setext";
                    break;
                case ".evy":
                    _ret = "application/envoy";
                    break;
                case ".exe":
                    _ret = "application/octet-stream";
                    break;
                case ".f":
                    _ret = "text/plain";
                    break;
                case ".f77":
                    _ret = "text/x-fortran";
                    break;
                case ".f90":
                    _ret = "text/plain";
                    break;
                case ".fdf":
                    _ret = "application/vnd.fdf";
                    break;
                case ".fif":
                    _ret = "image/fif";
                    break;
                case ".fli":
                    _ret = "video/fli";
                    break;
                case ".flo":
                    _ret = "image/florian";
                    break;
                case ".flx":
                    _ret = "text/vnd.fmi.flexstor";
                    break;
                case ".fmf":
                    _ret = "video/x-atomic3d-feature";
                    break;
                case ".for":
                    _ret = "text/x-fortran";
                    break;
                case ".fpx":
                    _ret = "image/vnd.fpx";
                    break;
                case ".frl":
                    _ret = "application/freeloader";
                    break;
                case ".funk":
                    _ret = "audio/make";
                    break;
                case ".g":
                    _ret = "text/plain";
                    break;
                case ".g3":
                    _ret = "image/g3fax";
                    break;
                case ".gif":
                    _ret = "image/gif";
                    break;
                case ".gl":
                    _ret = "video/gl";
                    break;
                case ".gsd":
                    _ret = "audio/x-gsm";
                    break;
                case ".gsm":
                    _ret = "audio/x-gsm";
                    break;
                case ".gsp":
                    _ret = "application/x-gsp";
                    break;
                case ".gss":
                    _ret = "application/x-gss";
                    break;
                case ".gtar":
                    _ret = "application/x-gtar";
                    break;
                case ".gz":
                    _ret = "application/x-gzip";
                    break;
                case ".gzip":
                    _ret = "application/x-gzip";
                    break;
                case ".h":
                    _ret = "text/plain";
                    break;
                case ".hdf":
                    _ret = "application/x-hdf";
                    break;
                case ".help":
                    _ret = "application/x-helpfile";
                    break;
                case ".hgl":
                    _ret = "application/vnd.hp-hpgl";
                    break;
                case ".hh":
                    _ret = "text/plain";
                    break;
                case ".hlb":
                    _ret = "text/x-script";
                    break;
                case ".hlp":
                    _ret = "application/hlp";
                    break;
                case ".hpg":
                    _ret = "application/vnd.hp-hpgl";
                    break;
                case ".hpgl":
                    _ret = "application/vnd.hp-hpgl";
                    break;
                case ".hqx":
                    _ret = "application/binhex";
                    break;
                case ".hta":
                    _ret = "application/hta";
                    break;
                case ".htc":
                    _ret = "text/x-component";
                    break;
                case ".htm":
                    _ret = "text/html";
                    break;
                case ".html":
                    _ret = "text/html";
                    break;
                case ".htmls":
                    _ret = "text/html";
                    break;
                case ".htt":
                    _ret = "text/webviewhtml";
                    break;
                case ".htx":
                    _ret = "text/html";
                    break;
                case ".ice":
                    _ret = "x-conference/x-cooltalk";
                    break;
                case ".ico":
                    _ret = "image/x-icon";
                    break;
                case ".idc":
                    _ret = "text/plain";
                    break;
                case ".ief":
                    _ret = "image/ief";
                    break;
                case ".iefs":
                    _ret = "image/ief";
                    break;
                case ".iges":
                    _ret = "application/iges";
                    break;
                case ".igs":
                    _ret = "application/iges";
                    break;
                case ".ima":
                    _ret = "application/x-ima";
                    break;
                case ".imap":
                    _ret = "application/x-httpd-imap";
                    break;
                case ".inf":
                    _ret = "application/inf";
                    break;
                case ".ins":
                    _ret = "application/x-internett-signup";
                    break;
                case ".ip":
                    _ret = "application/x-ip2";
                    break;
                case ".isu":
                    _ret = "video/x-isvideo";
                    break;
                case ".it":
                    _ret = "audio/it";
                    break;
                case ".iv":
                    _ret = "application/x-inventor";
                    break;
                case ".ivr":
                    _ret = "i-world/i-vrml";
                    break;
                case ".ivy":
                    _ret = "application/x-livescreen";
                    break;
                case ".jam":
                    _ret = "audio/x-jam";
                    break;
                case ".jav":
                    _ret = "text/plain";
                    break;
                case ".java":
                    _ret = "text/plain";
                    break;
                case ".jcm":
                    _ret = "application/x-java-commerce";
                    break;
                case ".jfif":
                    _ret = "image/jpeg";
                    break;
                case ".jfif-tbnl":
                    _ret = "image/jpeg";
                    break;
                case ".jpe":
                    _ret = "image/jpeg";
                    break;
                case ".jpeg":
                    _ret = "image/jpeg";
                    break;
                case ".jpg":
                    _ret = "image/jpeg";
                    break;
                case ".jps":
                    _ret = "image/x-jps";
                    break;
                case ".js":
                    _ret = "application/x-javascript";
                    break;
                case ".jut":
                    _ret = "image/jutvision";
                    break;
                case ".kar":
                    _ret = "audio/midi";
                    break;
                case ".ksh":
                    _ret = "application/x-ksh";
                    break;
                case ".la":
                    _ret = "audio/nspaudio";
                    break;
                case ".lam":
                    _ret = "audio/x-liveaudio";
                    break;
                case ".latex":
                    _ret = "application/x-latex";
                    break;
                case ".lha":
                    _ret = "application/octet-stream";
                    break;
                case ".lhx":
                    _ret = "application/octet-stream";
                    break;
                case ".list":
                    _ret = "text/plain";
                    break;
                case ".lma":
                    _ret = "audio/nspaudio";
                    break;
                case ".log":
                    _ret = "text/plain";
                    break;
                case ".lsp":
                    _ret = "application/x-lisp";
                    break;
                case ".lst":
                    _ret = "text/plain";
                    break;
                case ".lsx":
                    _ret = "text/x-la-asf";
                    break;
                case ".ltx":
                    _ret = "application/x-latex";
                    break;
                case ".lzh":
                    _ret = "application/octet-stream";
                    break;
                case ".lzx":
                    _ret = "application/octet-stream";
                    break;
                case ".m":
                    _ret = "text/plain";
                    break;
                case ".m1v":
                    _ret = "video/mpeg";
                    break;
                case ".m2a":
                    _ret = "audio/mpeg";
                    break;
                case ".m2v":
                    _ret = "video/mpeg";
                    break;
                case ".m3u":
                    _ret = "audio/x-mpequrl";
                    break;
                case ".man":
                    _ret = "application/x-troff-man";
                    break;
                case ".map":
                    _ret = "application/x-navimap";
                    break;
                case ".mar":
                    _ret = "text/plain";
                    break;
                case ".mbd":
                    _ret = "application/mbedlet";
                    break;
                case ".mc$":
                    _ret = "application/x-magic-cap-package-1.0";
                    break;
                case ".mcd":
                    _ret = "application/mcad";
                    break;
                case ".mcf":
                    _ret = "text/mcf";
                    break;
                case ".mcp":
                    _ret = "application/netmc";
                    break;
                case ".me":
                    _ret = "application/x-troff-me";
                    break;
                case ".mht":
                    _ret = "message/rfc822";
                    break;
                case ".mhtml":
                    _ret = "message/rfc822";
                    break;
                case ".mid":
                    _ret = "audio/midi";
                    break;
                case ".midi":
                    _ret = "audio/midi";
                    break;
                case ".mif":
                    _ret = "application/x-mif";
                    break;
                case ".mime":
                    _ret = "message/rfc822";
                    break;
                case ".mjf":
                    _ret = "audio/x-vnd.audioexplosion.mjuicemediafile";
                    break;
                case ".mjpg":
                    _ret = "video/x-motion-jpeg";
                    break;
                case ".mm":
                    _ret = "application/base64";
                    break;
                case ".mme":
                    _ret = "application/base64";
                    break;
                case ".mod":
                    _ret = "audio/mod";
                    break;
                case ".moov":
                    _ret = "video/quicktime";
                    break;
                case ".mov":
                    _ret = "video/quicktime";
                    break;
                case ".movie":
                    _ret = "video/x-sgi-movie";
                    break;
                case ".mp2":
                    _ret = "audio/mpeg";
                    break;
                case ".mp3":
                    _ret = "audio/mpeg";
                    break;
                case ".mpa":
                    _ret = "audio/mpeg";
                    break;
                case ".mpc":
                    _ret = "application/x-project";
                    break;
                case ".mpe":
                    _ret = "video/mpeg";
                    break;
                case ".mpeg":
                    _ret = "video/mpeg";
                    break;
                case ".mpg":
                    _ret = "video/mpeg";
                    break;
                case ".mpga":
                    _ret = "audio/mpeg";
                    break;
                case ".mpp":
                    _ret = "application/vnd.ms-project";
                    break;
                case ".mpt":
                    _ret = "application/vnd.ms-project";
                    break;
                case ".mpv":
                    _ret = "application/vnd.ms-project";
                    break;
                case ".mpx":
                    _ret = "application/vnd.ms-project";
                    break;
                case ".mrc":
                    _ret = "application/marc";
                    break;
                case ".ms":
                    _ret = "application/x-troff-ms";
                    break;
                case ".mv":
                    _ret = "video/x-sgi-movie";
                    break;
                case ".my":
                    _ret = "audio/make";
                    break;
                case ".mzz":
                    _ret = "application/x-vnd.audioexplosion.mzz";
                    break;
                case ".nap":
                    _ret = "image/naplps";
                    break;
                case ".naplps":
                    _ret = "image/naplps";
                    break;
                case ".nc":
                    _ret = "application/x-netcdf";
                    break;
                case ".ncm":
                    _ret = "application/vnd.nokia.configuration-message";
                    break;
                case ".nif":
                    _ret = "image/x-niff";
                    break;
                case ".niff":
                    _ret = "image/x-niff";
                    break;
                case ".nix":
                    _ret = "application/x-mix-transfer";
                    break;
                case ".nsc":
                    _ret = "application/x-conference";
                    break;
                case ".nvd":
                    _ret = "application/x-navidoc";
                    break;
                case ".o":
                    _ret = "application/octet-stream";
                    break;
                case ".oda":
                    _ret = "application/oda";
                    break;
                case ".omc":
                    _ret = "application/x-omc";
                    break;
                case ".omcd":
                    _ret = "application/x-omcdatamaker";
                    break;
                case ".omcr":
                    _ret = "application/x-omcregerator";
                    break;
                case ".p":
                    _ret = "text/x-pascal";
                    break;
                case ".p10":
                    _ret = "application/pkcs10";
                    break;
                case ".p12":
                    _ret = "application/pkcs-12";
                    break;
                case ".p7a":
                    _ret = "application/x-pkcs7-signature";
                    break;
                case ".p7c":
                    _ret = "application/pkcs7-mime";
                    break;
                case ".p7m":
                    _ret = "application/pkcs7-mime";
                    break;
                case ".p7r":
                    _ret = "application/x-pkcs7-certreqresp";
                    break;
                case ".p7s":
                    _ret = "application/pkcs7-signature";
                    break;
                case ".part":
                    _ret = "application/pro_eng";
                    break;
                case ".pas":
                    _ret = "text/pascal";
                    break;
                case ".pbm":
                    _ret = "image/x-portable-bitmap";
                    break;
                case ".pcl":
                    _ret = "application/vnd.hp-pcl";
                    break;
                case ".pct":
                    _ret = "image/x-pict";
                    break;
                case ".pcx":
                    _ret = "image/x-pcx";
                    break;
                case ".pdb":
                    _ret = "chemical/x-pdb";
                    break;
                case ".pdf":
                    _ret = "application/pdf";
                    break;
                case ".pfunk":
                    _ret = "audio/make";
                    break;
                case ".pgm":
                    _ret = "image/x-portable-greymap";
                    break;
                case ".pic":
                    _ret = "image/pict";
                    break;
                case ".pict":
                    _ret = "image/pict";
                    break;
                case ".pkg":
                    _ret = "application/x-newton-compatible-pkg";
                    break;
                case ".pko":
                    _ret = "application/vnd.ms-pki.pko";
                    break;
                case ".pl":
                    _ret = "text/plain";
                    break;
                case ".plx":
                    _ret = "application/x-pixclscript";
                    break;
                case ".pm":
                    _ret = "image/x-xpixmap";
                    break;
                case ".pm4":
                    _ret = "application/x-pagemaker";
                    break;
                case ".pm5":
                    _ret = "application/x-pagemaker";
                    break;
                case ".png":
                    _ret = "image/png";
                    break;
                case ".pnm":
                    _ret = "application/x-portable-anymap";
                    break;
                case ".pot":
                    _ret = "application/vnd.ms-powerpoint";
                    break;
                case ".pov":
                    _ret = "model/x-pov";
                    break;
                case ".ppa":
                    _ret = "application/vnd.ms-powerpoint";
                    break;
                case ".ppm":
                    _ret = "image/x-portable-pixmap";
                    break;
                case ".pps":
                    _ret = "application/vnd.ms-powerpoint";
                    break;
                case ".ppt":
                    _ret = "application/vnd.ms-powerpoint";
                    break;
                case ".ppz":
                    _ret = "application/vnd.ms-powerpoint";
                    break;
                case ".pre":
                    _ret = "application/x-freelance";
                    break;
                case ".prt":
                    _ret = "application/pro_eng";
                    break;
                case ".ps":
                    _ret = "application/postscript";
                    break;
                case ".psd":
                    _ret = "application/octet-stream";
                    break;
                case ".pvu":
                    _ret = "paleovu/x-pv";
                    break;
                case ".pwz":
                    _ret = "application/vnd.ms-powerpoint";
                    break;
                case ".py":
                    _ret = "text/x-script.phyton";
                    break;
                case ".pyc":
                    _ret = "applicaiton/x-bytecode.python";
                    break;
                case ".qcp":
                    _ret = "audio/vnd.qcelp";
                    break;
                case ".qd3":
                    _ret = "x-world/x-3dmf";
                    break;
                case ".qd3d":
                    _ret = "x-world/x-3dmf";
                    break;
                case ".qif":
                    _ret = "image/x-quicktime";
                    break;
                case ".qt":
                    _ret = "video/quicktime";
                    break;
                case ".qtc":
                    _ret = "video/x-qtc";
                    break;
                case ".qti":
                    _ret = "image/x-quicktime";
                    break;
                case ".qtif":
                    _ret = "image/x-quicktime";
                    break;
                case ".ra":
                    _ret = "audio/x-pn-realaudio";
                    break;
                case ".ram":
                    _ret = "audio/x-pn-realaudio";
                    break;
                case ".ras":
                    _ret = "application/x-cmu-raster";
                    break;
                case ".rast":
                    _ret = "image/cmu-raster";
                    break;
                case ".rexx":
                    _ret = "text/x-script.rexx";
                    break;
                case ".rf":
                    _ret = "image/vnd.rn-realflash";
                    break;
                case ".rgb":
                    _ret = "image/x-rgb";
                    break;
                case ".rm":
                    _ret = "application/vnd.rn-realmedia";
                    break;
                case ".rmi":
                    _ret = "audio/mid";
                    break;
                case ".rmm":
                    _ret = "audio/x-pn-realaudio";
                    break;
                case ".rmp":
                    _ret = "audio/x-pn-realaudio";
                    break;
                case ".rng":
                    _ret = "application/ringing-tones";
                    break;
                case ".rnx":
                    _ret = "application/vnd.rn-realplayer";
                    break;
                case ".roff":
                    _ret = "application/x-troff";
                    break;
                case ".rp":
                    _ret = "image/vnd.rn-realpix";
                    break;
                case ".rpm":
                    _ret = "audio/x-pn-realaudio-plugin";
                    break;
                case ".rt":
                    _ret = "text/richtext";
                    break;
                case ".rtf":
                    _ret = "text/richtext";
                    break;
                case ".rtx":
                    _ret = "text/richtext";
                    break;
                case ".rv":
                    _ret = "video/vnd.rn-realvideo";
                    break;
                case ".s":
                    _ret = "text/x-asm";
                    break;
                case ".s3m":
                    _ret = "audio/s3m";
                    break;
                case ".saveme":
                    _ret = "application/octet-stream";
                    break;
                case ".sbk":
                    _ret = "application/x-tbook";
                    break;
                case ".scm":
                    _ret = "application/x-lotusscreencam";
                    break;
                case ".sdml":
                    _ret = "text/plain";
                    break;
                case ".sdp":
                    _ret = "application/sdp";
                    break;
                case ".sdr":
                    _ret = "application/sounder";
                    break;
                case ".sea":
                    _ret = "application/sea";
                    break;
                case ".set":
                    _ret = "application/set";
                    break;
                case ".sgm":
                    _ret = "text/sgml";
                    break;
                case ".sgml":
                    _ret = "text/sgml";
                    break;
                case ".sh":
                    _ret = "application/x-sh";
                    break;
                case ".shar":
                    _ret = "application/x-shar";
                    break;
                case ".shtml":
                    _ret = "text/html";
                    break;
                case ".sid":
                    _ret = "audio/x-psid";
                    break;
                case ".sit":
                    _ret = "application/x-sit";
                    break;
                case ".skd":
                    _ret = "application/x-koan";
                    break;
                case ".skm":
                    _ret = "application/x-koan";
                    break;
                case ".skp":
                    _ret = "application/x-koan";
                    break;
                case ".skt":
                    _ret = "application/x-koan";
                    break;
                case ".sl":
                    _ret = "application/x-seelogo";
                    break;
                case ".smi":
                    _ret = "application/smil";
                    break;
                case ".smil":
                    _ret = "application/smil";
                    break;
                case ".snd":
                    _ret = "audio/basic";
                    break;
                case ".sol":
                    _ret = "application/solids";
                    break;
                case ".spc":
                    _ret = "text/x-speech";
                    break;
                case ".spl":
                    _ret = "application/futuresplash";
                    break;
                case ".spr":
                    _ret = "application/x-sprite";
                    break;
                case ".sprite":
                    _ret = "application/x-sprite";
                    break;
                case ".src":
                    _ret = "application/x-wais-source";
                    break;
                case ".ssi":
                    _ret = "text/x-server-parsed-html";
                    break;
                case ".ssm":
                    _ret = "application/streamingmedia";
                    break;
                case ".sst":
                    _ret = "application/vnd.ms-pki.certstore";
                    break;
                case ".step":
                    _ret = "application/step";
                    break;
                case ".stl":
                    _ret = "application/sla";
                    break;
                case ".stp":
                    _ret = "application/step";
                    break;
                case ".sv4cpio":
                    _ret = "application/x-sv4cpio";
                    break;
                case ".sv4crc":
                    _ret = "application/x-sv4crc";
                    break;
                case ".svf":
                    _ret = "image/vnd.dwg";
                    break;
                case ".svr":
                    _ret = "application/x-world";
                    break;
                case ".swf":
                    _ret = "application/x-shockwave-flash";
                    break;
                case ".t":
                    _ret = "application/x-troff";
                    break;
                case ".talk":
                    _ret = "text/x-speech";
                    break;
                case ".tar":
                    _ret = "application/x-tar";
                    break;
                case ".tbk":
                    _ret = "application/toolbook";
                    break;
                case ".tcl":
                    _ret = "application/x-tcl";
                    break;
                case ".tcsh":
                    _ret = "text/x-script.tcsh";
                    break;
                case ".tex":
                    _ret = "application/x-tex";
                    break;
                case ".texi":
                    _ret = "application/x-texinfo";
                    break;
                case ".texinfo":
                    _ret = "application/x-texinfo";
                    break;
                case ".text":
                    _ret = "text/plain";
                    break;
                case ".tgz":
                    _ret = "application/x-compressed";
                    break;
                case ".tif":
                    _ret = "image/tiff";
                    break;
                case ".tiff":
                    _ret = "image/tiff";
                    break;
                case ".tr":
                    _ret = "application/x-troff";
                    break;
                case ".tsi":
                    _ret = "audio/tsp-audio";
                    break;
                case ".tsp":
                    _ret = "application/dsptype";
                    break;
                case ".tsv":
                    _ret = "text/tab-separated-values";
                    break;
                case ".turbot":
                    _ret = "image/florian";
                    break;
                case ".txt":
                    _ret = "text/plain";
                    break;
                case ".uil":
                    _ret = "text/x-uil";
                    break;
                case ".uni":
                    _ret = "text/uri-list";
                    break;
                case ".unis":
                    _ret = "text/uri-list";
                    break;
                case ".unv":
                    _ret = "application/i-deas";
                    break;
                case ".uri":
                    _ret = "text/uri-list";
                    break;
                case ".uris":
                    _ret = "text/uri-list";
                    break;
                case ".ustar":
                    _ret = "application/x-ustar";
                    break;
                case ".uu":
                    _ret = "application/octet-stream";
                    break;
                case ".uue":
                    _ret = "text/x-uuencode";
                    break;
                case ".vcd":
                    _ret = "application/x-cdlink";
                    break;
                case ".vcs":
                    _ret = "text/x-vcalendar";
                    break;
                case ".vda":
                    _ret = "application/vda";
                    break;
                case ".vdo":
                    _ret = "video/vdo";
                    break;
                case ".vew":
                    _ret = "application/groupwise";
                    break;
                case ".viv":
                    _ret = "video/vivo";
                    break;
                case ".vivo":
                    _ret = "video/vivo";
                    break;
                case ".vmd":
                    _ret = "application/vocaltec-media-desc";
                    break;
                case ".vmf":
                    _ret = "application/vocaltec-media-file";
                    break;
                case ".voc":
                    _ret = "audio/voc";
                    break;
                case ".vos":
                    _ret = "video/vosaic";
                    break;
                case ".vox":
                    _ret = "audio/voxware";
                    break;
                case ".vqe":
                    _ret = "audio/x-twinvq-plugin";
                    break;
                case ".vqf":
                    _ret = "audio/x-twinvq";
                    break;
                case ".vql":
                    _ret = "audio/x-twinvq-plugin";
                    break;
                case ".vrml":
                    _ret = "application/x-vrml";
                    break;
                case ".vrt":
                    _ret = "x-world/x-vrt";
                    break;
                case ".vsd":
                    _ret = "application/x-visio";
                    break;
                case ".vst":
                    _ret = "application/x-visio";
                    break;
                case ".vsw":
                    _ret = "application/x-visio";
                    break;
                case ".w60":
                    _ret = "application/wordperfect6.0";
                    break;
                case ".w61":
                    _ret = "application/wordperfect6.1";
                    break;
                case ".w6w":
                    _ret = "application/msword";
                    break;
                case ".wav":
                    _ret = "audio/wav";
                    break;
                case ".wb1":
                    _ret = "application/x-qpro";
                    break;
                case ".wbmp":
                    _ret = "image/vnd.wap.wbmp";
                    break;
                case ".web":
                    _ret = "application/vnd.xara";
                    break;
                case ".wiz":
                    _ret = "application/msword";
                    break;
                case ".wk1":
                    _ret = "application/x-123";
                    break;
                case ".wmf":
                    _ret = "windows/metafile";
                    break;
                case ".wml":
                    _ret = "text/vnd.wap.wml";
                    break;
                case ".wmlc":
                    _ret = "application/vnd.wap.wmlc";
                    break;
                case ".wmls":
                    _ret = "text/vnd.wap.wmlscript";
                    break;
                case ".wmlsc":
                    _ret = "application/vnd.wap.wmlscriptc";
                    break;
                case ".word":
                    _ret = "application/msword";
                    break;
                case ".wp":
                    _ret = "application/wordperfect";
                    break;
                case ".wp5":
                    _ret = "application/wordperfect";
                    break;
                case ".wp6":
                    _ret = "application/wordperfect";
                    break;
                case ".wpd":
                    _ret = "application/wordperfect";
                    break;
                case ".wq1":
                    _ret = "application/x-lotus";
                    break;
                case ".wri":
                    _ret = "application/mswrite";
                    break;
                case ".wrl":
                    _ret = "application/x-world";
                    break;
                case ".wrz":
                    _ret = "x-world/x-vrml";
                    break;
                case ".wsc":
                    _ret = "text/scriplet";
                    break;
                case ".wsrc":
                    _ret = "application/x-wais-source";
                    break;
                case ".wtk":
                    _ret = "application/x-wintalk";
                    break;
                case ".xbm":
                    _ret = "image/x-xbitmap";
                    break;
                case ".xdr":
                    _ret = "video/x-amt-demorun";
                    break;
                case ".xgz":
                    _ret = "xgl/drawing";
                    break;
                case ".xif":
                    _ret = "image/vnd.xiff";
                    break;
                case ".xl":
                    _ret = "application/excel";
                    break;
                case ".xla":
                    _ret = "application/vnd.ms-excel";
                    break;
                case ".xlb":
                    _ret = "application/vnd.ms-excel";
                    break;
                case ".xlc":
                    _ret = "application/vnd.ms-excel";
                    break;
                case ".xld":
                    _ret = "application/vnd.ms-excel";
                    break;
                case ".xlk":
                    _ret = "application/vnd.ms-excel";
                    break;
                case ".xll":
                    _ret = "application/vnd.ms-excel";
                    break;
                case ".xlm":
                    _ret = "application/vnd.ms-excel";
                    break;
                case ".xls":
                    _ret = "application/vnd.ms-excel";
                    break;
                case ".xlt":
                    _ret = "application/vnd.ms-excel";
                    break;
                case ".xlv":
                    _ret = "application/vnd.ms-excel";
                    break;
                case ".xlw":
                    _ret = "application/vnd.ms-excel";
                    break;
                case ".xm":
                    _ret = "audio/xm";
                    break;
                case ".xml":
                    _ret = "application/xml";
                    break;
                case ".xmz":
                    _ret = "xgl/movie";
                    break;
                case ".xpix":
                    _ret = "application/x-vnd.ls-xpix";
                    break;
                case ".xpm":
                    _ret = "image/xpm";
                    break;
                case ".x-png":
                    _ret = "image/png";
                    break;
                case ".xsr":
                    _ret = "video/x-amt-showrun";
                    break;
                case ".xwd":
                    _ret = "image/x-xwd";
                    break;
                case ".xyz":
                    _ret = "chemical/x-pdb";
                    break;
                case ".z":
                    _ret = "application/x-compressed";
                    break;
                case ".zip":
                    _ret = "application/zip";
                    break;
                case ".zoo":
                    _ret = "application/octet-stream";
                    break;
                case ".zsh":
                    _ret = "text/x-script.zsh";
                    break;
                default:
                    _ret = "application/octet-stream";
                    break;
            }

            return _ret;
        }

        #endregion

        public static string saveFileWithoutOverwrite(string fileName, string saveToFolder)
        {
            int _count = 1;
            string[] _fileNameSplit = fileName.Split(new[] { '.' });
            string _ext = "." + _fileNameSplit[_fileNameSplit.Length - 1];
            string _prefix = fileName.Substring(0, fileName.Length - _ext.Length);

            while (File.Exists(saveToFolder + fileName))
            {
                fileName = _prefix + " (" + _count.ToString() + ")" + _ext;
                _count++;
            }

            return saveToFolder + fileName;
        }

        // Reads data from a stream until the end is reached. The
        // data is returned as a byte array. An IOException is
        // thrown if any of the underlying IO calls fail.
        public static byte[] ReadFully(Stream stream)
        {
            byte[] _buffer = new byte[32768];

            using (MemoryStream _ms = new MemoryStream())
            {
                while (true)
                {
                    int _read = stream.Read(_buffer, 0, _buffer.Length);

                    if (_read <= 0)
                        return _ms.ToArray();

                    _ms.Write(_buffer, 0, _read);
                }
            }
        }

        public static byte[] ConvertFileToByteArray(string fileName)
        {
            byte[] _ret;

            using (FileStream _fr = File.OpenRead(fileName))
            {
                _ret = ReadFully(_fr);

                //對, 但有問題的方式 
                //
                //using (BinaryReader _br = new BinaryReader(_fr))
                //{
                //    _ret = _br.ReadBytes((int)_fr.Length);
                //}
            }

            return _ret;
        }

        public static float ConvertBytesToMegaBytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
    }
}