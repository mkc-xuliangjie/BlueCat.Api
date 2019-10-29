using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BlueCat.Extensions.Helpers
{
    /// <summary>
    /// Class Name：【文件操作类--工具层】
    /// Powered By：General
    /// Create Date：2011-11-8
    /// </summary>
    public static class FileOperations
    {


        #region 创建文件夹【文件夹目录】

        /// <summary>
        /// 创建文件夹【文件夹目录】
        /// </summary>
        /// <param name="FileName">文件夹地址</param>
        public static void GetCreateAFolder(string FileName)
        {
            //判断文件夹是否存在，如果不存在就创建文件夹
            if (!Directory.Exists(FileName))
            {
                //创建文件夹
                Directory.CreateDirectory(FileName);
            }
        }

        #endregion

        #region 取的文件后缀名

        /// <summary>
        /// 取的文件后缀名
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>文件后缀名</returns>
        public static string GetPostfixStr(string filename)
        {
            int start = filename.LastIndexOf(".");
            int length = filename.Length;
            string postfix = filename.Substring(start, length - start);
            return postfix;
        }

        #endregion

        #region 写文件

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="content">文件内容</param>
        public static Task WriteFileAsync(string path, string content)
        {
            return WriteFileAsync(path, content, Encoding.UTF8);
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="data">文件内容</param>
        /// <param name="encoding">编码格式</param>
        public static Task WriteFileAsync(string path, string data, Encoding encoding)
        {
            return WriteFileToByteAsync(path, encoding.GetBytes(data));
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="data">文件内容</param>
        public static async Task WriteFileToByteAsync(string path, byte[] data)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
            {
                await fileStream.WriteAsync(data, 0, data.Length);
                fileStream.Position = 0; //设置当前位置
                fileStream.Close();
                fileStream.Dispose();
            }
        }

        #endregion

        #region 读文件

        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>文件内容</returns>
        public static Task<string> ReadFileAsync(string path)
        {
            return ReadFileAsync(path, Encoding.UTF8);
        }

        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>文件内容</returns>
        public static async Task<string> ReadFileAsync(string path, Encoding encoding)
        {
            string s = "";
            using (StreamReader f2 = new StreamReader(path, encoding))
            {
                s = await f2.ReadToEndAsync();
                f2.Close();
                f2.Dispose();
            }
            return s;
        }

        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>文件内容</returns>
        public static async Task<byte[]> ReadFileToByteAsync(string path)
        {
            byte[] data;
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                data = new byte[fileStream.Length];
                await fileStream.ReadAsync(data, 0, data.Length);
                fileStream.Close();
                fileStream.Dispose();
            }
            return data;
        }

        #endregion

        #region 追加文件

        /// <summary>
        /// 追加文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <param name="strings">内容</param>
        public static async Task FileAdd(string Path, string strings)
        {
            using (StreamWriter sw = System.IO.File.AppendText(Path))
            {
                await sw.WriteAsync(strings);
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
        }

        #endregion

        #region 拷贝文件

        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="OrignFile">原始文件</param>
        /// <param name="NewFile">新文件路径</param>
        public static void FileCoppy(string OrignFile, string NewFile)
        {
            System.IO.File.Copy(OrignFile, NewFile, true);
        }

        #endregion

        #region 删除文件

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="Path">路径</param>
        public static void FileDel(string Path)
        {
            System.IO.File.Delete(Path);
        }

        #endregion

        #region 移动文件

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="OrignFile">原始路径</param>
        /// <param name="NewFile">新路径</param>
        public static void FileMove(string OrignFile, string NewFile)
        {
            System.IO.File.Move(OrignFile, NewFile);
        }

        #endregion

        #region 在当前目录下创建目录

        /// <summary>
        /// 在当前目录下创建目录
        /// </summary>
        /// <param name="OrignFolder">当前目录</param>
        /// <param name="NewFloder">新目录</param>
        public static void FolderCreate(string OrignFolder, string NewFloder)
        {
            Directory.SetCurrentDirectory(OrignFolder);
            Directory.CreateDirectory(NewFloder);
        }

        #endregion

        #region 递归删除文件夹目录及文件

        /// <summary>
        /// 递归删除文件夹目录及文件
        /// </summary>
        /// <param name="dir"></param>  
        /// <returns></returns>
        public static void DeleteFolder(string dir)
        {
            if (Directory.Exists(dir)) //如果存在这个文件夹删除之 
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (System.IO.File.Exists(d))
                        System.IO.File.Delete(d); //直接删除其中的文件                        
                    else
                        DeleteFolder(d); //递归删除子文件夹 
                }
                Directory.Delete(dir, true); //删除已空文件夹                 
            }
        }

        #endregion

        #region 指定文件夹下面的所有内容copy到目标文件夹下面

        /// <summary>
        /// 指定文件夹下面的所有内容copy到目标文件夹下面
        /// </summary>
        /// <param name="srcPath">原始路径</param>
        /// <param name="aimPath">目标文件夹</param>
        public static void CopyDir(string srcPath, string aimPath)
        {
            // 检查目标目录是否以目录分割字符结束如果不是则添加之
            if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
                aimPath += Path.DirectorySeparatorChar;
            // 判断目标目录是否存在如果不存在则新建之
            if (!Directory.Exists(aimPath))
                Directory.CreateDirectory(aimPath);
            // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
            //如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法
            //string[] fileList = Directory.GetFiles(srcPath);
            string[] fileList = Directory.GetFileSystemEntries(srcPath);
            //遍历所有的文件和目录
            foreach (string file in fileList)
            {
                //先当作目录处理如果存在这个目录就递归Copy该目录下面的文件

                if (Directory.Exists(file))
                    CopyDir(file, aimPath + Path.GetFileName(file));
                //否则直接Copy文件
                else
                    System.IO.File.Copy(file, aimPath + Path.GetFileName(file), true);
            }
        }

        #endregion

        #region 获取指定文件夹下所有子目录及文件

        /// <summary>
        /// 获取指定文件夹下所有子目录及文件
        /// </summary>
        /// <param name="Path">详细路径</param>
        public static string GetFoldAll(string Path)
        {
            string str = "";
            DirectoryInfo thisOne = new DirectoryInfo(Path);
            str = ListTreeShow(thisOne, 0, str);
            return str;
        }

        #endregion

        #region 获取指定文件夹下所有子目录及文件函数

        /// <summary>
        /// 获取指定文件夹下所有子目录及文件函数
        /// </summary>
        /// <param name="theDir">指定目录</param>
        /// <param name="nLevel">默认起始值,调用时,一般为0</param>
        /// <param name="Rn">用于迭加的传入值,一般为空</param>
        /// <returns></returns>
        public static string ListTreeShow(DirectoryInfo theDir, int nLevel, string Rn)//递归目录 文件
        {
            DirectoryInfo[] subDirectories = theDir.GetDirectories();//获得目录
            foreach (DirectoryInfo dirinfo in subDirectories)
            {

                if (nLevel == 0)
                {
                    Rn += "├";
                }
                else
                {
                    string _s = "";
                    for (int i = 1; i <= nLevel; i++)
                    {
                        _s += "│&nbsp;";
                    }
                    Rn += _s + "├";
                }
                Rn += "<b>" + dirinfo.Name.ToString() + "</b><br />";
                FileInfo[] fileInfo = dirinfo.GetFiles();   //目录下的文件
                foreach (FileInfo fInfo in fileInfo)
                {
                    if (nLevel == 0)
                    {
                        Rn += "│&nbsp;├";
                    }
                    else
                    {
                        string _f = "";
                        for (int i = 1; i <= nLevel; i++)
                        {
                            _f += "│&nbsp;";
                        }
                        Rn += _f + "│&nbsp;├";
                    }
                    Rn += fInfo.Name.ToString() + " <br />";
                }
                Rn = ListTreeShow(dirinfo, nLevel + 1, Rn);
            }
            return Rn;
        }

        #endregion

        #region 获取文件夹大小

        /// <summary>
        /// 获取文件夹大小
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        /// <returns>文件夹大小</returns>
        public static long GetDirectoryLength(string dirPath)
        {
            if (!Directory.Exists(dirPath))
                return 0;
            long len = 0;
            DirectoryInfo di = new DirectoryInfo(dirPath);
            foreach (FileInfo fi in di.GetFiles())
            {
                len += fi.Length;
            }
            DirectoryInfo[] dis = di.GetDirectories();
            if (dis.Length > 0)
            {
                for (int i = 0; i < dis.Length; i++)
                {
                    len += GetDirectoryLength(dis[i].FullName);
                }
            }
            return len;
        }

        #endregion

        #region 获取指定文件详细属性

        /// <summary>
        /// 获取指定文件详细属性
        /// </summary>
        /// <param name="filePath">文件详细路径</param>
        /// <returns>文件详细属性</returns>
        public static string GetFileAttibeToString(string filePath)
        {
            System.IO.FileInfo objFI = new System.IO.FileInfo(filePath);
            var str = $"详细路径:{ objFI.FullName }<br/>文件名称:{ objFI.Name }<br/>文件长度:{ objFI.Length.ToString() }字节<br/>创建时间{ objFI.CreationTime.ToString() }<br/>最后访问时间:{ objFI.LastAccessTime.ToString() }<br/>修改时间:{ objFI.LastWriteTime.ToString() }<br/>所在目录:{ objFI.DirectoryName }<br/>扩展名:{ objFI.Extension}";
            return str;
        }

        /// <summary>
        /// 获取指定文件详细属性
        /// </summary>
        /// <param name="filePath">文件详细路径</param>
        /// <returns>文件详细属性</returns>
        public static Dictionary<string, string> GetFileAttibe(string filePath)
        {
            System.IO.FileInfo objFI = new System.IO.FileInfo(filePath);
            var dic = new Dictionary<string, string>();
            dic.Add("详细路径", objFI.FullName);
            dic.Add("文件名称", objFI.Name);
            dic.Add("文件长度", objFI.Length.ToString());
            dic.Add("创建时间", objFI.CreationTime.ToString());
            dic.Add("最后访问时间", objFI.LastAccessTime.ToString());
            dic.Add("修改时间", objFI.LastWriteTime.ToString());
            dic.Add("所在目录", objFI.DirectoryName);
            dic.Add("扩展名", objFI.Extension);
            return dic;
        }

        #endregion
    }
}
