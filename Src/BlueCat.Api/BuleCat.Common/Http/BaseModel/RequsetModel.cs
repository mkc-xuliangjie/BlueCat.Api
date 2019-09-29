
using System.ComponentModel.DataAnnotations;

namespace BuleCat.Common
{
    public class RequsetModel
    {
        /// <summary>
        /// 请求的应用名称
        /// </summary>
        [Required]
        public string Application { get; set; }

        /// <summary>
        /// 请求的数据
        /// </summary>
        [Required]
        public string Data { get; set; }

        /// <summary>
        /// 当前用户（若有验证）
        /// </summary>
        public string Account { get; set; }
    }

    public class RequsetModel<T> : RequsetModel where T : class, new()
    {
        /// <summary>
        /// 业务数据，将 <see cref="RequsetModel.Data"/> 反序列化为指定的对象 <typeparamref name="T"/>, 若不能反序列化，将设置为 <code>default(T)</code>
        /// </summary>
        public T BusinessData { get; set; }
    }
}
