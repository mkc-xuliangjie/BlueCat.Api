
using System;
using System.Text;

namespace BlueCat.RabbitMQ
{
    /// <summary>
    /// Class Utf8JsonRabbitMqSerializer.
    /// </summary>
    /// <seealso cref="BlueCat.RabbitMQ.IRabbitMqSerializer" />
    /// <remarks>
    /// <para>作者    :jason</para>	
    /// <para>创建时间:2018-12-19</para>
    /// <para>最后更新:jason</para>	
    /// <para>更新时间:2018-12-19</para>
    /// </remarks>
    public class Utf8JsonRabbitMqSerializer : IRabbitMqSerializer
    {


        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.Byte[].</returns>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public byte[] Serialize(object obj)
        {
            return Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
        }

        /// <summary>
        /// Deserializes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public object Deserialize(byte[] value, Type type)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(Encoding.UTF8.GetString(value), type);
        }
    }
}