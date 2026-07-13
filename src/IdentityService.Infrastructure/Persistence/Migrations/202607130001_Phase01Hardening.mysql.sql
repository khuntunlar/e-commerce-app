CREATE TABLE IF NOT EXISTS `PasswordResetTokens` (
  `Id` CHAR(36) NOT NULL,
  `UserId` CHAR(36) NOT NULL,
  `TokenHash` VARCHAR(128) NOT NULL,
  `ExpiresAt` DATETIME(6) NOT NULL,
  `UsedAt` DATETIME(6) NULL,
  `CreatedAt` DATETIME(6) NOT NULL,
  CONSTRAINT `PK_PasswordResetTokens` PRIMARY KEY (`Id`),
  CONSTRAINT `FK_PasswordResetTokens_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `IX_PasswordResetTokens_TokenHash` UNIQUE (`TokenHash`),
  INDEX `IX_PasswordResetTokens_UserId` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO `Roles` (`Id`, `Name`, `NormalizedName`)
VALUES ('22222222-2222-2222-2222-222222222222', 'Admin', 'ADMIN');

INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('202607130001_Phase01Hardening', '9.0.0');
