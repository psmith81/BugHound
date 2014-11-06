USE [BugHoundSQL]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 10/24/2014 10:50:59 AM ******/
DROP TABLE [dbo].[Users]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 10/24/2014 10:50:59 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[EMail] [nvarchar](256) NULL,
	[Phone] [nvarchar](50) NULL,
	[Supervisor] [nvarchar](256) NULL
) ON [PRIMARY]

GO


