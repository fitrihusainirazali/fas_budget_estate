USE [BFASOPMSCORP]
GO
ALTER TABLE [dbo].[bgt_Screen] DROP CONSTRAINT [FK_ScrHdrScreens]
GO
/****** Object:  Table [dbo].[bgt_Screen]    Script Date: 16/3/2023 12:36:07 PM ******/
DROP TABLE [dbo].[bgt_Screen]
GO
/****** Object:  Table [dbo].[bgt_Screen]    Script Date: 16/3/2023 12:36:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[bgt_Screen](
	[ScrID] [int] IDENTITY(1,1) NOT NULL,
	[ScrSort] [int] NULL DEFAULT (NULL),
	[ScrSystem] [char](2) NULL DEFAULT (NULL),
	[ScrHdrCode] [char](4) NULL DEFAULT (NULL),
	[ScrCode] [nvarchar](4) NOT NULL,
	[GLTick] [bit] NULL DEFAULT (NULL),
	[ScrName] [nvarchar](25) NOT NULL,
	[ScrNameLongDesc] [nvarchar](60) NULL DEFAULT (NULL),
PRIMARY KEY CLUSTERED 
(
	[ScrID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[bgt_Screen] ON 

INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (1, 1, N'CC', N'ICC ', N'I1', 1, N'Sawit', N'RUMUSAN JUALAN BULANAN SAWIT')
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (2, 2, N'CC', N'ICC ', N'I2', 1, N'Biji Benih Sawit', N'RUMUSAN JUALAN BULANAN BIJI BENIH')
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (3, 3, N'CC', N'ICC ', N'I3', 1, N'Produk Lain-Lain', N'RUMUSAN JUALAN BULANAN MENGIKUT PRODUK SELAIN SAWIT, BENIH')
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (4, 4, N'CC', N'EHQ ', N'E1', 1, N'Susutnilai', NULL)
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (5, 5, N'CC', N'EHQ ', N'E2', 1, N'Gaji Kakitangan', NULL)
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (6, 6, N'CC', N'EHQ ', N'E3', 1, N'Insuran', NULL)
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (7, 7, N'CC', N'EHQ ', N'E4', 1, N'IT', NULL)
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (8, 8, N'CC', N'EHQ ', N'E5', 1, N'Kawal HQ', N'PERBELANJAAN HQ (BELANJA LAIN-LAIN (DI KAWAL OLEH HQ))')
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (9, 9, N'CC', N'EHQ ', N'E6', 1, N'Kawal Korporat', NULL)
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (10, 10, N'CC', N'EHQ ', N'E7', 1, N'Perubatan', NULL)
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (11, 11, N'CC', N'EHQ ', N'E8', 1, N'Windfall Tax', NULL)
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (12, 12, N'CC', N'ECC ', N'E9', 1, N'Harta Tetap', NULL)
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (13, 13, N'CC', N'ECC ', N'E10', 1, N'Perbelanjaan Antara Unit', NULL)
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (14, 14, N'CC', N'ECC ', N'E11', 1, N'Levi (Sawit)', NULL)
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (15, 15, N'CC', N'ECC ', N'E12', 1, N'Kawalan Gaji (KG)', NULL)
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (16, 16, N'CC', N'ECC ', N'E13', NULL, N'Daftar Kenderaan', NULL)
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (17, 17, N'CC', N'ECC ', N'E14', 1, N'Kenderaan', NULL)
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (18, 18, N'CC', N'ECC ', N'E15', 1, N'Perbelanjaan Lain-Lain', NULL)
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (19, 1, N'HQ', NULL, N'H1', NULL, N'Master File', N'CONFIG MASTER FILE SCREEN @ Corporate')
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (20, 2, N'HQ', NULL, N'H2', NULL, N'Budget Guidelines', N'BUDGET GUIDELINES @ Corporate')
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (21, 3, N'HQ', NULL, N'H3', NULL, N'Draft Budget', N'Draft Budget @ Corporate')
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (22, 4, N'HQ', NULL, N'H4', NULL, N'Notification Letter', N'Notification Letter @ Corporate')
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (23, 5, N'HQ', NULL, N'H5', NULL, N'Budget Correction', N'Budget Correction @ Corporate')
INSERT [dbo].[bgt_Screen] ([ScrID], [ScrSort], [ScrSystem], [ScrHdrCode], [ScrCode], [GLTick], [ScrName], [ScrNameLongDesc]) VALUES (24, 6, N'HQ', NULL, N'H6', NULL, N'Final Budget', N'Final Budget @ Corporate')
SET IDENTITY_INSERT [dbo].[bgt_Screen] OFF
ALTER TABLE [dbo].[bgt_Screen]  WITH CHECK ADD  CONSTRAINT [FK_ScrHdrScreens] FOREIGN KEY([ScrHdrCode])
REFERENCES [dbo].[bgt_ScrHdr] ([ScrHdrCode])
GO
ALTER TABLE [dbo].[bgt_Screen] CHECK CONSTRAINT [FK_ScrHdrScreens]
GO
