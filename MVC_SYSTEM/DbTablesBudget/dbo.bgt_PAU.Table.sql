USE [BFASOPMSCORP]
GO
/****** Object:  Table [dbo].[bgt_PAU]    Script Date: 15/5/2023 4:08:08 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bgt_PAU]') AND type in (N'U'))
DROP TABLE [dbo].[bgt_PAU]
GO
/****** Object:  Table [dbo].[bgt_PAU]    Script Date: 15/5/2023 4:08:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[bgt_PAU](
	[PAU_ID] [int] IDENTITY(1,1) NOT NULL,
	[GLCode_Buy] [nvarchar](10) NOT NULL,
	[GLCode_Buy_Desc] [nvarchar](50) NULL,
	[GLCode_Sale] [nvarchar](10) NOT NULL,
	[GLCode_Sale_Desc] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[PAU_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[bgt_PAU] ON 

INSERT [dbo].[bgt_PAU] ([PAU_ID], [GLCode_Buy], [GLCode_Buy_Desc], [GLCode_Sale], [GLCode_Sale_Desc]) VALUES (1, N'53201010', N'Kos Jualan Antara Unit - Benih', N'43001010', N'Jualan Antara Unit - Biji Benih')
INSERT [dbo].[bgt_PAU] ([PAU_ID], [GLCode_Buy], [GLCode_Buy_Desc], [GLCode_Sale], [GLCode_Sale_Desc]) VALUES (2, N'53201020', N'Kos Jualan Antara Unit - Anak Sawit', N'43001020', N'Jualan Antara Unit - Anak Sawit')
INSERT [dbo].[bgt_PAU] ([PAU_ID], [GLCode_Buy], [GLCode_Buy_Desc], [GLCode_Sale], [GLCode_Sale_Desc]) VALUES (3, N'53201030', N'Kos Jualan Antara Unit - Cerakinan', N'43001030', N'Jualan Antara Unit - Cerakinan')
INSERT [dbo].[bgt_PAU] ([PAU_ID], [GLCode_Buy], [GLCode_Buy_Desc], [GLCode_Sale], [GLCode_Sale_Desc]) VALUES (4, N'53201050', N'Kos Jualan Antara Unit - Penyelidikan', N'43001050', N'Jualan Antara Unit - Penyelidikan')
INSERT [dbo].[bgt_PAU] ([PAU_ID], [GLCode_Buy], [GLCode_Buy_Desc], [GLCode_Sale], [GLCode_Sale_Desc]) VALUES (5, N'53201060', N'Kos Jualan Antara Unit - Buahan', N'43001060', N'Jualan Antara Unit - Buahan')
INSERT [dbo].[bgt_PAU] ([PAU_ID], [GLCode_Buy], [GLCode_Buy_Desc], [GLCode_Sale], [GLCode_Sale_Desc]) VALUES (6, N'53201070', N'Kos Jualan Antara Unit - Lain-lain', N'43001070', N'Jualan Antara Unit - Lain-lain')
INSERT [dbo].[bgt_PAU] ([PAU_ID], [GLCode_Buy], [GLCode_Buy_Desc], [GLCode_Sale], [GLCode_Sale_Desc]) VALUES (7, N'53201100', N'Kos Jualan Antara Unit - Butik S', N'43001090', N'Jualan Antara Unit - Butik S')
SET IDENTITY_INSERT [dbo].[bgt_PAU] OFF
GO
