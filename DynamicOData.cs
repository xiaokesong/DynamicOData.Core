using CN.STOCK.DynamicOData.DataSource;
using System;
using System.Collections.Generic;
using System.Text;

namespace CN.STOCK.DynamicOData
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
