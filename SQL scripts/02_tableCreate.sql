-- Create Superhero table
CREATE TABLE Superhero (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Alias NVARCHAR(100),
    Origin NVARCHAR(255)
);

-- Create Assistant table
CREATE TABLE Assistant (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);

-- Create Power table
CREATE TABLE Power (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255)
);
