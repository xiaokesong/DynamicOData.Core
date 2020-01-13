using Microsoft.AspNet.OData;
using System;
using System.Collections.Generic;
using System.Text;

namespace CN.STOCK.DynamicOData.DataSource
{
    public class GetResult
    {
        public EdmEntityObjectCollection Collection { get; set; }

        public int Count { get; set; }
    }
}
