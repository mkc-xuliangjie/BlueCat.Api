using BuleCat.Common.Extend.ScopeBase;
using System.Collections.Generic;

namespace System.Linq
{
    /// <summary>
    ///   表示一个栈底为固定值的栈
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    public sealed class FixStack<T>
    {

        /// <summary>
        ///  栈是否为空
        /// </summary>
        public bool IsEmpty { get; private set; }

        /// <summary>
        ///   当前
        /// </summary>
        public T Current { get; private set; }

        /// <summary>
        ///   栈底为固定值
        /// </summary>
        public T FixValue { get; private set; }

        /// <summary>
        ///   栈底为固定值,即保证最后栈中总有一个值
        /// </summary>
        /// <remarks>
        ///   当调用了SetDefault后为真
        /// </remarks>
        public bool FixStackBottom { get; private set; }

        private List<T> stackValues = new List<T>();

        /// <summary>
        /// 栈内值
        /// </summary>
        public List<T> Stack
        {
            get
            {
                return stackValues;
            }
        }

        /// <summary>
        /// 构造
        /// </summary>
        public FixStack()
        {
            IsEmpty = true;
        }

        /// <summary>
        /// 清栈
        /// </summary>
        public void Clear()
        {
            using (ThreadLockScope.Scope(this))
            {
                stackValues = new List<T>();
                this.Current = this.FixValue;
            }
        }

        /// <summary>
        /// 栈深
        /// </summary>
        public int StackCount
        {
            get { return stackValues.Count; }
        }

        /// <summary>
        ///   自动转换
        /// </summary>
        /// <param name="stack"> </param>
        /// <returns> </returns>
        public static implicit operator T(FixStack<T> stack)
        {
            return stack.Current;
        }

        /// <summary>
        ///   配置固定值(只第一次调用有效果)
        /// </summary>
        /// <param name="value"> </param>
        public void SetFix(T value)
        {
            using (ThreadLockScope.Scope(this))
            {
                if (Equals(value, default(T)) || this.FixStackBottom)
                {
                    return;
                }
                this.FixStackBottom = true;
                this.FixValue = this.Current = value;
            }
        }

        /// <summary>
        /// 设置配置固定值(只第一次调用有效果)并将栈内所有值替换为它
        /// </summary>
        /// <param name="value"> </param>
        public void SetFixAndReplaceAll(T value)
        {
            using (ThreadLockScope.Scope(this))
            {
                if (Equals(value, default(T)) || this.FixStackBottom)
                {
                    return;
                }
                this.FixStackBottom = true;
                this.FixValue = this.Current = value;
                int cnt = stackValues.Count;
                stackValues.Clear();
                for (int i = 0; i < cnt; i++)
                {
                    stackValues.Add(value);
                }
            }
        }

        /// <summary>
        ///   入栈
        /// </summary>
        /// <param name="value"> </param>
        public void Push(T value)
        {
            using (ThreadLockScope.Scope(this))
            {
                if (Equals(value, default(T)))
                {
                    return;
                }
                this.IsEmpty = false;
                this.stackValues.Add(value);
                this.Current = value;
            }
        }

        /// <summary>
        ///  空入栈
        /// </summary>
        public void PushNull()
        {
            using (ThreadLockScope.Scope(this))
            {
                this.IsEmpty = false;
                this.stackValues.Add(default(T));
                this.Current = default(T);
            }
        }

        /// <summary>
        ///  当前再入栈
        /// </summary>
        /// <remarks>目的是和其它人做相同次数的入栈和出栈</remarks>
        public void PushCurrent()
        {
            using (ThreadLockScope.Scope(this))
            {
                this.IsEmpty = false;
                this.stackValues.Add(this.Current);
            }
        }

        /// <summary>
        ///   出栈
        /// </summary>
        public T Pop()
        {
            using (ThreadLockScope.Scope(this))
            {
                if (this.stackValues.Count == 0)
                {
                    return FixValue;
                }
                this.stackValues.RemoveAt(this.stackValues.Count - 1);
                this.IsEmpty = this.stackValues.Count == 0;
                return this.Current = this.IsEmpty
                    ? this.FixValue
                    : this.stackValues[this.stackValues.Count - 1];
            }
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="value"></param>
        public void Remove(T value)
        {
            using (ThreadLockScope.Scope(this))
            {
                if (this.stackValues != null)
                    this.stackValues.Remove(value);
            }
        }

        /// <summary>
        /// 直接操作Stack后的更新
        /// </summary>
        public void Refresh()
        {
            using (ThreadLockScope.Scope(this))
            {
                this.IsEmpty = this.stackValues.Count == 0;
                this.Current = this.IsEmpty
                    ? this.FixValue
                    : this.stackValues[this.stackValues.Count - 1];
            }
        }

    }


    /// <summary>
    ///   表示一个栈底为固定值的栈
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    public class FixStack2<T>
          where T : struct
    {
        private List<T> stackValues = new List<T>();

        /// <summary>
        ///   当前
        /// </summary>
        public T Current { get; private set; }

        /// <summary>
        ///  栈是否为空
        /// </summary>
        public bool IsEmpty { get; private set; }

        /// <summary>
        ///   固定
        /// </summary>
        public T FixValue { get; private set; }

        /// <summary>
        ///   栈底为固定值,即保证最后栈中总有一个值
        /// </summary>
        /// <remarks>
        ///   当调用了SetDefault后为真
        /// </remarks>
        public bool FixStackBottom { get; private set; }

        /// <summary>
        ///   自动转换
        /// </summary>
        /// <param name="stack"> </param>
        /// <returns> </returns>
        public static implicit operator T(FixStack2<T> stack)
        {
            return stack.Current;
        }

        /// <summary>
        /// 构造
        /// </summary>
        public FixStack2()
        {
            IsEmpty = true;
        }

        /// <summary>
        ///   入栈
        /// </summary>
        /// <param name="value"> </param>
        public void Push(T value)
        {
            using (ThreadLockScope.Scope(this))
            {
                this.IsEmpty = false;
                this.stackValues.Add(value);
                this.Current = value;
            }
        }

        /// <summary>
        ///  当前再入栈
        /// </summary>
        /// <remarks>目的是和其它人做相同次数的入栈和出栈</remarks>
        public void PushCurrent()
        {
            using (ThreadLockScope.Scope(this))
            {
                this.IsEmpty = false;
                this.stackValues.Add(this.Current);
            }
        }
        /// <summary>
        /// 清栈
        /// </summary>
        public void Clear()
        {
            using (ThreadLockScope.Scope(this))
            {
                stackValues = new List<T>();
                this.Current = this.FixValue;
            }
        }
        /// <summary>
        /// 栈深
        /// </summary>
        public int StackCount
        {
            get { return stackValues.Count; }
        }
        /// <summary>
        ///   出栈
        /// </summary>
        public void Pop()
        {
            using (ThreadLockScope.Scope(this))
            {
                if (this.stackValues.Count == 0)
                {
                    return;
                }
                this.stackValues.RemoveAt(this.stackValues.Count - 1);
                this.IsEmpty = this.stackValues.Count == 0;
                this.Current = this.IsEmpty
                    ? this.FixValue
                    : this.stackValues[this.stackValues.Count - 1];
            }
        }
    }
}
