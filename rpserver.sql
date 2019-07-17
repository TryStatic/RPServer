/*
 Navicat MySQL Data Transfer

 Source Server         : localhost_3306
 Source Server Type    : MySQL
 Source Server Version : 80016
 Source Host           : localhost:3306
 Source Schema         : rpserver

 Target Server Type    : MySQL
 Target Server Version : 80016
 File Encoding         : 65001

 Date: 17/07/2019 21:10:33
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
) ENGINE = InnoDB AUTO_INCREMENT = 242 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for appearances
-- ----------------------------
DROP TABLE IF EXISTS `appearances`;
CREATE TABLE `appearances`  (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `CharacterID` int(11) NOT NULL,
  `SkinModel` bigint(4) NOT NULL,
  `IsMale` tinyint(4) NOT NULL,
  `ShapeFirstID` tinyint(4) NOT NULL,
  `ShapeSecondID` tinyint(4) NOT NULL,
  `ShapeMix` float NOT NULL,
  `SkinMix` float NOT NULL,
  `Blemishes` tinyint(4) NOT NULL,
  `FacialHair` tinyint(4) NOT NULL,
  `Eyebrows` tinyint(4) NOT NULL,
  `Ageing` tinyint(4) NOT NULL,
  `Makeup` tinyint(4) NOT NULL,
  `Blush` tinyint(4) NOT NULL,
  `Complexion` tinyint(4) NOT NULL,
  `SunDamage` tinyint(4) NOT NULL,
  `Lipstick` tinyint(4) NOT NULL,
  `Freckles` tinyint(4) NOT NULL,
  `ChestHair` tinyint(4) NOT NULL,
  `BodyBlemishes` tinyint(4) NOT NULL,
  `AdditionalBodyBlemishes` tinyint(4) NOT NULL,
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
  `BlemishesColor` tinyint(4) NOT NULL,
  `FacialHairColor` tinyint(4) NOT NULL,
  `EyebrowsColor` tinyint(4) NOT NULL,
  `AgeingColor` tinyint(4) NOT NULL,
  `MakeupColor` tinyint(4) NOT NULL,
  `BlushColor` tinyint(4) NOT NULL,
  `ComplexionColor` tinyint(4) NOT NULL,
  `SunDamageColor` tinyint(4) NOT NULL,
  `LipstickColor` tinyint(4) NOT NULL,
  `FrecklesColor` tinyint(4) NOT NULL,
  `ChestHairColor` tinyint(4) NOT NULL,
  `BodyBlemishesColor` tinyint(4) NOT NULL,
  `AdditionalBodyBlemishesColor` tinyint(4) NOT NULL,
  `BlemishesSecColor` tinyint(4) NOT NULL,
  `FacialHairSecColor` tinyint(4) NOT NULL,
  `EyebrowsSecColor` tinyint(4) NOT NULL,
  `AgeingSecColor` tinyint(4) NOT NULL,
  `MakeupSecColor` tinyint(4) NOT NULL,
  `BlushSecColor` tinyint(4) NOT NULL,
  `ComplexionSecColor` tinyint(4) NOT NULL,
  `SunDamageSecColor` tinyint(4) NOT NULL,
  `LipstickSecColor` tinyint(4) NOT NULL,
  `FrecklesSecColor` tinyint(4) NOT NULL,
  `ChestHairSecColor` tinyint(4) NOT NULL,
  `BodyBlemishesSecColor` tinyint(4) NOT NULL,
  `AdditionalBodyBlemishesSecColor` tinyint(4) NOT NULL,
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
  `EyeColor` tinyint(4) NOT NULL,
  `HairHighlightColor` tinyint(4) NOT NULL,
  `HairColor` tinyint(4) NOT NULL,
  `HairStyle` tinyint(4) NOT NULL,
  `HairStyleTexture` tinyint(4) NOT NULL,
  PRIMARY KEY (`ID`, `CharacterID`) USING BTREE,
  UNIQUE INDEX `CharacterID_UNIQUE`(`CharacterID`) USING BTREE,
  INDEX `fkey_char_to_appearance_idx`(`CharacterID`) USING BTREE,
  CONSTRAINT `fkey_char_to_appearance` FOREIGN KEY (`CharacterID`) REFERENCES `characters` (`ID`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_520_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for characters
-- ----------------------------
DROP TABLE IF EXISTS `characters`;
CREATE TABLE `characters`  (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `CharOwnerID` int(11) NOT NULL,
  `CharacterName` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE INDEX `name_UNIQUE`(`CharacterName`) USING BTREE,
  INDEX `fkey_idx`(`CharOwnerID`) USING BTREE,
  CONSTRAINT `fkey_acc_to_char` FOREIGN KEY (`CharOwnerID`) REFERENCES `accounts` (`ID`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 14 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

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

SET FOREIGN_KEY_CHECKS = 1;
