using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection.Emit;

namespace BlueCat.ORM
{
    [Serializable]
    public abstract class ModelBase
    {
        private List<string> _ChangedPropertys = new List<string>();
        private bool _IgnorePropertyChange = false;
        protected string tableName;

        private static PropertyChangingEventArgs EmptyChangingEventArgs = new PropertyChangingEventArgs(string.Empty);

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

        protected abstract TableSchema Schema { get; }

        public ModelBase()
        {
        }

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

        internal List<string> ChangedPropertys
        {
            get
            {
                return this._ChangedPropertys;
            }
        }

        private static Dictionary<string, DynamicMethodProxyHandler> cache = new Dictionary<string, DynamicMethodProxyHandler>();
        private static readonly object cacheLocker = new object();
        private static DynamicMethodFactory dymFac = new DynamicMethodFactory();
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

        internal bool IgnorePropertyChange
        {
            get
            {
                return this._IgnorePropertyChange;
            }
        }

        internal void SetIgnoreChange(bool isIgnore)
        {
            this._IgnorePropertyChange = isIgnore;
        }


        internal string GetTableName()
        {
            return Schema.GetTableName();
        }
    }
}
