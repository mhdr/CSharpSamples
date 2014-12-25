/*
Navicat SQLite Data Transfer

Source Server         : test
Source Server Version : 30802
Source Host           : :0

Target Server Type    : SQLite
Target Server Version : 30802
File Encoding         : 65001

Date: 2014-12-25 09:30:36
*/

PRAGMA foreign_keys = OFF;

-- ----------------------------
-- Table structure for People
-- ----------------------------
DROP TABLE IF EXISTS "main"."People";
CREATE TABLE "People" (
"PersonId"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"FirstName"  TEXT,
"Age"  INTEGER NOT NULL
);
