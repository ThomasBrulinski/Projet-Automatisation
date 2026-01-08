-- Création de la table Migrations
CREATE TABLE IF NOT EXISTS Migrations (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    RowHash VARCHAR(64) UNIQUE NOT NULL,
    MigrationStartTime DATETIME NULL,
    SubJobId VARCHAR(50) NULL,
    Title VARCHAR(512) NULL,
    Type VARCHAR(50) NULL,
    SourceId VARCHAR(255) NULL,
    Source VARCHAR(512) NULL,
    DestinationId VARCHAR(255) NULL,
    Destination VARCHAR(512) NULL,
    Size VARCHAR(20) NULL,
    Status VARCHAR(20) NULL,
    MigrationAction VARCHAR(20) NULL,
    Comment VARCHAR(512) NULL,
    ErrorCode VARCHAR(255) NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
