USE [BFASOPMSESTSEMENANJUNG]
GO
ALTER TABLE [dbo].[bgt_expenses_pau] DROP CONSTRAINT [DF__bgt_expen__last___16644E42]
GO
ALTER TABLE [dbo].[bgt_expenses_pau] DROP CONSTRAINT [DF__bgt_expen__abep___15702A09]
GO
ALTER TABLE [dbo].[bgt_expenses_pau] DROP CONSTRAINT [DF__bgt_expen__abep___147C05D0]
GO
ALTER TABLE [dbo].[bgt_expenses_pau] DROP CONSTRAINT [DF__bgt_expen__abep___1387E197]
GO
/****** Object:  Table [dbo].[bgt_expenses_pau]    Script Date: 15/5/2023 4:09:59 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bgt_expenses_pau]') AND type in (N'U'))
DROP TABLE [dbo].[bgt_expenses_pau]
GO
/****** Object:  Table [dbo].[bgt_expenses_pau]    Script Date: 15/5/2023 4:09:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[bgt_expenses_pau](
	[abep_id] [int] IDENTITY(1,1) NOT NULL,
	[abep_revision] [int] NOT NULL,
	[abep_budgeting_year] [int] NOT NULL,
	[abep_syarikat_id] [int] NULL,
	[abep_syarikat_name] [nvarchar](100) NULL,
	[abep_ladang_id] [int] NULL,
	[abep_ladang_code] [nvarchar](5) NULL,
	[abep_ladang_name] [nvarchar](50) NULL,
	[abep_wilayah_id] [int] NULL,
	[abep_wilayah_name] [nvarchar](50) NULL,
	[abep_cost_center_code] [nvarchar](15) NULL,
	[abep_cost_center_desc] [nvarchar](100) NULL,
	[abep_gl_expenses_code] [nvarchar](20) NULL,
	[abep_gl_expenses_name] [nvarchar](100) NULL,
	[abep_gl_income_code] [nvarchar](20) NULL,
	[abep_gl_income_name] [nvarchar](100) NULL,
	[abep_product_code] [nvarchar](20) NULL,
	[abep_product_name] [nvarchar](100) NULL,
	[abep_cost_center_jualan_code] [nvarchar](15) NULL,
	[abep_cost_center_jualan_desc] [nvarchar](100) NULL,
	[abep_quantity] [decimal](18, 2) NULL,
	[abep_uom_1] [nvarchar](25) NULL,
	[abep_rate] [decimal](18, 2) NULL,
	[abep_quantity_1] [decimal](18, 2) NULL,
	[abep_quantity_2] [decimal](18, 2) NULL,
	[abep_quantity_3] [decimal](18, 2) NULL,
	[abep_quantity_4] [decimal](18, 2) NULL,
	[abep_quantity_5] [decimal](18, 2) NULL,
	[abep_quantity_6] [decimal](18, 2) NULL,
	[abep_quantity_7] [decimal](18, 2) NULL,
	[abep_quantity_8] [decimal](18, 2) NULL,
	[abep_quantity_9] [decimal](18, 2) NULL,
	[abep_quantity_10] [decimal](18, 2) NULL,
	[abep_quantity_11] [decimal](18, 2) NULL,
	[abep_quantity_12] [decimal](18, 2) NULL,
	[abep_amount_1] [decimal](18, 2) NULL,
	[abep_amount_2] [decimal](18, 2) NULL,
	[abep_amount_3] [decimal](18, 2) NULL,
	[abep_amount_4] [decimal](18, 2) NULL,
	[abep_amount_5] [decimal](18, 2) NULL,
	[abep_amount_6] [decimal](18, 2) NULL,
	[abep_amount_7] [decimal](18, 2) NULL,
	[abep_amount_8] [decimal](18, 2) NULL,
	[abep_amount_9] [decimal](18, 2) NULL,
	[abep_amount_10] [decimal](18, 2) NULL,
	[abep_amount_11] [decimal](18, 2) NULL,
	[abep_amount_12] [decimal](18, 2) NULL,
	[abep_total_quantity] [decimal](18, 2) NULL,
	[abep_total] [decimal](18, 2) NULL,
	[abep_proration] [nvarchar](25) NULL,
	[abep_history] [bit] NOT NULL,
	[abep_status] [nvarchar](50) NULL,
	[abep_created_by] [int] NULL,
	[abep_deleted] [bit] NOT NULL,
	[last_modified] [datetime] NULL,
 CONSTRAINT [PK__bgt_expe__92FE2232460BE950] PRIMARY KEY CLUSTERED 
(
	[abep_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[bgt_expenses_pau] ADD  CONSTRAINT [DF__bgt_expen__abep___1387E197]  DEFAULT ((0)) FOR [abep_revision]
GO
ALTER TABLE [dbo].[bgt_expenses_pau] ADD  CONSTRAINT [DF__bgt_expen__abep___147C05D0]  DEFAULT ((0)) FOR [abep_history]
GO
ALTER TABLE [dbo].[bgt_expenses_pau] ADD  CONSTRAINT [DF__bgt_expen__abep___15702A09]  DEFAULT ((0)) FOR [abep_deleted]
GO
ALTER TABLE [dbo].[bgt_expenses_pau] ADD  CONSTRAINT [DF__bgt_expen__last___16644E42]  DEFAULT (getdate()) FOR [last_modified]
GO
