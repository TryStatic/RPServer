/*
 Navicat Premium Data Transfer

 Source Server         : localhost_3306
 Source Server Type    : MySQL
 Source Server Version : 80016
 Source Host           : localhost:3306
 Source Schema         : rpserver

 Target Server Type    : MySQL
 Target Server Version : 80016
 File Encoding         : 65001

 Date: 15/09/2019 15:29:49
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for accounts
-- ----------------------------
DROP TABLE IF EXISTS `accounts`;
CREATE TABLE `accounts`  (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Username` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `EmailAddress` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `Hash` varbinary(64) NOT NULL,
  `AdminLevel` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `ForumName` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `NickName` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `RegSocialClubName` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LastSocialClubName` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `LastIP` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `LastHWID` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `CreationDate` datetime(0) NOT NULL,
  `LastLoginDate` datetime(0) NULL DEFAULT NULL,
  `HasEnabledTwoStepByEmail` tinyint(4) NOT NULL DEFAULT 0,
  `TwoFactorGASharedKey` varbinary(64) NULL DEFAULT NULL,
  `LastSpawnedCharId` int(11) NOT NULL DEFAULT -1,
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE INDEX `username_UNIQUE`(`Username`) USING BTREE,
  UNIQUE INDEX `emailaddress_UNIQUE`(`EmailAddress`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 245 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for aliases
-- ----------------------------
DROP TABLE IF EXISTS `aliases`;
CREATE TABLE `aliases`  (
  `CharID` int(11) NOT NULL,
  `AliasedID` int(11) NOT NULL,
  `AliasName` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_520_ci NOT NULL,
  `AliasDesc` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_520_ci NULL DEFAULT NULL,
  PRIMARY KEY (`CharID`, `AliasedID`) USING BTREE,
  INDEX `fkey_char_to__idx`(`AliasedID`) USING BTREE,
  CONSTRAINT `fkey_char_to_charaliasid` FOREIGN KEY (`AliasedID`) REFERENCES `characters` (`ID`) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT `fkey_char_to_charid` FOREIGN KEY (`CharID`) REFERENCES `characters` (`ID`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_520_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for appearances
-- ----------------------------
DROP TABLE IF EXISTS `appearances`;
CREATE TABLE `appearances`  (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `CharacterID` int(11) NOT NULL,
  `SkinModel` bigint(4) UNSIGNED NOT NULL,
  `IsMale` tinyint(4) UNSIGNED NOT NULL,
  `ShapeFirstID` tinyint(4) UNSIGNED NOT NULL,
  `ShapeSecondID` tinyint(4) UNSIGNED NOT NULL,
  `SkinSecondID` tinyint(4) UNSIGNED NOT NULL,
  `ShapeMix` float NOT NULL,
  `SkinMix` float NOT NULL,
  `Blemishes` tinyint(4) UNSIGNED NOT NULL,
  `FacialHair` tinyint(4) UNSIGNED NOT NULL,
  `Eyebrows` tinyint(4) UNSIGNED NOT NULL,
  `Ageing` tinyint(4) UNSIGNED NOT NULL,
  `Makeup` tinyint(4) UNSIGNED NOT NULL,
  `Blush` tinyint(4) UNSIGNED NOT NULL,
  `Complexion` tinyint(4) UNSIGNED NOT NULL,
  `SunDamage` tinyint(4) UNSIGNED NOT NULL,
  `Lipstick` tinyint(4) UNSIGNED NOT NULL,
  `Freckles` tinyint(4) UNSIGNED NOT NULL,
  `ChestHair` tinyint(4) UNSIGNED NOT NULL,
  `BodyBlemishes` tinyint(4) UNSIGNED NOT NULL,
  `AdditionalBodyBlemishes` tinyint(4) UNSIGNED NOT NULL,
  `BlemishesOpacity` float NOT NULL,
  `FacialHairOpacity` float NOT NULL,
  `EyebrowsOpacity` float NOT NULL,
  `AgeingOpacity` float NOT NULL,
  `MakeupOpacity` float NOT NULL,
  `BlushOpacity` float NOT NULL,
  `ComplexionOpacity` float NOT NULL,
  `SunDamageOpacity` float NOT NULL,
  `LipstickOpacity` float NOT NULL,
  `FrecklesOpacity` float NOT NULL,
  `ChestHairOpacity` float NOT NULL,
  `BodyBlemishesOpacity` float NOT NULL,
  `AdditionalBodyBlemishesOpacity` float NOT NULL,
  `BlemishesColor` tinyint(4) UNSIGNED NOT NULL,
  `FacialHairColor` tinyint(4) UNSIGNED NOT NULL,
  `EyebrowsColor` tinyint(4) UNSIGNED NOT NULL,
  `AgeingColor` tinyint(4) UNSIGNED NOT NULL,
  `MakeupColor` tinyint(4) UNSIGNED NOT NULL,
  `BlushColor` tinyint(4) UNSIGNED NOT NULL,
  `ComplexionColor` tinyint(4) UNSIGNED NOT NULL,
  `SunDamageColor` tinyint(4) UNSIGNED NOT NULL,
  `LipstickColor` tinyint(4) UNSIGNED NOT NULL,
  `FrecklesColor` tinyint(4) UNSIGNED NOT NULL,
  `ChestHairColor` tinyint(4) UNSIGNED NOT NULL,
  `BodyBlemishesColor` tinyint(4) UNSIGNED NOT NULL,
  `AdditionalBodyBlemishesColor` tinyint(4) UNSIGNED NOT NULL,
  `BlemishesSecColor` tinyint(4) UNSIGNED NOT NULL,
  `FacialHairSecColor` tinyint(4) UNSIGNED NOT NULL,
  `EyebrowsSecColor` tinyint(4) UNSIGNED NOT NULL,
  `AgeingSecColor` tinyint(4) UNSIGNED NOT NULL,
  `MakeupSecColor` tinyint(4) UNSIGNED NOT NULL,
  `BlushSecColor` tinyint(4) UNSIGNED NOT NULL,
  `ComplexionSecColor` tinyint(4) UNSIGNED NOT NULL,
  `SunDamageSecColor` tinyint(4) UNSIGNED NOT NULL,
  `LipstickSecColor` tinyint(4) UNSIGNED NOT NULL,
  `FrecklesSecColor` tinyint(4) UNSIGNED NOT NULL,
  `ChestHairSecColor` tinyint(4) UNSIGNED NOT NULL,
  `BodyBlemishesSecColor` tinyint(4) UNSIGNED NOT NULL,
  `AdditionalBodyBlemishesSecColor` tinyint(4) UNSIGNED NOT NULL,
  `NoseWidth` float NOT NULL,
  `NoseHeight` float NOT NULL,
  `NoseLength` float NOT NULL,
  `NoseBridge` float NOT NULL,
  `NoseTip` float NOT NULL,
  `NoseBridgeShift` float NOT NULL,
  `BrowHeight` float NOT NULL,
  `BrowWidth` float NOT NULL,
  `CheekboneHeight` float NOT NULL,
  `CheekboneWidth` float NOT NULL,
  `CheeksWidth` float NOT NULL,
  `Eyes` float NOT NULL,
  `Lips` float NOT NULL,
  `JawWidth` float NOT NULL,
  `JawHeight` float NOT NULL,
  `ChinLength` float NOT NULL,
  `ChinPosition` float NOT NULL,
  `ChinWidth` float NOT NULL,
  `ChinShape` float NOT NULL,
  `NeckWidth` float NOT NULL,
  `EyeColor` tinyint(4) UNSIGNED NOT NULL,
  `HairHighlightColor` tinyint(4) UNSIGNED NOT NULL,
  `HairColor` tinyint(4) UNSIGNED NOT NULL,
  `HairStyle` tinyint(4) UNSIGNED NOT NULL,
  `HairStyleTexture` tinyint(4) UNSIGNED NOT NULL,
  PRIMARY KEY (`ID`, `CharacterID`) USING BTREE,
  UNIQUE INDEX `CharacterID_UNIQUE`(`CharacterID`) USING BTREE,
  INDEX `fkey_char_to_appearance_idx`(`CharacterID`) USING BTREE,
  CONSTRAINT `fkey_char_to_appearance` FOREIGN KEY (`CharacterID`) REFERENCES `characters` (`ID`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 10 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_520_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for characters
-- ----------------------------
DROP TABLE IF EXISTS `characters`;
CREATE TABLE `characters`  (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `CharOwnerID` int(11) NOT NULL,
  `CharacterName` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `MinutesPlayed` int(11) NOT NULL DEFAULT 0,
  `LastX` float NOT NULL,
  `LastY` float NOT NULL,
  `LastZ` float NOT NULL,
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE INDEX `name_UNIQUE`(`CharacterName`) USING BTREE,
  INDEX `fkey_idx`(`CharOwnerID`) USING BTREE,
  CONSTRAINT `fkey_acc_to_char` FOREIGN KEY (`CharOwnerID`) REFERENCES `accounts` (`ID`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 29 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for emailtokens
-- ----------------------------
DROP TABLE IF EXISTS `emailtokens`;
CREATE TABLE `emailtokens`  (
  `accountID` int(11) NOT NULL,
  `token` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `emailaddress` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `expirydate` datetime(0) NOT NULL,
  PRIMARY KEY (`accountID`) USING BTREE,
  UNIQUE INDEX `emailaddress_UNIQUE`(`emailaddress`) USING BTREE,
  INDEX `accountID_UNIQUE`(`accountID`) USING BTREE,
  CONSTRAINT `fkey_acc_to_emailtoken` FOREIGN KEY (`accountID`) REFERENCES `accounts` (`ID`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for vehicles
-- ----------------------------
DROP TABLE IF EXISTS `vehicles`;
CREATE TABLE `vehicles`  (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `OwnerID` int(11) NOT NULL,
  `Model` int(11) UNSIGNED NOT NULL,
  `PrimaryColor` int(11) NULL DEFAULT NULL,
  `SecondaryColor` int(11) NULL DEFAULT NULL,
  `PlateText` varchar(8) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_520_ci NOT NULL DEFAULT 'STATIQUE',
  `PlateStyle` tinyint(4) NULL DEFAULT NULL,
  PRIMARY KEY (`ID`) USING BTREE,
  INDEX `fkey_vehicle_to_charid_idx`(`OwnerID`) USING BTREE,
  CONSTRAINT `fkey_vehicle_to_charid` FOREIGN KEY (`OwnerID`) REFERENCES `characters` (`ID`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 55 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_520_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for world
-- ----------------------------
DROP TABLE IF EXISTS `world`;
CREATE TABLE `world`  (
  `id` int(11) NOT NULL,
  `ServerTime` datetime(0) NOT NULL DEFAULT CURRENT_TIMESTAMP(0),
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_520_ci COMMENT = '	' ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
