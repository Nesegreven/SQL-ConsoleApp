-- Create linking table for Superhero and Power
CREATE TABLE SuperheroPower (
    SuperheroId INT,
    PowerId INT,
    PRIMARY KEY (SuperheroId, PowerId)
);

-- Add foreign key constraints
ALTER TABLE SuperheroPower
ADD CONSTRAINT FK_SuperheroPower_Superhero
FOREIGN KEY (SuperheroId) REFERENCES Superhero(Id);

ALTER TABLE SuperheroPower
ADD CONSTRAINT FK_SuperheroPower_Power
FOREIGN KEY (PowerId) REFERENCES Power(Id);
