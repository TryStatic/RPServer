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

 Date: 06/07/2019 20:52:00
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for accounts
-- ----------------------------
DROP TABLE IF EXISTS `accounts`;
CREATE TABLE `accounts`  (
  `AccountID` int(11) NOT NULL AUTO_INCREMENT,
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
  PRIMARY KEY (`AccountID`) USING BTREE,
  UNIQUE INDEX `username_UNIQUE`(`Username`) USING BTREE,
  UNIQUE INDEX `emailaddress_UNIQUE`(`EmailAddress`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 231 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for characters
-- ----------------------------
DROP TABLE IF EXISTS `characters`;
CREATE TABLE `characters`  (
  `characterID` int(11) NOT NULL AUTO_INCREMENT,
  `charownerID` int(11) NOT NULL,
  `charname` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `customization` varchar(2500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`characterID`) USING BTREE,
  UNIQUE INDEX `name_UNIQUE`(`charname`) USING BTREE,
  INDEX `fkey_idx`(`charownerID`) USING BTREE,
  CONSTRAINT `fkey_acc_to_char` FOREIGN KEY (`charownerID`) REFERENCES `accounts` (`AccountID`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

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
  CONSTRAINT `fkey_acc_to_emailtoken` FOREIGN KEY (`accountID`) REFERENCES `accounts` (`AccountID`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
