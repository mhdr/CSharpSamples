/*
Navicat SQLite Data Transfer

Source Server         : mGetDb
Source Server Version : 30808
Source Host           : :0

Target Server Type    : SQLite
Target Server Version : 30808
File Encoding         : 65001

Date: 2015-07-17 19:16:39
*/

PRAGMA foreign_keys = OFF;

-- ----------------------------
-- Table structure for Downloads
-- ----------------------------
DROP TABLE IF EXISTS "main"."Downloads";
CREATE TABLE "Downloads" (
"DownloadId"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"Url"  TEXT NOT NULL,
"FileName"  TEXT NOT NULL,
"FileSize"  INTEGER,
"Status"  INTEGER NOT NULL,
"Progress"  REAL,
"AddedDate"  TEXT NOT NULL
);
