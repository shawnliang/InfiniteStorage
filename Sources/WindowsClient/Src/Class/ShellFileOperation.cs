using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace TVM.SnailTools.lib
{
    /// <summary>
    /// Windows的文件操作API：SHFileOperation，实现文件的复制，移动，删除和重命名
    /// </summary>
    public class ShellFileOperation
    {
        /// <summary>
        /// 映射API方法
        /// </summary>
        /// <param name="lpFileOp"></param>
        /// <returns></returns>
        [DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int SHFileOperation(SHFILEOPSTRUCT lpFileOp);

        /// <summary>
        /// 错误码映射表
        /// </summary>
        private static Dictionary<string, string> ErrorMap = null;

        /// <summary>
        /// 多个文件路径的分隔符
        /// </summary>
        private const string FILE_SPLITER = "\0";

        /// <summary>
        /// 复制单个文件
        /// </summary>
        /// <param name="sourceFile">要复制的文件路径</param>
        /// <param name="targetFile">目标文件路径</param>
        /// <returns>0表示成功，其余为错误码，可通过GetErrorString()方法获取错误内容</returns>
        public static int Copy(string sourceFile, string targetFile)
        {
            return Copy(new string[] { sourceFile }, new string[] { targetFile });
        }

        /// <summary>
        /// 复制多个文件，源文件和目标文件列表个数必须一致
        /// </summary>
        /// <param name="sourceFiles">要复制的文件路径列表</param>
        /// <param name="targetFiles">目标文件路径列表</param>
        /// <returns>0表示成功，其余为错误码，可通过GetErrorString()方法获取错误内容</returns>
        public static int Copy(IEnumerable<string> sourceFiles, IEnumerable<string> targetFiles)
        {
            return Copy(sourceFiles, targetFiles, false);
        }

        /// <summary>
        /// 复制多个文件，源文件和目标文件列表个数必须一致
        /// </summary>
        /// <param name="sourceFiles">要复制的文件路径列表</param>
        /// <param name="targetFiles">目标文件路径列表</param>
        /// <param name="targetFiles">是否删除源文件，为true时使用“移动”而非“复制”</param>
        /// <returns>0表示成功，其余为错误码，可通过GetErrorString()方法获取错误内容</returns>
        public static int Copy(IEnumerable<string> sourceFiles, IEnumerable<string> targetFiles, bool deleteSourceFile)
        {
            if (sourceFiles.Count() != targetFiles.Count())
            {
                throw new ArgumentException("sourceFiles和targetFiles个数不一致");
            }
            SHFILEOPSTRUCT pm = new SHFILEOPSTRUCT();
            if (true == deleteSourceFile)
            {
                pm.wFunc = wFunc.FO_MOVE;
                pm.lpszProgressTitle = "移动文件";
            }
            else
            {
                pm.wFunc = wFunc.FO_COPY;
                pm.lpszProgressTitle = "复制文件";
            }
            pm.pFrom = "";
            pm.pTo = "";
            foreach (string file in sourceFiles)
            {
                pm.pFrom += file + FILE_SPLITER;
            }
            pm.pFrom += FILE_SPLITER;
            foreach (string file in targetFiles)
            {
                pm.pTo += file + FILE_SPLITER;
            }
            pm.pTo += FILE_SPLITER;
            pm.fFlags = FILEOP_FLAGS.FOF_NOCONFIRMATION | FILEOP_FLAGS.FOF_MULTIDESTFILES | FILEOP_FLAGS.FOF_ALLOWUNDO;

            return SHFileOperation(pm);
        }

        /// <summary>
        /// 移动单个文件
        /// </summary>
        /// <param name="sourceFile">要移动的文件路径</param>
        /// <param name="targetFile">目标文件路径</param>
        /// <returns>0表示成功，其余为错误码，可通过GetErrorString()方法获取错误内容</returns>
        public static int Move(string sourceFile, string targetFile)
        {
            return Move(new string[] { sourceFile }, new string[] { targetFile });
        }

        /// <summary>
        /// 移动多个文件，源文件和目标文件列表个数必须一致
        /// </summary>
        /// <param name="sourceFiles">要移动的文件路径列表</param>
        /// <param name="targetFiles">目标文件路径列表</param>
        /// <returns>0表示成功，其余为错误码，可通过GetErrorString()方法获取错误内容</returns>
        public static int Move(IEnumerable<string> sourceFiles, IEnumerable<string> targetFiles)
        {
            return Copy(sourceFiles, targetFiles, true);
        }

        /// <summary>
        /// 删除文件到回收站
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>0表示成功，其余为错误码，可通过GetErrorString()方法获取错误内容</returns>
        public static int Delete(string filePath)
        {
            return Delete(filePath, true);
        }

        /// <summary>
        /// 删除文件，并设置是否允许撤销
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="allowUndo">是否允许撤销</param>
        /// <returns>0表示成功，其余为错误码，可通过GetErrorString()方法获取错误内容</returns>
        public static int Delete(string filePath, bool allowUndo)
        {
            SHFILEOPSTRUCT lpFileOp = new SHFILEOPSTRUCT();
            lpFileOp.wFunc = wFunc.FO_DELETE;
            lpFileOp.pFrom = filePath + FILE_SPLITER;
            lpFileOp.fFlags = FILEOP_FLAGS.FOF_NOCONFIRMATION | FILEOP_FLAGS.FOF_NOERRORUI | FILEOP_FLAGS.FOF_SILENT;
            if (true == allowUndo)
            {
                lpFileOp.fFlags |= FILEOP_FLAGS.FOF_ALLOWUNDO;
            }
            lpFileOp.fAnyOperationsAborted = false;
            return SHFileOperation(lpFileOp);
        }

        /// <summary>
        /// Shell文件操作数据类型
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private class SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            /// <summary>
            /// 设置操作方式，移动：FO_MOVE，复制：FO_COPY，删除：FO_DELETE
            /// </summary>
            public wFunc wFunc;
            /// <summary>
            /// 源文件路径
            /// </summary>
            public string pFrom;
            /// <summary>
            /// 目标文件路径
            /// </summary>
            public string pTo;
            /// <summary>
            /// 允许恢复
            /// </summary>
            public FILEOP_FLAGS fFlags;
            /// <summary>
            /// 监测有无中止
            /// </summary>
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            /// <summary>
            /// 设置标题
            /// </summary>
            public string lpszProgressTitle;
        }

        /// <summary>
        /// 文件操作方式
        /// </summary>
        private enum wFunc
        {
            /// <summary>
            /// 移动
            /// </summary>
            FO_MOVE = 0x0001,
            /// <summary>
            /// 复制
            /// </summary>
            FO_COPY = 0x0002,
            /// <summary>
            /// 删除
            /// </summary>
            FO_DELETE = 0x0003,
            /// <summary>
            /// 重命名
            /// </summary>
            FO_RENAME = 0x0004
        }

        /// <summary>
        /// fFlags枚举值，
        /// 参见：http://msdn.microsoft.com/zh-cn/library/bb759795(v=vs.85).aspx
        /// </summary>
        private enum FILEOP_FLAGS
        {

            ///<summary>
            ///pTo 指定了多个目标文件，而不是单个目录
            ///The pTo member specifies multiple destination files (one for each source file) rather than one directory where all source files are to be deposited. 
            ///</summary>
            FOF_MULTIDESTFILES = 0x1,
            ///<summary>
            ///不再使用
            ///Not currently used. 
            ///</summary>
            FOF_CONFIRMMOUSE = 0x2,
            ///<summary>
            ///不显示一个进度对话框
            ///Do not display a progress dialog box. 
            ///</summary>
            FOF_SILENT = 0x4,
            ///<summary>
            ///碰到有抵触的名字时，自动分配前缀
            ///Give the file being operated on a new name in a move, copy, or rename operation if a file with the target name already exists. 
            ///</summary>
            FOF_RENAMEONCOLLISION = 0x8,
            ///<summary>
            ///不对用户显示提示
            ///Respond with "Yes to All" for any dialog box that is displayed. 
            ///</summary>
            FOF_NOCONFIRMATION = 0x10,
            ///<summary>
            ///填充 hNameMappings 字段，必须使用 SHFreeNameMappings 释放
            ///If FOF_RENAMEONCOLLISION is specified and any files were renamed, assign a name mapping object containing their old and new names to the hNameMappings member.
            ///</summary>
            FOF_WANTMAPPINGHANDLE = 0x20,
            ///<summary>
            ///允许撤销
            ///Preserve Undo information, if possible. If pFrom does not contain fully qualified path and file names, this flag is ignored. 
            ///</summary>
            FOF_ALLOWUNDO = 0x40,
            ///<summary>
            ///使用 *.* 时, 只对文件操作
            ///Perform the operation on files only if a wildcard file name (*.*) is specified. 
            ///</summary>
            FOF_FILESONLY = 0x80,
            ///<summary>
            ///简单进度条，意味着不显示文件名。
            ///Display a progress dialog box but do not show the file names. 
            ///</summary>
            FOF_SIMPLEPROGRESS = 0x100,
            ///<summary>
            ///建新目录时不需要用户确定
            ///Do not confirm the creation of a new directory if the operation requires one to be created. 
            ///</summary>
            FOF_NOCONFIRMMKDIR = 0x200,
            ///<summary>
            ///不显示出错用户界面
            ///Do not display a user interface if an error occurs. 
            ///</summary>
            FOF_NOERRORUI = 0x400,
            ///<summary>
            /// 不复制 NT 文件的安全属性
            ///Do not copy the security attributes of the file.
            ///</summary>
            FOF_NOCOPYSECURITYATTRIBS = 0x800,
            ///<summary>
            /// 不递归目录
            ///Only operate in the local directory. Don't operate recursively into subdirectories.
            ///</summary>
            FOF_NORECURSION = 0x1000,
            ///<summary>
            ///Do not move connected files as a group. Only move the specified files. 
            ///</summary>
            FOF_NO_CONNECTED_ELEMENTS = 0x2000,
            ///<summary>
            ///Send a warning if a file is being destroyed during a delete operation rather than recycled. This flag partially overrides FOF_NOCONFIRMATION.
            ///</summary>
            FOF_WANTNUKEWARNING = 0x4000,
            ///<summary>
            ///Treat reparse points as objects, not containers.
            ///</summary>
            FOF_NORECURSEREPARSE = 0x8000,

        }

        /// <summary>
        /// 获取错误代码
        /// 参见：http://msdn.microsoft.com/zh-cn/library/bb762164(v=vs.85).aspx
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string GetErrorString(int n)
        {
            if (n == 0)
            {
                return string.Empty;
            }
            if (ErrorMap == null)
            {
                InitErrorMap();
            }

            string code = n.ToString("X").ToUpper();

            if (ErrorMap.ContainsKey(code))
            {
                return ErrorMap[code];
            }
            else
            {
                return code;
            }
        }

        /// <summary>
        /// 初始化错误码映射表
        /// </summary>
        private static void InitErrorMap()
        {
            ErrorMap = new Dictionary<string, string>();
            ErrorMap.Add("71", "DE_SAMEFILE : The source and destination files are the same file.");
            ErrorMap.Add("72", "DE_MANYSRC1DEST : Multiple file paths were specified in the source buffer, but only one destination file path.");
            ErrorMap.Add("73", "DE_DIFFDIR : Rename operation was specified but the destination path is a different directory. Use the move operation instead.");
            ErrorMap.Add("74", "DE_ROOTDIR : The source is a root directory, which cannot be moved or renamed.");
            ErrorMap.Add("75", "DE_OPCANCELLED : The operation was canceled by the user, or silently canceled if the appropriate flags were supplied to SHFileOperation.");
            ErrorMap.Add("76", "DE_DESTSUBTREE : The destination is a subtree of the source.");
            ErrorMap.Add("78", "DE_ACCESSDENIEDSRC : Security settings denied access to the source.");
            ErrorMap.Add("79", "DE_PATHTOODEEP : The source or destination path exceeded or would exceed MAX_PATH.");
            ErrorMap.Add("7A", "DE_MANYDEST : The operation involved multiple destination paths, which can fail in the case of a move operation.");
            ErrorMap.Add("7C", "DE_INVALIDFILES : The path in the source or destination or both was invalid.");
            ErrorMap.Add("7D", "DE_DESTSAMETREE : The source and destination have the same parent folder.");
            ErrorMap.Add("7E", "DE_FLDDESTISFILE : The destination path is an existing file.");
            ErrorMap.Add("80", "DE_FILEDESTISFLD : The destination path is an existing folder.");
            ErrorMap.Add("81", "DE_FILENAMETOOLONG : The name of the file exceeds MAX_PATH.");
            ErrorMap.Add("82", "DE_DEST_IS_CDROM : The destination is a read-only CD-ROM, possibly unformatted.");
            ErrorMap.Add("83", "DE_DEST_IS_DVD : The destination is a read-only DVD, possibly unformatted.");
            ErrorMap.Add("84", "DE_DEST_IS_CDRECORD : The destination is a writable CD-ROM, possibly unformatted.");
            ErrorMap.Add("85", "DE_FILE_TOO_LARGE : The file involved in the operation is too large for the destination media or file system.");
            ErrorMap.Add("86", "DE_SRC_IS_CDROM : The source is a read-only CD-ROM, possibly unformatted.");
            ErrorMap.Add("87", "DE_SRC_IS_DVD : The source is a read-only DVD, possibly unformatted.");
            ErrorMap.Add("88", "DE_SRC_IS_CDRECORD : The source is a writable CD-ROM, possibly unformatted.");
            ErrorMap.Add("B7", "DE_ERROR_MAX : MAX_PATH was exceeded during the operation.");
            ErrorMap.Add("402", "DE_ERROR_UNKNOWN : An unknown error occurred. This is typically due to an invalid path in the source or destination. This error does not occur on Windows Vista and later.");
            ErrorMap.Add("10000", "ERRORONDEST : An unspecified error occurred on the destination.");
            ErrorMap.Add("10074", "DE_ROOTDIR | ERRORONDEST : Destination is a root directory and cannot be renamed.");
        }

    }
}
