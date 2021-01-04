﻿CREATE TABLE [dbo].[myCameras] (
    [Id]               NVARCHAR (50)  NOT NULL,
    [urls]             NCHAR (333)    NOT NULL,
    [name]             NCHAR (50)     NOT NULL,
    [Face_Detection]   BIT            DEFAULT ((0)) NOT NULL,
    [Face_Recognition] BIT            DEFAULT ((0)) NOT NULL,
    [Brightness]       INT            DEFAULT ((0)) NOT NULL,
    [Contrast]         INT            DEFAULT ((0)) NOT NULL,
    [Darkness]         INT            DEFAULT ((0)) NOT NULL,
    [Recording]        BIT            DEFAULT ((0)) NOT NULL,
    [On_Move_Pic]      BIT            DEFAULT ((0)) NOT NULL,
    [On_Move_Rec]      BIT            DEFAULT ((0)) NOT NULL,
    [On_Move_SMS]      BIT            DEFAULT ((0)) NOT NULL,
    [On_Move_EMAIL]    BIT            DEFAULT ((0)) NOT NULL,
    [Move_Sensitivity] INT            DEFAULT ((2)) NOT NULL,
    [Up_req]           NCHAR(333) NULL ,
    [Down_req]         NCHAR(333) NULL,
    [Left_req]         NCHAR(333) NULL,
    [Right_req]        NCHAR(333) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

