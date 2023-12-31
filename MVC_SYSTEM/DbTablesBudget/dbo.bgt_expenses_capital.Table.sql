USE [BFASOPMSESTSEMENANJUNG]
GO
/****** Object:  Table [dbo].[bgt_expenses_capital]    Script Date: 6/4/2023 4:28:12 PM ******/
DROP TABLE [dbo].[bgt_expenses_capital]
GO
/****** Object:  Table [dbo].[bgt_expenses_capital]    Script Date: 6/4/2023 4:28:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[bgt_expenses_capital](
	[abce_id] [int] IDENTITY(1,1) NOT NULL,
	[abce_revision] [int] NOT NULL DEFAULT ((0)),
	[abce_budgeting_year] [int] NOT NULL,
	[abce_syarikat_id] [int] NULL,
	[abce_syarikat_name] [nvarchar](100) NULL,
	[abce_ladang_id] [int] NULL,
	[abce_ladang_code] [nvarchar](5) NULL,
	[abce_ladang_name] [nvarchar](50) NULL,
	[abce_wilayah_id] [int] NULL,
	[abce_wilayah_name] [nvarchar](50) NULL,
	[abce_cost_center_code] [nvarchar](15) NULL,
	[abce_cost_center_desc] [nvarchar](100) NULL,
	[abce_gl_code] [nvarchar](20) NULL,
	[abce_gl_name] [nvarchar](100) NULL,
	[abce_desc] [nvarchar](255) NULL,
	[abce_jenis_code] [nvarchar](20) NULL,
	[abce_jenis_name] [nvarchar](100) NULL,
	[abce_reason] [nvarchar](255) NULL,
	[abce_material_details] [nvarchar](100) NULL,
	[abce_unit_1] [decimal](18, 2) NULL,
	[abce_uom_1] [decimal](18, 2) NULL,
	[abce_rate] [decimal](18, 2) NULL,
	[abce_quantity_1] [decimal](18, 2) NULL,
	[abce_quantity_2] [decimal](18, 2) NULL,
	[abce_quantity_3] [decimal](18, 2) NULL,
	[abce_quantity_4] [decimal](18, 2) NULL,
	[abce_quantity_5] [decimal](18, 2) NULL,
	[abce_quantity_6] [decimal](18, 2) NULL,
	[abce_quantity_7] [decimal](18, 2) NULL,
	[abce_quantity_8] [decimal](18, 2) NULL,
	[abce_quantity_9] [decimal](18, 2) NULL,
	[abce_quantity_10] [decimal](18, 2) NULL,
	[abce_quantity_11] [decimal](18, 2) NULL,
	[abce_quantity_12] [decimal](18, 2) NULL,
	[abce_amount_1] [decimal](18, 2) NULL,
	[abce_amount_2] [decimal](18, 2) NULL,
	[abce_amount_3] [decimal](18, 2) NULL,
	[abce_amount_4] [decimal](18, 2) NULL,
	[abce_amount_5] [decimal](18, 2) NULL,
	[abce_amount_6] [decimal](18, 2) NULL,
	[abce_amount_7] [decimal](18, 2) NULL,
	[abce_amount_8] [decimal](18, 2) NULL,
	[abce_amount_9] [decimal](18, 2) NULL,
	[abce_amount_10] [decimal](18, 2) NULL,
	[abce_amount_11] [decimal](18, 2) NULL,
	[abce_amount_12] [decimal](18, 2) NULL,
	[abce_total] [decimal](18, 2) NULL,
	[abce_total_quantity] [decimal](18, 2) NULL,
	[abce_proration] [nvarchar](25) NULL,
	[abce_history] [bit] NOT NULL DEFAULT ((0)),
	[abce_status] [nvarchar](50) NULL,
	[abce_deleted] [bit] NOT NULL DEFAULT ((0)),
	[last_modified] [datetime] NULL DEFAULT (getdate()),
PRIMARY KEY CLUSTERED 
(
	[abce_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
