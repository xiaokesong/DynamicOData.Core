CREATE DATABASE [DynamicOData.Core.Demo]
GO

USE [DynamicOData.Core.Demo]
GO

-- =============================================
-- Author:		Xiao Ke Song
-- Create date: 2020-01-14
-- Description:	必须的存储过程，用于获取数据库表和视图信息
-- =============================================
CREATE PROCEDURE [dbo].[SysGetEdmModelInfo] 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT 
		c.TABLE_NAME
		,c.COLUMN_NAME
		,(CASE WHEN c.DATA_TYPE='timestamp' THEN 'bigint' ELSE c.DATA_TYPE END) AS DATA_TYPE
		,c.IS_NULLABLE
		,k.COLUMN_NAME AS KEY_COLUMN_NAME
	FROM INFORMATION_SCHEMA.COLUMNS AS c
		LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS k ON 
			OBJECTPROPERTY(OBJECT_ID(k.CONSTRAINT_NAME), 'IsPrimaryKey')=1
			AND k.COLUMN_NAME=c.COLUMN_NAME 
			AND k.TABLE_NAME=c.TABLE_NAME
			--WHERE c.DATA_TYPE<>'timestamp'
	ORDER BY c.TABLE_NAME
END


GO

CREATE TABLE [dbo].[SysUser](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[RealName] [nvarchar](50) NOT NULL,
	[Age] [int] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [SysUser]
VALUES
('zhangsan', '张三', 20, '2018-01-01' ), 
('lisi', '李四', 30, '2019-01-01' ), 
('wangwu', '王五', 40, '2020-01-01' )
