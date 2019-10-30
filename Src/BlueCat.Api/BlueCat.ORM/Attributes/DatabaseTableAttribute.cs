using System;

namespace BlueCat.ORM.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DatabaseTableAttribute : Attribute
    {
        private string m_Name;

        public DatabaseTableAttribute(string name)
        {
            this.m_Name = name;
        }

        public string Name
        {
            get
            {
                return this.m_Name;
            }
        }
    }
}

