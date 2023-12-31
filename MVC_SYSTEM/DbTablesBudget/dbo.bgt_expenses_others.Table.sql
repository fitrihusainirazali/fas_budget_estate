USE [BFASOPMSESTSEMENANJUNG]
GO
ALTER TABLE [dbo].[bgt_expenses_others] DROP CONSTRAINT [DF__bgt_expen__last___0539C240]
GO
ALTER TABLE [dbo].[bgt_expenses_others] DROP CONSTRAINT [DF__bgt_expen__abeo___04459E07]
GO
ALTER TABLE [dbo].[bgt_expenses_others] DROP CONSTRAINT [DF__bgt_expen__abeo___035179CE]
GO
ALTER TABLE [dbo].[bgt_expenses_others] DROP CONSTRAINT [DF__bgt_expen__abeo___025D5595]
GO
/****** Object:  Table [dbo].[bgt_expenses_others]    Script Date: 13/4/2023 2:31:11 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bgt_expenses_others]') AND type in (N'U'))
DROP TABLE [dbo].[bgt_expenses_others]
GO
/****** Object:  Table [dbo].[bgt_expenses_others]    Script Date: 13/4/2023 2:31:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[bgt_expenses_others](
	[abeo_id] [int] IDENTITY(1,1) NOT NULL,
	[abeo_revision] [int] NOT NULL,
	[abeo_budgeting_year] [int] NOT NULL,
	[abeo_syarikat_id] [int] NULL,
	[abeo_syarikat_name] [nvarchar](100) NULL,
	[abeo_ladang_id] [int] NULL,
	[abeo_ladang_code] [nvarchar](5) NULL,
	[abeo_ladang_name] [nvarchar](50) NULL,
	[abeo_wilayah_id] [int] NULL,
	[abeo_wilayah_name] [nvarchar](50) NULL,
	[abeo_company_category] [nvarchar](50) NULL,
	[abeo_company_code] [nvarchar](50) NULL,
	[abeo_company_name] [nvarchar](100) NULL,
	[abeo_cost_center_code] [nvarchar](15) NULL,
	[abeo_cost_center_desc] [nvarchar](100) NULL,
	[abeo_gl_expenses_code] [nvarchar](20) NULL,
	[abeo_gl_expenses_name] [nvarchar](100) NULL,
	[abeo_note] [nvarchar](100) NULL,
	[abeo_unit_1] [decimal](18, 2) NULL,
	[abeo_uom_1] [nvarchar](25) NULL,
	[abeo_operation_1] [nvarchar](25) NULL,
	[abeo_unit_2] [decimal](18, 2) NULL,
	[abeo_uom_2] [nvarchar](25) NULL,
	[abeo_operation_2] [nvarchar](25) NULL,
	[abeo_unit_3] [decimal](18, 2) NULL,
	[abeo_uom_3] [nvarchar](25) NULL,
	[abeo_operation_3] [nvarchar](25) NULL,
	[abeo_rate] [decimal](18, 2) NULL,
	[abeo_total] [decimal](18, 2) NULL,
	[abeo_month_1] [decimal](18, 2) NULL,
	[abeo_month_2] [decimal](18, 2) NULL,
	[abeo_month_3] [decimal](18, 2) NULL,
	[abeo_month_4] [decimal](18, 2) NULL,
	[abeo_month_5] [decimal](18, 2) NULL,
	[abeo_month_6] [decimal](18, 2) NULL,
	[abeo_month_7] [decimal](18, 2) NULL,
	[abeo_month_8] [decimal](18, 2) NULL,
	[abeo_month_9] [decimal](18, 2) NULL,
	[abeo_month_10] [decimal](18, 2) NULL,
	[abeo_month_11] [decimal](18, 2) NULL,
	[abeo_month_12] [decimal](18, 2) NULL,
	[abeo_proration] [nvarchar](25) NULL,
	[abeo_history] [bit] NOT NULL,
	[abeo_status] [nvarchar](50) NULL,
	[abeo_created_by] [int] NULL,
	[abeo_deleted] [bit] NOT NULL,
	[last_modified] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[abeo_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[bgt_expenses_others] ADD  DEFAULT ((0)) FOR [abeo_revision]
GO
ALTER TABLE [dbo].[bgt_expenses_others] ADD  DEFAULT ((0)) FOR [abeo_history]
GO
ALTER TABLE [dbo].[bgt_expenses_others] ADD  DEFAULT ((0)) FOR [abeo_deleted]
GO
ALTER TABLE [dbo].[bgt_expenses_others] ADD  DEFAULT (getdate()) FOR [last_modified]
GO
