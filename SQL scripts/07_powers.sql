-- Insert powers
INSERT INTO Power (Name, Description)
VALUES 
('Web-slinging', 'Ability to shoot and swing from webs'),
('Super strength', 'Enhanced physical strength'),
('Genius-level intellect', 'Extremely high IQ and problem-solving skills'),
('Flight', 'Ability to fly');

-- Associate powers with superheroes
INSERT INTO SuperheroPower (SuperheroId, PowerId)
VALUES 
(1, 1), -- Spider-Man: Web-slinging
(1, 2), -- Spider-Man: Super strength
(2, 3), -- Batman: Genius-level intellect
(3, 2), -- Wonder Woman: Super strength
(3, 4); -- Wonder Woman: Flight
