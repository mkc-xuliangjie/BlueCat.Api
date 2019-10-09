using System;
using System.Collections.Generic;
using System.Text;

namespace BuleCat.Common.Extend.ScopeBase
{
    public abstract class ScopeBase : IDisposable
    {
        /// <summary>
        ///   记录失败的调用堆栈
        /// </summary>
        protected void RecordFailedStack()
        {
           // LogRecorder.RecordStackTrace(string.Format("在范围对象{0}的析构时,确认范围对象结论为失败,调用堆栈如下:", this.GetType()));
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        protected abstract void OnDispose();

        /// <summary>
        /// 防止多次析构
        /// </summary>
        private bool _isDisposed;
        /// <summary>
        ///   析构
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
                return;
            this.OnDispose();
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
