USE [BFASOPMSESTSEMENANJUNG]
GO
/****** Object:  StoredProcedure [dbo].[sp_BudgetExpensesOthersView]    Script Date: 16/5/2023 4:37:22 PM ******/
DROP PROCEDURE [dbo].[sp_BudgetExpensesOthersView]
GO
/****** Object:  StoredProcedure [dbo].[sp_BudgetExpensesOthersView]    Script Date: 16/5/2023 4:37:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Muhammad Hazim
-- Create date: 17/4/2023
-- Description:	Budget - Expenses Others View
-- =============================================
CREATE PROCEDURE [dbo].[sp_BudgetExpensesOthersView] 
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
		SELECT s.ScrNameLongDesc as ScreenName, o.*
		FROM bgt_expenses_others o, BFASOPMSCORP.dbo.bgt_Screen s
		WHERE o.abeo_deleted = ''0''
		AND s.ScrCode = ''E15'''

	IF (@year IS NOT NULL AND @year <> '')
	BEGIN
		SET @sql = @sql + ' AND o.abeo_budgeting_year = ''' + @year + ''''
	END

	IF (@costCenter IS NOT NULL AND @costCenter <> '')
	BEGIN
		SET @sql = @sql + ' AND (o.abeo_cost_center_code LIKE ''%' + @costCenter + '%'' OR o.abeo_cost_center_desc LIKE ''%' + @costCenter + '%'')'
	END

	IF (@syarikatId IS NOT NULL AND @syarikatId <> '')
	BEGIN
		SET @sql = @sql + ' AND o.abce_syarikat_id = ''' + LTRIM(RTRIM(@syarikatId)) + ''''
	END

	IF (@ladangId IS NOT NULL AND @ladangId <> '')
	BEGIN
		SET @sql = @sql + ' AND o.abce_ladang_id = ''' + LTRIM(RTRIM(@ladangId)) + ''''
	END

	IF (@wilayahId IS NOT NULL AND @wilayahId <> '')
	BEGIN
		SET @sql = @sql + ' AND o.abce_wilayah_id = ''' + LTRIM(RTRIM(@wilayahId)) + ''''
	END

	SET @sql = @sql + ' ORDER BY o.abeo_budgeting_year DESC, o.abeo_cost_center_code, o.abeo_gl_expenses_code'

	EXEC(@sql)
END
GO
