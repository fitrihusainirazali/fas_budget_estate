USE [BFASOPMSCORP]
GO
ALTER TABLE [dbo].[bgt_Product] DROP CONSTRAINT [FK__bgt_Produ__UOMID__0307610B]
GO
ALTER TABLE [dbo].[bgt_Product] DROP CONSTRAINT [FK__bgt_Produ__LocID__02133CD2]
GO
ALTER TABLE [dbo].[bgt_Product] DROP CONSTRAINT [DF__bgt_Produ__DateM__011F1899]
GO
ALTER TABLE [dbo].[bgt_Product] DROP CONSTRAINT [DF__bgt_Produ__OldPr__002AF460]
GO
ALTER TABLE [dbo].[bgt_Product] DROP CONSTRAINT [DF__bgt_Produ__Activ__7F36D027]
GO
ALTER TABLE [dbo].[bgt_Product] DROP CONSTRAINT [DF__bgt_Produ__Lowes__7E42ABEE]
GO
/****** Object:  Table [dbo].[bgt_Product]    Script Date: 15/5/2023 4:08:08 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bgt_Product]') AND type in (N'U'))
DROP TABLE [dbo].[bgt_Product]
GO
/****** Object:  Table [dbo].[bgt_Product]    Script Date: 15/5/2023 4:08:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[bgt_Product](
	[PrdID] [int] IDENTITY(1,1) NOT NULL,
	[CoCode] [nvarchar](4) NOT NULL,
	[YearBgt] [int] NOT NULL,
	[PrdCode] [nvarchar](20) NOT NULL,
	[PrdName] [nvarchar](50) NOT NULL,
	[LocID] [int] NULL,
	[Lowest] [bit] NOT NULL,
	[UOMID] [int] NULL,
	[PriceSale] [decimal](18, 2) NULL,
	[PricePAU] [decimal](18, 2) NULL,
	[Active] [bit] NOT NULL,
	[OldPriceSale] [decimal](18, 2) NULL,
	[DateModify] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[PrdID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[bgt_Product] ON 

INSERT [dbo].[bgt_Product] ([PrdID], [CoCode], [YearBgt], [PrdCode], [PrdName], [LocID], [Lowest], [UOMID], [PriceSale], [PricePAU], [Active], [OldPriceSale], [DateModify]) VALUES (2, N'8200', 2024, N'1801', N'BTS', 1, 0, NULL, NULL, CAST(1000.00 AS Decimal(18, 2)), 1, NULL, CAST(N'2023-03-05T00:00:00.000' AS DateTime))
INSERT [dbo].[bgt_Product] ([PrdID], [CoCode], [YearBgt], [PrdCode], [PrdName], [LocID], [Lowest], [UOMID], [PriceSale], [PricePAU], [Active], [OldPriceSale], [DateModify]) VALUES (3, N'8200', 2024, N'1101', N'AJAS', 2, 1, 21, NULL, CAST(2000.00 AS Decimal(18, 2)), 1, NULL, NULL)
INSERT [dbo].[bgt_Product] ([PrdID], [CoCode], [YearBgt], [PrdCode], [PrdName], [LocID], [Lowest], [UOMID], [PriceSale], [PricePAU], [Active], [OldPriceSale], [DateModify]) VALUES (4, N'8200', 2024, N'1102', N'AJAS', 3, 1, 21, NULL, CAST(2200.00 AS Decimal(18, 2)), 1, NULL, NULL)
INSERT [dbo].[bgt_Product] ([PrdID], [CoCode], [YearBgt], [PrdCode], [PrdName], [LocID], [Lowest], [UOMID], [PriceSale], [PricePAU], [Active], [OldPriceSale], [DateModify]) VALUES (5, N'8200', 2024, N'1103', N'AJAS', 4, 1, 21, NULL, CAST(2300.00 AS Decimal(18, 2)), 1, NULL, NULL)
INSERT [dbo].[bgt_Product] ([PrdID], [CoCode], [YearBgt], [PrdCode], [PrdName], [LocID], [Lowest], [UOMID], [PriceSale], [PricePAU], [Active], [OldPriceSale], [DateModify]) VALUES (6, N'8200', 2024, N'12', N'FUNGI', 1, 0, NULL, NULL, CAST(3000.00 AS Decimal(18, 2)), 1, NULL, NULL)
INSERT [dbo].[bgt_Product] ([PrdID], [CoCode], [YearBgt], [PrdCode], [PrdName], [LocID], [Lowest], [UOMID], [PriceSale], [PricePAU], [Active], [OldPriceSale], [DateModify]) VALUES (7, N'8200', 2024, N'1201', N'AMF (ARBUSCULAR MYCHORHIZA FUNGI)', 1, 1, 39, NULL, CAST(4000.00 AS Decimal(18, 2)), 1, NULL, NULL)
INSERT [dbo].[bgt_Product] ([PrdID], [CoCode], [YearBgt], [PrdCode], [PrdName], [LocID], [Lowest], [UOMID], [PriceSale], [PricePAU], [Active], [OldPriceSale], [DateModify]) VALUES (8, N'8200', 2024, N'13', N'BAHAN TANAMAN', 1, 0, NULL, NULL, CAST(5000.00 AS Decimal(18, 2)), 1, NULL, NULL)
INSERT [dbo].[bgt_Product] ([PrdID], [CoCode], [YearBgt], [PrdCode], [PrdName], [LocID], [Lowest], [UOMID], [PriceSale], [PricePAU], [Active], [OldPriceSale], [DateModify]) VALUES (9, N'8200', 2024, N'1301', N'ANAK SAWIT BESAR', 1, 0, NULL, NULL, CAST(6000.00 AS Decimal(18, 2)), 1, NULL, NULL)
INSERT [dbo].[bgt_Product] ([PrdID], [CoCode], [YearBgt], [PrdCode], [PrdName], [LocID], [Lowest], [UOMID], [PriceSale], [PricePAU], [Active], [OldPriceSale], [DateModify]) VALUES (10, N'8200', 2024, N'130101', N'ANAK SAWIT BESAR KLON DxP', 1, 1, 25, NULL, CAST(7000.00 AS Decimal(18, 2)), 1, NULL, NULL)
INSERT [dbo].[bgt_Product] ([PrdID], [CoCode], [YearBgt], [PrdCode], [PrdName], [LocID], [Lowest], [UOMID], [PriceSale], [PricePAU], [Active], [OldPriceSale], [DateModify]) VALUES (11, N'8200', 2024, N'130102', N'ANAK SAWIT KECIL KLON DxP+', 1, 1, 25, NULL, CAST(8000.00 AS Decimal(18, 2)), 1, NULL, NULL)
INSERT [dbo].[bgt_Product] ([PrdID], [CoCode], [YearBgt], [PrdCode], [PrdName], [LocID], [Lowest], [UOMID], [PriceSale], [PricePAU], [Active], [OldPriceSale], [DateModify]) VALUES (12, N'8200', 2024, N'1', N'AJAS HDR', 1, 0, NULL, NULL, CAST(9000.00 AS Decimal(18, 2)), 1, NULL, NULL)
SET IDENTITY_INSERT [dbo].[bgt_Product] OFF
GO
ALTER TABLE [dbo].[bgt_Product] ADD  DEFAULT ((0)) FOR [Lowest]
GO
ALTER TABLE [dbo].[bgt_Product] ADD  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[bgt_Product] ADD  DEFAULT (NULL) FOR [OldPriceSale]
GO
ALTER TABLE [dbo].[bgt_Product] ADD  DEFAULT (NULL) FOR [DateModify]
GO
ALTER TABLE [dbo].[bgt_Product]  WITH CHECK ADD FOREIGN KEY([LocID])
REFERENCES [dbo].[bgt_Location] ([LocID])
GO
ALTER TABLE [dbo].[bgt_Product]  WITH CHECK ADD FOREIGN KEY([UOMID])
REFERENCES [dbo].[bgt_UOM] ([UOMID])
GO
