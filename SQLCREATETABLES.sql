DROP TABLE [dbo].[Word];  
DROP TABLE [dbo].[Video];  

CREATE TABLE [dbo].[User] (
	[UserId] INT IDENTITY (1, 1) NOT NULL,
	[Name] VARCHAR (MAX) NOT NULL,
	[EmailAdress] VARCHAR (MAX) NOT NULL,
	[Role] VARCHAR (MAX) NOT NULL,
	PRIMARY KEY CLUSTERED ([UserId] ASC)
); 

CREATE TABLE [dbo].[Transcription] (
	[TranscriptionId] INT IDENTITY (1, 1) NOT NULL,
	[Flag] VARCHAR (MAX) NOT NULL,
	PRIMARY KEY CLUSTERED ([TranscriptionId] ASC)
); 

CREATE TABLE [dbo].[File] (
    [FileId]       INT           IDENTITY (1, 1) NOT NULL,
    [Title]         VARCHAR (MAX) NULL,
    [FilePath]     VARCHAR (MAX) NULL,
    [TranscriptionId] INT NULL REFERENCES [Transcription] ([TranscriptionId]),
    [DateAdded]     DATETIME      DEFAULT (getdate()) NULL,
    [Type]     VARCHAR (MAX) NULL,
    [UserId]  INT NULL REFERENCES [User] ([UserId]),
    [Description] VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([FileId] ASC)
);

CREATE TABLE [dbo].[Versions] (
	[VersionId] INT IDENTITY (1, 1) NOT NULL,
	[UserId]  INT NULL REFERENCES [User] ([UserId]),
	[TranscriptionId] INT NULL REFERENCES [Transcription] ([TranscriptionId]),
	[Transcription] VARCHAR (MAX) NOT NULL,
	[Active] INT DEFAULT 0 NOT NULL, 
	PRIMARY KEY CLUSTERED ([VersionId] ASC) 
);

CREATE TABLE [dbo].[Word] (
    [Id]        INT           NOT NULL,
    [Term]      VARCHAR (MAX) NULL,
    [Timestamp] VARCHAR (MAX) NULL,
    [VersionId]   INT           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([VersionId]) REFERENCES [dbo].[Versions] ([VersionId])
)
