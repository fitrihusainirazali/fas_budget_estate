USE [BFASOPMSCORP]
GO
ALTER TABLE [dbo].[bgt_ScreenGL] DROP CONSTRAINT [FK_tbl_SAPGLPUP]
GO
ALTER TABLE [dbo].[bgt_ScreenGL] DROP CONSTRAINT [FK_bgt_Screen]
GO
/****** Object:  Table [dbo].[bgt_ScreenGL]    Script Date: 16/3/2023 12:33:54 PM ******/
DROP TABLE [dbo].[bgt_ScreenGL]
GO
/****** Object:  Table [dbo].[bgt_ScreenGL]    Script Date: 16/3/2023 12:33:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[bgt_ScreenGL](
	[fld_ScrID] [int] NOT NULL,
	[fld_GLID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_bgt_ScreenGL] PRIMARY KEY CLUSTERED 
(
	[fld_ScrID] ASC,
	[fld_GLID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[bgt_ScreenGL]  WITH CHECK ADD  CONSTRAINT [FK_bgt_Screen] FOREIGN KEY([fld_ScrID])
REFERENCES [dbo].[bgt_Screen] ([ScrID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[bgt_ScreenGL] CHECK CONSTRAINT [FK_bgt_Screen]
GO
ALTER TABLE [dbo].[bgt_ScreenGL]  WITH CHECK ADD  CONSTRAINT [FK_tbl_SAPGLPUP] FOREIGN KEY([fld_GLID])
REFERENCES [dbo].[tbl_SAPGLPUP] ([fld_ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[bgt_ScreenGL] CHECK CONSTRAINT [FK_tbl_SAPGLPUP]
GO
