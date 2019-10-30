using System.ComponentModel;
//using Microsoft.Web.Infrastructure.DynamicModuleHelper;

namespace BlueCat.ORM
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class PreApplicationStartCode
    {
        private static bool _startWasCalled;

        public static void Start()
        {
            if (_startWasCalled) return;

            _startWasCalled = true;
            //DynamicModuleUtility.RegisterModule(typeof(MiniProfilerHttpModule));
        }
    }
}
