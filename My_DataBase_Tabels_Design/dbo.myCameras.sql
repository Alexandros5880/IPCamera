﻿CREATE TABLE [dbo].[myCameras] (
    [Id]               NVARCHAR (50) NOT NULL,
    [urls]             NCHAR (333)   NOT NULL,
    [name]             NCHAR (50)    NOT NULL,
    [Face_Detection]   BIT           DEFAULT ((0)) NOT NULL,
    [Face_Recognition] BIT           DEFAULT ((0)) NOT NULL,
    [Brightness]       INT           DEFAULT 0 NOT NULL,
    [Contrast]         INT           DEFAULT 0 NOT NULL,
    [Darkness]         INT           DEFAULT 0 NOT NULL,
    [Recording]        BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
