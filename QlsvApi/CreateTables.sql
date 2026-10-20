USE QLSV2

-- Tao bang Khoa truoc
CREATE TABLE [Majors] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [MajorCode] NVARCHAR(10) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    
    CONSTRAINT [PK_Majors] PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Majors_Name] UNIQUE ([Name]) -- Ràng buộc UNIQUE cho Name
);


-- Tao bang student thi p sau
CREATE TABLE [Students] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [CardId] NVARCHAR(20) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [BirthDate] DATE NOT NULL,
    [Gender] NVARCHAR(10) NOT NULL,
    [MajorId] INT NOT NULL,
    
    CONSTRAINT [PK_Students] PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Students_CardId] UNIQUE ([CardId]), -- cardId is UNIQUE
    
    -- Foreign Key to Major table
    CONSTRAINT [FK_Students_Majors_MajorId] FOREIGN KEY ([MajorId]) 
        REFERENCES [Majors] ([Id]) 
);