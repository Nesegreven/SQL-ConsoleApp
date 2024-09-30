-- Add SuperheroId foreign key to Assistant table
ALTER TABLE Assistant
ADD SuperheroId INT;

-- Add foreign key constraint
ALTER TABLE Assistant
ADD CONSTRAINT FK_Assistant_Superhero
FOREIGN KEY (SuperheroId) REFERENCES Superhero(Id);
