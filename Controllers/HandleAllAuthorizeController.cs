// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using CN.STOCK.DynamicOData.DataSource;
using CN.STOCK.DynamicOData.Extensions;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OData.Edm;
using ODataPath = Microsoft.AspNet.OData.Routing.ODataPath;

namespace CN.STOCK.DynamicOData.Controllers
{
    public class HandleAllAuthorizeController : ODataController
    {
        [Authorize]
        public EdmEntityObjectCollection Get()
        {
            string dsName = Request.GetDataSource();
            var ds = DataSourceProvider.GetDataSource(dsName);
            var options = BuildQueryOptions();
            EdmEntityObjectCollection rtv = null;

            if (options.SelectExpand != null)
                Request.ODataFeature().SelectExpandClause = options.SelectExpand.SelectExpandClause;

            GetResult getResult = ds.Get(options);
            rtv = getResult.Collection;
            if (options.Count != null && options.Count.Value)
            {
                Request.ODataFeature().TotalCount = getResult.Count;
            }

            return rtv;
        }

        private ODataQueryOptions BuildQueryOptions()
        {
            ODataPath path = Request.ODataFeature().Path;
            IEdmType edmType = path.Segments[0].EdmType;
            IEdmType elementType = edmType.TypeKind == EdmTypeKind.Collection ? (edmType as IEdmCollectionType).ElementType.Definition : edmType;
            ODataQueryContext queryContext = new ODataQueryContext(Request.GetModel(), elementType, path);
            ODataQueryOptions queryOptions = new ODataQueryOptions(queryContext, Request);
            return queryOptions;
        }

    }
}
