using Microsoft.AspNet.OData;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicOData.Core.DataSource
{
    public class GetResult
    {
        public EdmEntityObjectCollection Collection { get; set; }

        public int Count { get; set; }
    }
}
