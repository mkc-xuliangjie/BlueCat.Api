using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection.Emit;
using System.Data;
using BlueCat.ORM.Attributes;
using System.Reflection;

namespace BlueCat.ORM
{
    /// <summary>
    /// 数据模型基类
    /// </summary>
    /// <remarks>
    ///  	<para>创建：Teddy</para>
    ///  	<para>日期：2016-10-17</para>
    /// </remarks>
    [Serializable]
    public abstract class EntityBase
    {
        /// <summary>
        /// The _ changed propertys
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        private List<string> _ChangedPropertys = new List<string>();
        /// <summary>
        /// The _ ignore property change
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        private bool _IgnorePropertyChange = false;
        /// <summary>
        /// The table name
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        protected string tableName;

        /// <summary>
        /// The empty changing event 参数列表
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        private static PropertyChangingEventArgs EmptyChangingEventArgs = new PropertyChangingEventArgs(string.Empty);

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when [property changing].
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        public event PropertyChangingEventHandler PropertyChanging;


        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <value>The schema.</value>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        protected abstract TableSchema Schema { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityBase"/> class.
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        public EntityBase()
        {
        }

        /// <summary>
        /// Sends the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        protected void SendPropertyChanged(string propertyName)
        {
            if (!this.IgnorePropertyChange)
            {
                if (!this.ChangedPropertys.Contains(propertyName))
                {
                    this.ChangedPropertys.Add(propertyName);
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        /// <summary>
        /// Sends the property changing.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        protected void SendPropertyChanging(string propertyName)
        {
            if (!this.IgnorePropertyChange)
            {
                if (PropertyChanging != null)
                {
                    PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
                }
            }
        }

        /// <summary>
        /// Gets the changed propertys.
        /// </summary>
        /// <value>The changed propertys.</value>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        internal List<string> ChangedPropertys
        {
            get
            {
                return this._ChangedPropertys;
            }
        }

        /// <summary>
        /// 缓存
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        private static Dictionary<string, DynamicMethodProxyHandler> cache = new Dictionary<string, DynamicMethodProxyHandler>();
        /// <summary>
        /// 缓存锁定对象
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        private static readonly object cacheLocker = new object();
        /// <summary>
        /// The dym fac
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        private static DynamicMethodFactory dymFac = new DynamicMethodFactory();
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>EntityValue.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        internal EntityValue GetValue(string propertyName)
        {
            var key = this.GetType().FullName + "_" + propertyName;
            DynamicMethodProxyHandler handler;
            if (cache.ContainsKey(key))
            {
                handler = cache[key];
            }
            else
            {
                lock (cacheLocker)
                {
                    if (cache.ContainsKey(key))
                    {
                        handler = cache[key];
                    }
                    else
                    {
                        var property = this.GetType().GetProperty(propertyName);

                        handler = dymFac.GetPropertyGetMethodDelegate(property);
                        cache.Add(key, handler);
                    }
                }
            }
            var entityValue = new EntityValue();
            var column = this.Schema.GetColumn(this.GetTableName() + "." + propertyName);
            entityValue.Column = column;
            entityValue.Value = handler(this, null);
            return entityValue;
        }

        /// <summary>
        /// Gets a value indicating whether [ignore property change].
        /// </summary>
        /// <value><c>是</c> if [ignore property change]; otherwise, <c>否</c>.</value>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        internal bool IgnorePropertyChange
        {
            get
            {
                return this._IgnorePropertyChange;
            }
        }

        /// <summary>
        /// Sets the ignore change.
        /// </summary>
        /// <param name="isIgnore">if set to <c>是</c> [is ignore].</param>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        internal void SetIgnoreChange(bool isIgnore)
        {
            this._IgnorePropertyChange = isIgnore;
        }


        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <returns>System.String.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        internal string GetTableName()
        {
            return Schema.GetTableName();
        }
    }


    /// <summary>
    /// 数据字段值
    /// </summary>
    /// <remarks>
    ///  	<para>创建：Teddy</para>
    ///  	<para>日期：2016-10-17</para>
    /// </remarks>
    public class EntityValue
    {
        /// <summary>
        /// Query字段
        /// </summary>
        /// <value>The column.</value>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        public QueryColumn Column { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        /// <value>The value.</value>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        public object Value { get; set; }
    }
}
