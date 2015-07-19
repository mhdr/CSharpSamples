/*
Navicat SQLite Data Transfer

Source Server         : mGetDb
Source Server Version : 30808
Source Host           : :0

Target Server Type    : SQLite
Target Server Version : 30808
File Encoding         : 65001

Date: 2015-07-17 19:16:47
*/

PRAGMA foreign_keys = OFF;

-- ----------------------------
-- Table structure for Info
-- ----------------------------
DROP TABLE IF EXISTS "main"."Info";
CREATE TABLE "Info" (
"InfoId"  INTEGER NOT NULL,
"Key"  TEXT NOT NULL,
"Value"  TEXT,
PRIMARY KEY ("InfoId" ASC)
);
