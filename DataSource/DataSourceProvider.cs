// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using CN.STOCK.DynamicOData.DataSource.SqlServer;
using Microsoft.AspNet.OData;
using Microsoft.OData.Edm;

namespace CN.STOCK.DynamicOData.DataSource
{
    internal class DataSourceProvider
    {
        static Dictionary<string, IDataSource> DataSource = new Dictionary<string, IDataSource>();
        public static void AddDataSource(IDataSource dataSource)
        {
            DataSource.Add(dataSource.Name, dataSource);
        }

        public static IEdmModel GetEdmModel(string dataSourceName)
        {
            return GetDataSource(dataSourceName).Model;
        }

        public static IDataSource GetDataSource(string dataSourceName)
        {
            dataSourceName = dataSourceName == null ? string.Empty : dataSourceName.ToLowerInvariant();
            IDataSource ds = null;
            if (DataSource.TryGetValue(dataSourceName, out ds))
                return ds;
            throw new InvalidOperationException(
                string.Format("Data source: {0} is not registered.", dataSourceName));

        }
    }
}
