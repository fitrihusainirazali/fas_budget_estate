USE [BFASOPMSCORP]
GO
/****** Object:  Table [dbo].[bgt_Operation]    Script Date: 13/4/2023 2:32:02 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bgt_Operation]') AND type in (N'U'))
DROP TABLE [dbo].[bgt_Operation]
GO
/****** Object:  Table [dbo].[bgt_Operation]    Script Date: 13/4/2023 2:32:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[bgt_Operation](
	[Opr_ID] [int] IDENTITY(1,1) NOT NULL,
	[Opr_Sign] [char](1) NOT NULL,
	[Opr_Desc] [nvarchar](15) NULL,
 CONSTRAINT [PK_bgt_Operation] PRIMARY KEY CLUSTERED 
(
	[Opr_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[bgt_Operation] ON 

INSERT [dbo].[bgt_Operation] ([Opr_ID], [Opr_Sign], [Opr_Desc]) VALUES (1, N'+', N'PLUS')
INSERT [dbo].[bgt_Operation] ([Opr_ID], [Opr_Sign], [Opr_Desc]) VALUES (2, N'-', N'MINUS')
INSERT [dbo].[bgt_Operation] ([Opr_ID], [Opr_Sign], [Opr_Desc]) VALUES (3, N'*', N'MULTIPLY')
INSERT [dbo].[bgt_Operation] ([Opr_ID], [Opr_Sign], [Opr_Desc]) VALUES (4, N'/', N'DIVIDE')
SET IDENTITY_INSERT [dbo].[bgt_Operation] OFF
GO
