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

 Date: 29/06/2019 03:55:27
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for accounts
-- ----------------------------
DROP TABLE IF EXISTS `accounts`;
CREATE TABLE `accounts`  (
  `accountID` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `emailaddress` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `hash` varbinary(64) NOT NULL,
  `forumname` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `nickname` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `regsocialclubname` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `lastsocialclubname` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `LastIP` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `LastHWID` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `creationdate` datetime(0) NOT NULL,
  `lastlogindate` datetime(0) NULL DEFAULT NULL,
  `enabled2FAbyemail` tinyint(4) NOT NULL DEFAULT 0,
  `twofactorsharedkey` varbinary(64) NULL DEFAULT NULL,
  PRIMARY KEY (`accountID`) USING BTREE,
  UNIQUE INDEX `username_UNIQUE`(`username`) USING BTREE,
  UNIQUE INDEX `emailaddress_UNIQUE`(`emailaddress`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 231 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for characters
-- ----------------------------
DROP TABLE IF EXISTS `characters`;
CREATE TABLE `characters`  (
  `characterID` int(11) NOT NULL AUTO_INCREMENT,
  `charownerID` int(11) NOT NULL,
  `charname` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `skinmodel` int(11) UNSIGNED NOT NULL DEFAULT 1885233650,
  PRIMARY KEY (`characterID`) USING BTREE,
  UNIQUE INDEX `name_UNIQUE`(`charname`) USING BTREE,
  INDEX `fkey_idx`(`charownerID`) USING BTREE,
  CONSTRAINT `fkey_acc_to_char` FOREIGN KEY (`charownerID`) REFERENCES `accounts` (`accountID`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

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
  CONSTRAINT `fkey_acc_to_emailtoken` FOREIGN KEY (`accountID`) REFERENCES `accounts` (`accountID`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
