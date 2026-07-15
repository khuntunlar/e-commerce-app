CREATE TABLE IF NOT EXISTS `Users` (
  `Id` CHAR(36) NOT NULL,
  `Email` VARCHAR(256) NOT NULL,
  `NormalizedEmail` VARCHAR(256) NOT NULL,
  `PasswordHash` VARCHAR(512) NOT NULL,
  `DisplayName` VARCHAR(100) NOT NULL,
  `IsActive` TINYINT(1) NOT NULL,
  `CreatedAt` DATETIME(6) NOT NULL,
  `UpdatedAt` DATETIME(6) NULL,
  CONSTRAINT `PK_Users` PRIMARY KEY (`Id`),
  CONSTRAINT `IX_Users_NormalizedEmail` UNIQUE (`NormalizedEmail`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `Roles` (
  `Id` CHAR(36) NOT NULL,
  `Name` VARCHAR(100) NOT NULL,
  `NormalizedName` VARCHAR(100) NOT NULL,
  CONSTRAINT `PK_Roles` PRIMARY KEY (`Id`),
  CONSTRAINT `IX_Roles_NormalizedName` UNIQUE (`NormalizedName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `UserRoles` (
  `Id` CHAR(36) NOT NULL,
  `UserId` CHAR(36) NOT NULL,
  `RoleId` CHAR(36) NOT NULL,
  CONSTRAINT `PK_UserRoles` PRIMARY KEY (`Id`),
  CONSTRAINT `FK_UserRoles_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_UserRoles_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Roles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `IX_UserRoles_UserId_RoleId` UNIQUE (`UserId`, `RoleId`),
  INDEX `IX_UserRoles_RoleId` (`RoleId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `RefreshTokens` (
  `Id` CHAR(36) NOT NULL,
  `UserId` CHAR(36) NOT NULL,
  `TokenHash` VARCHAR(128) NOT NULL,
  `ExpiresAt` DATETIME(6) NOT NULL,
  `RevokedAt` DATETIME(6) NULL,
  `CreatedByIp` VARCHAR(64) NOT NULL,
  CONSTRAINT `PK_RefreshTokens` PRIMARY KEY (`Id`),
  CONSTRAINT `FK_RefreshTokens_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `IX_RefreshTokens_TokenHash` UNIQUE (`TokenHash`),
  INDEX `IX_RefreshTokens_UserId` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

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

CREATE TABLE IF NOT EXISTS `AuditLogs` (
  `Id` CHAR(36) NOT NULL,
  `UserId` CHAR(36) NULL,
  `Action` VARCHAR(100) NOT NULL,
  `Metadata` VARCHAR(500) NOT NULL,
  `CreatedAt` DATETIME(6) NOT NULL,
  CONSTRAINT `PK_AuditLogs` PRIMARY KEY (`Id`),
  INDEX `IX_AuditLogs_UserId` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
  `MigrationId` VARCHAR(150) NOT NULL,
  `ProductVersion` VARCHAR(32) NOT NULL,
  CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO `Roles` (`Id`, `Name`, `NormalizedName`)
VALUES ('11111111-1111-1111-1111-111111111111', 'Customer', 'CUSTOMER');

INSERT IGNORE INTO `Roles` (`Id`, `Name`, `NormalizedName`)
VALUES ('22222222-2222-2222-2222-222222222222', 'Admin', 'ADMIN');

INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('202607030001_InitialIdentitySchema', '9.0.0');
