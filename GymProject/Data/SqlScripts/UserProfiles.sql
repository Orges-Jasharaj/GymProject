CREATE TABLE UserProfiles
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId NVARCHAR(450) NOT NULL,
    Gender NVARCHAR(20) NULL,
    HeightCm DECIMAL(5,2) NULL,
    CurrentWeightKg DECIMAL(5,2) NULL,
    GoalWeightKg DECIMAL(5,2) NULL,
    ActivityLevel NVARCHAR(50) NULL,
    FitnessGoal NVARCHAR(100) NULL,
    DateOfBirth DATE NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL,

    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);