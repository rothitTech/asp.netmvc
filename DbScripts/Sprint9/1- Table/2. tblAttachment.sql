/****** Object:  Table [dbo].[Attachment]    Script Date: 11/14/2014 11:17:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblAttachment](
	[AttachmentId] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [varchar](255) NOT NULL,
	[FileExtension] [varchar](10) NOT NULL,
	[CreatedUserId] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedUserId] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK__Attachment] PRIMARY KEY CLUSTERED 
(
	[AttachmentId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[tblKnowledgeBaseAttachment]    Script Date: 11/14/2014 11:17:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblKnowledgeBaseAttachment](
	[KnowledgeBaseAttachmentId] [int] IDENTITY(1,1) NOT NULL,
	[KnowledgeBaseId] [int] NOT NULL,
	[AttachmentId] [int] NOT NULL,
	[CreatedUserId] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedUserId] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK__KnowledgeBase] PRIMARY KEY CLUSTERED 
(
	[KnowledgeBaseAttachmentId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  ForeignKey [FK_AnnouncementAttachment_Attachment]    Script Date: 11/14/2014 11:17:28 ******/
ALTER TABLE [dbo].[tblKnowledgeBaseAttachment]  WITH CHECK ADD  CONSTRAINT [FK_KnowledgeBaseAttachment_Attachment] FOREIGN KEY([AttachmentId])
REFERENCES [dbo].[tblAttachment] ([AttachmentId])
GO
ALTER TABLE [dbo].[tblKnowledgeBaseAttachment] CHECK CONSTRAINT [FK_KnowledgeBaseAttachment_Attachment]
GO

ALTER TABLE [dbo].[tblKnowledgeBaseAttachment]  WITH CHECK ADD  CONSTRAINT [FK_KnowledgeBaseAttachment_KnowledgeBase] FOREIGN KEY([KnowledgeBaseId])
REFERENCES [dbo].[tblKnowledgeBase] ([KnowledgeBaseId])
GO
ALTER TABLE [dbo].[tblKnowledgeBaseAttachment] CHECK CONSTRAINT [FK_KnowledgeBaseAttachment_KnowledgeBase]
GO