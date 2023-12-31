USE [BFASOPMSCORP]
GO
ALTER TABLE [dbo].[bgt_FeldaGrpBBSawit] DROP CONSTRAINT [DF__bgt_Felda__Price__32816A03]
GO
/****** Object:  Table [dbo].[bgt_FeldaGrpBBSawit]    Script Date: 11/4/2023 12:03:41 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bgt_FeldaGrpBBSawit]') AND type in (N'U'))
DROP TABLE [dbo].[bgt_FeldaGrpBBSawit]
GO
/****** Object:  Table [dbo].[bgt_FeldaGrpBBSawit]    Script Date: 11/4/2023 12:03:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[bgt_FeldaGrpBBSawit](
	[fld_ID] [int] NOT NULL,
	[PriceYear] [int] NOT NULL,
	[PriceBBSawit] [decimal](10, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[fld_ID] ASC,
	[PriceYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[bgt_FeldaGrpBBSawit] ([fld_ID], [PriceYear], [PriceBBSawit]) VALUES (1, 2023, CAST(2.38 AS Decimal(10, 2)))
INSERT [dbo].[bgt_FeldaGrpBBSawit] ([fld_ID], [PriceYear], [PriceBBSawit]) VALUES (2, 2023, CAST(2.38 AS Decimal(10, 2)))
INSERT [dbo].[bgt_FeldaGrpBBSawit] ([fld_ID], [PriceYear], [PriceBBSawit]) VALUES (3, 2023, CAST(2.38 AS Decimal(10, 2)))
INSERT [dbo].[bgt_FeldaGrpBBSawit] ([fld_ID], [PriceYear], [PriceBBSawit]) VALUES (4, 2023, CAST(4.04 AS Decimal(10, 2)))
INSERT [dbo].[bgt_FeldaGrpBBSawit] ([fld_ID], [PriceYear], [PriceBBSawit]) VALUES (5, 2023, CAST(2.38 AS Decimal(10, 2)))
GO
ALTER TABLE [dbo].[bgt_FeldaGrpBBSawit] ADD  DEFAULT (NULL) FOR [PriceBBSawit]
GO
