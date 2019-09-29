using System.ComponentModel;

namespace BlueCat.Contract
{
    public class ResponseStatusCode
    {
        /// 签名错误
        /// </summary>
        [Description("签名错误")]
        public static string SignError = "0001";

        /// <summary>
        /// 成功
        /// </summary> 
        [Description("成功")]
        public static string Success = "0000";

        /// <summary>
        /// 服务器错误
        /// </summary> 
        [Description("服务器错误")]
        public static string Error = "4001";
        /// <summary>
        /// 没找到用户
        /// </summary>
        [Description("没找到用户")]
        public static string Not_Found_User = "4101";

        /// <summary>
        /// 无效 token
        /// </summary>
        [Description("无效 token")]
        public static string Auth_Invalid_Token = "4102";

        /// <summary>
        /// 过期 token
        /// </summary>
        [Description("过期 token")]
        public static string Auth_Expired_Token = "4103";

        /// <summary>
        /// 参数无效
        /// </summary>
        [Description("参数无效")]
        public static string Argument_Invalid = "4401";

        /// <summary>
        /// 无效的签名
        /// </summary>
        [Description("无效的签名")]
        public static string Sign_Invalid = "4410";

        /// <summary>
        /// 第三方服务响应出错
        /// </summary>
        [Description("第三方服务响应出错")]
        public static string Response_ThirdServices_Error = "4501";

        /// <summary>
        /// 解析响应的 Json 结果出错
        /// </summary>
        [Description("解析响应的 Json 结果出错")]
        public static string Response_ResolveResultJsonData_Error = "4502";

        /// <summary>
        /// 上传文件到本地出错
        /// </summary>
        [Description("上传文件到本地出错")]
        public static string Upload_To_Local_Error = "4521";

        /// <summary>
        /// 上传文件到远程服务器出错
        /// </summary>
        [Description("上传文件到远程服务器出错")]
        public static string Upload_To_Remote_Error = "4522";

        /// <summary>
        /// 上传文件到 CDN 出错
        /// </summary>
        [Description("上传文件到 CDN 出错")]
        public static string Upload_To_CDN_Error = "4523";

        /// <summary>
        /// 没有在版本映射文件中找到指定的事件
        /// </summary>
        [Description("没有在版本映射文件中找到指定的事件")]
        public static string Internal_Not_Found_Event_In_MappingFile_Error = "5101";

        /// <summary>
        /// 数据部分成功
        /// </summary>
        [Description("数据部分成功")]
        public static string ImportExcel_Part_Success = "3000";
        /// <summary>
        /// 导出Excel数据太大
        /// </summary>
        [Description("导出Excel数据太大")]
        public static string Excel_DataMax_Invalid = "4420";
    }
}
