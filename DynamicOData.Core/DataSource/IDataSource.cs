using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.OData.Edm;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicOData.Core.DataSource
{
    public interface IDataSource
    {
        string Name { get; }
        EdmModel Model { get; }
        GetResult Get(ODataQueryOptions queryOptions);
        //GetResult Get(ODataQueryOptions queryOptions);
        //int GetCount(ODataQueryOptions queryOptions);
        //EdmEntityObject Get(string key, ODataQueryOptions queryOptions);
        //string Create(IEdmEntityObject entity, RequestInfo requestInfo);
        //int Delete(string key, IEdmType elementType, RequestInfo requestInfo);
        //int Merge(string key, IEdmEntityObject entity, RequestInfo requestInfo);
        //int Replace(string key, IEdmEntityObject entity, RequestInfo requestInfo);
        //IEdmObject InvokeFunction(IEdmFunction action, JObject parameterValues, ODataQueryOptions queryOptions = null);
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="action"></param>
        ///// <param name="parameterValues"></param>
        ///// <param name="queryOptions"></param>
        ///// <returns></returns>
        //int GetFuncResultCount(IEdmFunction action, JObject parameterValues, ODataQueryOptions queryOptions);
    }
}
