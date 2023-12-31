USE [BFASOPMSCORP]
GO
/****** Object:  Table [dbo].[bgt_Notification]    Script Date: 6/4/2023 1:15:02 PM ******/
DROP TABLE [dbo].[bgt_Notification]
GO
/****** Object:  Table [dbo].[bgt_Notification]    Script Date: 6/4/2023 1:15:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[bgt_Notification](
	[Notify_ID] [int] IDENTITY(1,1) NOT NULL,
	[Bgt_Year] [int] NOT NULL,
	[Notify_Letter] [varchar](200) NULL,
	[Notify_Ref] [varchar](50) NULL,
	[Notify_Date] [date] NULL,
	[Notify_Content] [text] NULL,
	[Last_Modified] [datetime] NULL,
	[fld_Deleted] [bit] NULL,
	[fld_NegaraID] [int] NULL,
	[fld_SyarikatID] [int] NULL,
 CONSTRAINT [PK_bgt_Notification] PRIMARY KEY CLUSTERED 
(
	[Notify_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[bgt_Notification] ON 

INSERT [dbo].[bgt_Notification] ([Notify_ID], [Bgt_Year], [Notify_Letter], [Notify_Ref], [Notify_Date], [Notify_Content], [Last_Modified], [fld_Deleted], [fld_NegaraID], [fld_SyarikatID]) VALUES (1, 2024, N'NOTIFICATION LETTER FOR ESTATE TO PREPARE DRAFT BUDGET 2024', N'(04) Persuratan AM/3/1/BAJET/2024', CAST(N'2023-02-09' AS Date), N'<p class="MsoNormal" style="text-align:justify"><span lang="EN-US" style="font-size:11.0pt;font-family:&quot;Tahoma&quot;,sans-', CAST(N'2024-02-08T23:00:00.000' AS DateTime), 0, 1, 1)
SET IDENTITY_INSERT [dbo].[bgt_Notification] OFF
