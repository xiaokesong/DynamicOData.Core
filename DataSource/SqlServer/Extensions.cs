using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Formatter.Deserialization;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Query.Expressions;
using Microsoft.AspNet.OData.Query.Validators;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text;
using ODataPath = Microsoft.AspNet.OData.Routing.ODataPath;
using ServiceLifetime = Microsoft.OData.ServiceLifetime;

namespace CN.STOCK.DynamicOData.DataSource.SqlServer
{
    public static class Extensions
    {
        public static bool IsDBNull(this DbDataReader reader, string columnName)
        {
            return Convert.IsDBNull(reader[columnName]);
        }
        internal static string ParseSelect(this ODataQueryOptions options)
        {
            if (options.SelectExpand == null || options.SelectExpand.SelectExpandClause.AllSelected)
            {
                return "*";
            }
            else
            {
                List<string> s = new List<string>();
                PathSelectItem select = null;
                foreach (var item in options.SelectExpand.SelectExpandClause.SelectedItems)
                {
                    select = item as PathSelectItem;
                    if (select != null)
                    {
                        foreach (PropertySegment path in select.SelectedPath)
                        {
                            s.Add(string.Format("[{0}]", path.Property.Name));
                        }
                    }
                }
                return string.Join(",", s);
            }
        }
        internal static string ParseSelect(this ExpandedNavigationSelectItem expanded)
        {
            if (expanded.CountOption.HasValue)
                return "count(*)";
            if (expanded.SelectAndExpand == null)
                return "*";
            if (expanded.SelectAndExpand.AllSelected)
                return "*";
            List<string> s = new List<string>();
            PathSelectItem select = null;
            foreach (var item in expanded.SelectAndExpand.SelectedItems)
            {
                select = item as PathSelectItem;
                if (select != null)
                {
                    foreach (PropertySegment path in select.SelectedPath)
                    {
                        s.Add(string.Format("[{0}]", path.Property.Name));
                    }
                }
            }
            return string.Join(",", s);
        }
        internal static string ParseOrderBy(this ODataQueryOptions options)
        {
            if (options.OrderBy == null)
                return string.Empty;
            return SQLOrderByBinder.BindOrderByQueryOption(options.OrderBy.OrderByClause);

        }
        internal static string ParseOrderBy(this ExpandedNavigationSelectItem expanded)
        {
            if (expanded.CountOption.HasValue)
                return string.Empty;
            if (expanded.OrderByOption == null)
                return string.Empty;
            return SQLOrderByBinder.BindOrderByQueryOption(expanded.OrderByOption);

        }
        internal static string ParseWhere(this ODataQueryOptions options)
        {
            var where = string.Empty;
            if (options.Filter != null)
            {
                where = SQLFilterBinder.BindFilterQueryOption(options.Filter.FilterClause, options.Context.Model);
            }

            var dateTimeStamp = string.Empty;
            if (options.Request.Headers.ContainsKey("DateTimeStamp"))
            {
                var dateTimeStampHeader = options.Request.Headers.FirstOrDefault(r => r.Key.ToLower() == "datetimestamp").Value;
                dateTimeStamp = "CONVERT(BIGINT,DateTimeStamp)>" + dateTimeStampHeader;
            }

            if (string.IsNullOrEmpty(where))
            {
                where = dateTimeStamp;
            }
            else if (!string.IsNullOrEmpty(dateTimeStamp))
            {
                where = string.Format("({0}) and ({1})", dateTimeStamp, where);
            }

            if (!string.IsNullOrEmpty(where))
                where = " where " + where;
            return where;
        }
        internal static string ParseWhere(this ExpandedNavigationSelectItem expanded, string condition, EdmModel model)
        {
            string where = SQLFilterBinder.BindFilterQueryOption(expanded.FilterOption, model);
            if (string.IsNullOrEmpty(where))
            {
                where = condition;
            }
            else if (!string.IsNullOrEmpty(condition))
            {
                where = string.Format("({0}) and ({1})", condition, where);
            }

            if (!string.IsNullOrEmpty(where))
            {
                where = " where " + where;
            }
            return where;
        }
        internal static void SetEntityPropertyValue(this DbDataReader reader, int fieldIndex, EdmStructuredObject entity)
        {
            string name = reader.GetName(fieldIndex);
            if (reader.IsDBNull(fieldIndex))
            {
                entity.TrySetPropertyValue(name, null);
                return;
            }
            if (reader.GetFieldType(fieldIndex) == typeof(DateTime))
            {
                entity.TrySetPropertyValue(name, new DateTimeOffset(reader.GetDateTime(fieldIndex)));
            }
            else
            {
                entity.TrySetPropertyValue(name, reader.GetValue(fieldIndex));
            }
        }

