use MyWeb
select * from SysAccount
declare @RowCount int,@PageCount int 
exec  Proc_Paging 'SysAccount','Id',1,10,'',@RowCount output,@PageCount  output
alter PROCEDURE SP_Paging
(
@TableName nvarchar(30),--表名称
@IDName nvarchar(20),--表主键名称
@PageIndex int = 1,--当前页数 
@PageSize  int = 10,--每页大小 
@Where nvarchar(255) = '',--wehre查询条件
@RowCount int output,--总行数(传出参数)
@PageCount int output--总页数(传出参数)
)
AS
IF @PageIndex > 0
BEGIN
	SET NOCOUNT ON --使返回的结果中不包含有关受 Transact-SQL 语句影响的行数的信息。 
		DECLARE @PageLowerBound INT,@StartID INT,@sql NVARCHAR(225),@sqlCount NVARCHAR(225)
		--SET @sql=N'set @RowCount = (SELECT  count('+@IDName+') FROM '+@TableName+')'
		--EXEC sp_executesql @sql,N'@RowCount int output',@RowCount OUTPUT 
--获取总行数和总页数
		SET @sqlCount = 'SELECT @RowCount=COUNT(*),@PageCount=CEILING((COUNT(*)+0.0)/'+ CAST(@PageSize AS VARCHAR)+') FROM ' + @TableName
		IF len(@Where)>1
		BEGIN
		   set @sqlCount=@sqlCount+' WHERE '+@Where
		END
		--系统扩展存储过程（参数化的sql,参数列表，参数）
        EXEC SP_EXECUTESQL @sqlCount,N'@RowCount INT OUTPUT,@PageCount INT OUTPUT',@RowCount OUTPUT,@PageCount OUTPUT
--找出启始Id
		SET @PageLowerBound = @PageSize * (@PageIndex-1)
		IF @PageLowerBound<1
			SET @PageLowerBound=1
		SET ROWCOUNT @PageLowerBound --设置查询前几行数据
		SET @sql=N'SELECT  @StartID = ['+@IDName+'] FROM '+@TableName
--拼接where条件
		IF len(@Where)>1
		BEGIN
		   set @sql=@sql+' WHERE '+@Where
		END
        SET @sql=@sql+' ORDER BY '+@IDName
		
	    EXEC sp_executesql @sql,N'@StartID int output',@StartID output
--拼接sql	    
		SET ROWCOUNT 0 --恢复查询所有
		SET @sql='SELECT TOP '+str(@PageSize) +' * FROM '+@TableName+' WHERE ['+@IDName+']>='+ str(@StartID)
--拼接where条件
		IF LEN(@Where)>1
		BEGIN
			set @sql=@sql+' AND '+@Where
		END
--排序		
		SET @sql=@sql +' ORDER BY ['+@IDName+']' 
		EXEC(@sql)
	SET NOCOUNT OFF
END

declare @StartID varchar(100)
select @StartID=Name from sysAccount order by name
select @StartID