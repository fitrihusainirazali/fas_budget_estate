USE [BFASOPMSESTSEMENANJUNG]
GO
/****** Object:  StoredProcedure [dbo].[sp_BudgetExpensesCapitalList]    Script Date: 16/5/2023 4:37:22 PM ******/
DROP PROCEDURE [dbo].[sp_BudgetExpensesCapitalList]
GO
/****** Object:  StoredProcedure [dbo].[sp_BudgetExpensesCapitalList]    Script Date: 16/5/2023 4:37:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Muhammad Hazim
-- Create date: 29/3/2023
-- Description:	Get Budget Expenses Capital List group by Cost Center
-- =============================================
CREATE PROCEDURE [dbo].[sp_BudgetExpensesCapitalList] 
	-- Add the parameters for the stored procedure here
	@year varchar(4) = null,
	@costCenter varchar(255) = null,
	@syarikatId varchar(11) = null,
	@ladangId varchar(11) = null,
	@wilayahId varchar(11) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @sql VARCHAR(8000)
	SET @sql = '
		SELECT s.ScrNameLongDesc AS ScreenName, c.abce_budgeting_year AS BudgetYear, c.abce_cost_center_code AS CostCenterCode, c.abce_cost_center_desc AS CostCenterDesc, SUM(c.abce_total) AS Total
		FROM bgt_expenses_capital c, BFASOPMSCORP.dbo.bgt_Screen s
		WHERE c.abce_deleted = ''0''
		AND s.ScrCode = ''E9'''

	IF (@year IS NOT NULL AND @year <> '')
	BEGIN
		SET @sql = @sql + ' AND c.abce_budgeting_year = ''' + @year + ''''
	END

	IF (@costCenter IS NOT NULL AND @costCenter <> '')
	BEGIN
		SET @sql = @sql + ' AND (c.abce_cost_center_code LIKE ''%' + @costCenter + '%'' OR c.abce_cost_center_desc LIKE ''%' + @costCenter + '%'')'
	END

	IF (@syarikatId IS NOT NULL AND @syarikatId <> '')
	BEGIN
		SET @sql = @sql + ' AND c.abce_syarikat_id = ''' + LTRIM(RTRIM(@syarikatId)) + ''''
	END

	IF (@ladangId IS NOT NULL AND @ladangId <> '')
	BEGIN
		SET @sql = @sql + ' AND c.abce_ladang_id = ''' + LTRIM(RTRIM(@ladangId)) + ''''
	END

	IF (@wilayahId IS NOT NULL AND @wilayahId <> '')
	BEGIN
		SET @sql = @sql + ' AND c.abce_wilayah_id = ''' + LTRIM(RTRIM(@wilayahId)) + ''''
	END

	SET @sql = @sql + ' 
		GROUP BY s.ScrNameLongDesc, c.abce_budgeting_year, c.abce_cost_center_code, c.abce_cost_center_desc
		ORDER BY c.abce_budgeting_year DESC, c.abce_cost_center_code'

	EXEC(@sql)
END
GO
