USE SuperheroesDb;
GO


-- List all superheroes and their aliases
SELECT Name, Alias
FROM Superhero;
GO

-- Find all assistants and their associated superheroes
SELECT Assistant.Name AS AssistantName, Superhero.Name AS SuperheroName
FROM Assistant
JOIN Superhero ON Assistant.SuperheroId = Superhero.Id;
GO

-- List all superheroes along with their powers
SELECT Superhero.Name AS SuperheroName, Power.Name AS PowerName
FROM Superhero
JOIN SuperheroPower ON Superhero.Id = SuperheroPower.SuperheroId
JOIN Power ON SuperheroPower.PowerId = Power.Id;
GO

-- Find superheroes who have a specific power (e.g., 'Super strength')
SELECT Superhero.Name
FROM Superhero
JOIN SuperheroPower ON Superhero.Id = SuperheroPower.SuperheroId
JOIN Power ON SuperheroPower.PowerId = Power.Id
WHERE Power.Name = 'Super strength';
GO

-- List all superheroes along with their origins and the names of their assistants
SELECT Superhero.Name AS SuperheroName, Superhero.Origin, Assistant.Name AS AssistantName
FROM Superhero
LEFT JOIN Assistant ON Superhero.Id = Assistant.SuperheroId;
GO

-- Count the number of powers each superhero has
SELECT Superhero.Name, COUNT(SuperheroPower.PowerId) AS NumberOfPowers
FROM Superhero
JOIN SuperheroPower ON Superhero.Id = SuperheroPower.SuperheroId
GROUP BY Superhero.Name;
GO

-- Find superheroes without any assistants
SELECT Name
FROM Superhero
WHERE Id NOT IN (SELECT SuperheroId FROM Assistant);
GO
