//using System;
//using System.Web;
//namespace BlueCat.ORM
//{
//    internal class MiniProfilerHttpModule : IHttpModule
//    {
//        public static bool EnableMiniProfiler
//        {
//            get { return false; }
//        }

//        public void Init(HttpApplication context)
//        {
//            if (context == null)
//            {
//                throw new ArgumentNullException("context");
//            }

//            context.BeginRequest += Application_BeginRequest;
//            context.EndRequest += Application_EndRequest;
//        }

//        public void Dispose()
//        {
//        }

//        static void Application_BeginRequest(Object sender, EventArgs e)
//        {
//            if (MiniProfilerHttpModule.EnableMiniProfiler)
//            {
//                MiniProfiler.Start();
//            }
//        }

//        static void Application_EndRequest(Object sender, EventArgs e)
//        {
//            if (MiniProfilerHttpModule.EnableMiniProfiler)
//            {
//                MiniProfiler.Stop();
//            }
//        }
//    }
//}