# DynamicOData.Core

用于支持表和视图的动态 OData

使用前提：

    1、基于.NET Core
    2、目前支持 SqlServer2016 及以上
    3、必须创建 SysGetEdmModelInfo 存储过程
    4、支持 Odata 表和视图的查询

搭建示例：

    1、创建示例数据库，参照 Demo.sql

    2、创建.NET Core 的 WEBAPI 项目，并安装 DynamicOData.Core：
    	dotnet add package DynamicOData.Core

    3、在 WebApi 项目 Startup.cs 文件 ConfigureServices 方法中修改 services.AddMvc 如下：
    	services.AddMvc(options =>
    	{
    		options.EnableEndpointRouting = false;
    	}).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

    4、在WebApi项目Startup.cs文件ConfigureServices方法中添加如下：
    	services.AddOData();
    	services.AddODataQueryFilter();

    5、在WebApi项目Startup.cs文件Configure方法中修改app.UseMvc如下（将connectStr替换为你对应的数据库连接字符串）：
    	app.UseMvc(routeBuilder =>
    	{
    		routeBuilder.CustomMapODataServiceRoute("odata", "odata/{dataSource}");
    		string connectStr = "Server=.;Initial Catalog=DynamicOData.Core.Demo;Persist Security Info=False;User ID=sa;Password=Password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";
    		var sqlSource = new SQLDataSource(Constants.SqlServerDataSource, connectStr);
    		DynamicOData.Core.DynamicOData.AddDataSource(sqlSource);
    	});

使用示例（将https://localhost:44378替换为你对应的URL地址）：

    1、OData-比较-等于-eq
        https://localhost:44378/odata/db/SysUser?$filter=UserName eq 'zhangsan'

    2、OData-比较-不等于-ne
    	https://localhost:44378/odata/db/SysUser?$filter=UserName ne 'zhangsan'

    3、OData-比较-大于-gt
    	https://localhost:44378/odata/db/SysUser?$filter=Age gt 30

    4、OData-比较-大于等于-ge
    	https://localhost:44378/odata/db/SysUser?$filter=Age ge 30

    5、OData-比较-小于-lt
    	https://localhost:44378/odata/db/SysUser?$filter=Age lt 30

    6、OData-比较-小于等于-le
    	https://localhost:44378/odata/db/SysUser?$filter=Age le 30

    7、OData-逻辑-与-and
    	https://localhost:44378/odata/db/SysUser?$filter=Age le 30 and UserName eq 'zhangsan'

    8、OData-逻辑-或-or
    	https://localhost:44378/odata/db/SysUser?$filter=Age le 30 or UserName eq 'zhangsan'

    9、OData-函数-模糊查询-contains
    	https://localhost:44378/odata/db/SysUser?$filter=contains(UserName,'a')

    10、OData-系统-排序-$orderby
    	https://localhost:44378/odata/db/SysUser?$orderby=Age desc

    11、OData-系统-限制数量-$top
    	https://localhost:44378/odata/db/SysUser?$top=2

    12、OData-系统-跳过数量-$skip
    	https://localhost:44378/odata/db/SysUser?$top=2&$skip=2&$orderby=Age

    13、OData-系统-查询总数-$count
    	https://localhost:44378/odata/db/SysUser?$count=true
