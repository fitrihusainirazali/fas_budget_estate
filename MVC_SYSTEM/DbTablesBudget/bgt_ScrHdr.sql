USE [BFASOPMSCORP]
GO
/****** Object:  Table [dbo].[bgt_ScrHdr]    Script Date: 16/3/2023 12:35:03 PM ******/
DROP TABLE [dbo].[bgt_ScrHdr]
GO
/****** Object:  Table [dbo].[bgt_ScrHdr]    Script Date: 16/3/2023 12:35:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[bgt_ScrHdr](
	[ScrHdrCode] [char](4) NOT NULL,
	[ScrHdrName] [nvarchar](30) NOT NULL,
	[ScrHdrDesc] [nvarchar](60) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ScrHdrCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[bgt_ScrHdr] ([ScrHdrCode], [ScrHdrName], [ScrHdrDesc]) VALUES (N'ECC ', N'Expense CC', N'EXPENSES - GL COST CENTER')
INSERT [dbo].[bgt_ScrHdr] ([ScrHdrCode], [ScrHdrName], [ScrHdrDesc]) VALUES (N'EHQ ', N'Expense HQ', N'EXPENSES - GL UNDER HQ SUPERVISION')
INSERT [dbo].[bgt_ScrHdr] ([ScrHdrCode], [ScrHdrName], [ScrHdrDesc]) VALUES (N'ICC ', N'Income', N'INCOMES - GL COST CENTER')
INSERT [dbo].[bgt_ScrHdr] ([ScrHdrCode], [ScrHdrName], [ScrHdrDesc]) VALUES (N'MGR ', N'Manager Approval', N'Manager To Approve Draft Budget')
