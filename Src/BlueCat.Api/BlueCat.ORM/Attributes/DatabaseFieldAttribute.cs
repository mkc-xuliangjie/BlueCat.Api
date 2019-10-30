using System;
using System.Data;

namespace BlueCat.ORM.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DatabaseFieldAttribute : Attribute
    {
        private bool _IsComplexField;
        private string m_Description = "";
        private bool m_IsEncryptField = false;
        private bool m_isIdentity = false;
        private bool m_isPK = false;
        private int m_length = 4;
        private string m_Name;
        private DbType m_Type = DbType.String;

        public DatabaseFieldAttribute(string Name)
        {
            this.m_Name = Name;
        }

        public string Description
        {
            get
            {
                return this.m_Description;
            }
            set
            {
                this.m_Description = value;
            }
        }

        public bool IsComplexField
        {
            get
            {
                return this._IsComplexField;
            }
            set
            {
                this._IsComplexField = value;
            }
        }

        public bool IsEncryptField
        {
            get
            {
                return this.m_IsEncryptField;
            }
            set
            {
                this.m_IsEncryptField = value;
            }
        }

        public bool IsIdentity
        {
            get
            {
                return this.m_isIdentity;
            }
            set
            {
                this.m_isIdentity = value;
            }
        }

        public bool IsPK
        {
            get
            {
                return this.m_isPK;
            }
            set
            {
                this.m_isPK = value;
            }
        }

        public int Length
        {
            get
            {
                return this.m_length;
            }
            set
            {
                this.m_length = value;
            }
        }

        public string Name
        {
            get
            {
                return this.m_Name;
            }
        }

        public DbType Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
            }
        }
    }
}