        internal static IEdmType GetEdmType(this ODataPath path)
        {
            return path.Segments[0].EdmType;
        }

        public static Type ToClrType(this EdmPrimitiveTypeKind t)
        {
            switch (t)
            {
                case EdmPrimitiveTypeKind.Binary:
                    break;
                case EdmPrimitiveTypeKind.Boolean:
                    return typeof(bool);
                case EdmPrimitiveTypeKind.Byte:
                    return typeof(Byte);
                case EdmPrimitiveTypeKind.Date:
                    break;
                case EdmPrimitiveTypeKind.DateTimeOffset:
                    return typeof(DateTime);
                case EdmPrimitiveTypeKind.Decimal:
                    return typeof(decimal);
                case EdmPrimitiveTypeKind.Double:
                    return typeof(double);
                case EdmPrimitiveTypeKind.Duration:
                    break;
                case EdmPrimitiveTypeKind.Geography:
                    break;
                case EdmPrimitiveTypeKind.GeographyCollection:
                    break;
                case EdmPrimitiveTypeKind.GeographyLineString:
                    break;
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                    break;
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                    break;
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                    break;
                case EdmPrimitiveTypeKind.GeographyPoint:
                    break;
                case EdmPrimitiveTypeKind.GeographyPolygon:
                    break;
                case EdmPrimitiveTypeKind.Geometry:
                    break;
                case EdmPrimitiveTypeKind.GeometryCollection:
                    break;
                case EdmPrimitiveTypeKind.GeometryLineString:
                    break;
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                    break;
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                    break;
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                    break;
                case EdmPrimitiveTypeKind.GeometryPoint:
                    break;
                case EdmPrimitiveTypeKind.GeometryPolygon:
                    break;
                case EdmPrimitiveTypeKind.Guid:
                    return typeof(Guid);
                case EdmPrimitiveTypeKind.Int16:
                    return typeof(Int16);
                case EdmPrimitiveTypeKind.Int32:
                    return typeof(Int32);
                case EdmPrimitiveTypeKind.Int64:
                    return typeof(Int64);
                case EdmPrimitiveTypeKind.None:
                    break;
                case EdmPrimitiveTypeKind.SByte:
                    break;
                case EdmPrimitiveTypeKind.Single:
                    break;
                case EdmPrimitiveTypeKind.Stream:
                    break;
                case EdmPrimitiveTypeKind.String:
                    return typeof(string);
                case EdmPrimitiveTypeKind.TimeOfDay:
                    break;
                default:
                    break;
            }
            return typeof(object);
        }

        public static object ChangeType(this object v, EdmPrimitiveTypeKind t)
        {
            return v.ChangeType(t.ToClrType());
        }

        public static object ChangeType(this object v, Type t)
        {
            if (v == null || Convert.IsDBNull(v))
                return null;
            else
            {
                try
                {
                    return Convert.ChangeType(v, t);
                }
                catch
                {
                    if (t == typeof(Guid))
                    {
                        Guid g;
                        if (Guid.TryParse(v.ToString(), out g))
                            return g;
                    }
                }
            }
            return null;
        }

    }
}
