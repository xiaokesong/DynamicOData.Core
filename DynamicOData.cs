using DynamicOData.Core.DataSource;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicOData.Core
{
    public static class DynamicOData
    {
        public static void AddDataSource(IDataSource dataSource)
        {
            DataSourceProvider.AddDataSource(dataSource);
        }
        //public static Action<RequestInfo> BeforeExcute;
    }
}
