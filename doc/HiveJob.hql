
DROP TABLE IF EXISTS Uba_BaseInfo;
CREATE TABLE Uba_BaseInfo(Appid STRING,Ip string,hd string,operator string,dr string,ch string,version string,fm string,
	os string,browser string,bversion string,ulev int,checkin TINYINT,fheadimge TINYINT,
	fsch TINYINT,fqq TINYINT,fmobile TINYINT,fse TINYINT,fgpk TINYINT, fpk TINYINT,
	umemo3 string,umemo4 string,umemo5 string, userid bigint ,
	sex string,grade string,qq tinyint, mobile tinyint,sch string,frs int,mdou int,sta TINYINT,kb TINYINT,rtm string,
	amemo1 string,amemo2 string,amemo3 string,amemo4 string,amemo5 string, actid string,atime date
	)
ROW FORMAT DELIMITED FIELDS TERMINATED BY '\\\\'
STORED AS TEXTFILE LOCATION 'wasb://uba@ubastorage.blob.core.chinacloudapi.cn/';
DROP TABLE IF EXISTS Uba_UserInfo;
CREATE TABLE Uba_UserInfo(userid bigint,sex string,ulev int,grade string,sta TINYINT,sch string,frs int,qq tinyint,mobile tinyint,rtm string);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_UserInfo
select distinct userid,sex,ulev,grade,sta,sch,frs,qq,mobile,rtm where userid is not null and sex !='';
--a001
Drop table if exists Uba_Acta001;
create table Uba_Acta001(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta001
select userid,count(1) where actid='a001' and userid is not null group by userid;
--a002
Drop table if exists Uba_Acta002;
create table Uba_Acta002(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta002
select userid,count(1) where actid='a002' and userid is not null group by userid;
--a003
Drop table if exists Uba_Acta003;
create table Uba_Acta003(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta003
select userid,count(1) where actid='a003' and userid is not null group by userid;
--a004
Drop table if exists Uba_Acta004;
create table Uba_Acta004(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta004
select userid,count(1) where actid='a004' and userid is not null group by userid;
--a005
Drop table if exists Uba_Acta005;
create table Uba_Acta005(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta005
select userid,count(1) where actid='a005' and userid is not null group by userid;
--a006
Drop table if exists Uba_Acta006;
create table Uba_Acta006(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta006
select userid,count(1) where actid='a006' and userid is not null group by userid;
--a007
Drop table if exists Uba_Acta007;
create table Uba_Acta007(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta007
select userid,count(1) where actid='a007' and userid is not null group by userid;
--a008
Drop table if exists Uba_Acta008;
create table Uba_Acta008(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta008
select userid,count(1) where actid='a008' and userid is not null group by userid;
--a009
Drop table if exists Uba_Acta009;
create table Uba_Acta009(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta009
select userid,count(1) where actid='a009' and userid is not null group by userid;
--a010
Drop table if exists Uba_Acta010;
create table Uba_Acta010(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta010
select userid,count(1) where actid='a010' and userid is not null group by userid;
--a011
Drop table if exists Uba_Acta011;
create table Uba_Acta011(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta011
select userid,count(1) where actid='a011' and userid is not null group by userid;
--a012
Drop table if exists Uba_Acta012;
create table Uba_Acta012(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta012
select userid,count(1) where actid='a012' and userid is not null group by userid;
--a013
Drop table if exists Uba_Acta013;
create table Uba_Acta013(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta013
select userid,count(1) where actid='a013' and userid is not null group by userid;
--a014
Drop table if exists Uba_Acta014;
create table Uba_Acta014(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta014
select userid,count(1) where actid='a014' and userid is not null group by userid;
--a015
Drop table if exists Uba_Acta015;
create table Uba_Acta015(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta015
select userid,count(1) where actid='a015' and userid is not null group by userid;
--a016
Drop table if exists Uba_Acta016;
create table Uba_Acta016(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta016
select userid,count(1) where actid='a016' and userid is not null group by userid;
--a017
Drop table if exists Uba_Acta017;
create table Uba_Acta017(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta017
select userid,count(1) where actid='a017' and userid is not null group by userid;
--a018
Drop table if exists Uba_Acta018;
create table Uba_Acta018(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta018
select userid,count(1) where actid='a018' and userid is not null group by userid;
--a019
Drop table if exists Uba_Acta019;
create table Uba_Acta019(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta019
select userid,count(1) where actid='a019' and userid is not null group by userid;
--a020
Drop table if exists Uba_Acta020;
create table Uba_Acta020(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta020
select userid,count(1) where actid='a020' and userid is not null group by userid;
--a021
Drop table if exists Uba_Acta021;
create table Uba_Acta021(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta021
select userid,count(1) where actid='a021' and userid is not null group by userid;
--a022
Drop table if exists Uba_Acta022;
create table Uba_Acta022(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta022
select userid,count(1) where actid='a022' and userid is not null group by userid;
--a033
Drop table if exists Uba_Acta033;
create table Uba_Acta033(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta033
select userid,count(1) where actid='a033' and userid is not null group by userid;
--a034
Drop table if exists Uba_Acta034;
create table Uba_Acta034(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta034
select userid,count(1) where actid='a034' and userid is not null group by userid;
--a035
Drop table if exists Uba_Acta035;
create table Uba_Acta035(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta035
select userid,count(1) where actid='a035' and userid is not null group by userid;
--a036
Drop table if exists Uba_Acta036;
create table Uba_Acta036(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta036
select userid,count(1) where actid='a036' and userid is not null group by userid;
--a037
Drop table if exists Uba_Acta037;
create table Uba_Acta037(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta037
select userid,count(1) where actid='a037' and userid is not null group by userid;
--a038
Drop table if exists Uba_Acta038;
create table Uba_Acta038(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta038
select userid,count(1) where actid='a038' and userid is not null group by userid;
--a039
Drop table if exists Uba_Acta039;
create table Uba_Acta039(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta039
select userid,count(1) where actid='a039' and userid is not null group by userid;
--a040
Drop table if exists Uba_Acta040;
create table Uba_Acta040(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta040
select userid,count(1) where actid='a040' and userid is not null group by userid;
--a041
Drop table if exists Uba_Acta041;
create table Uba_Acta041(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta041
select userid,count(1) where actid='a041' and userid is not null group by userid;
--a042
Drop table if exists Uba_Acta042;
create table Uba_Acta042(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta042
select userid,count(1) where actid='a042' and userid is not null group by userid;
--a043
Drop table if exists Uba_Acta043;
create table Uba_Acta043(userid bigint,ActTotal int);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_Acta043
select userid,count(1) where actid='a043' and userid is not null group by userid;
--result
select count(1) from Uba_UserInfo 
   left JOIN Uba_Acta001 ON (Uba_UserInfo.userid = Uba_Acta001.userid)
   left JOIN Uba_Acta002 ON (Uba_UserInfo.userid = Uba_Acta002.userid)
   left JOIN Uba_Acta003 ON (Uba_UserInfo.userid = Uba_Acta003.userid)
   left JOIN Uba_Acta004 ON (Uba_UserInfo.userid = Uba_Acta004.userid)
   left JOIN Uba_Acta005 ON (Uba_UserInfo.userid = Uba_Acta005.userid)
   left JOIN Uba_Acta006 ON (Uba_UserInfo.userid = Uba_Acta006.userid)
   left JOIN Uba_Acta007 ON (Uba_UserInfo.userid = Uba_Acta007.userid)
   left JOIN Uba_Acta008 ON (Uba_UserInfo.userid = Uba_Acta008.userid)
   left JOIN Uba_Acta009 ON (Uba_UserInfo.userid = Uba_Acta009.userid)
   left JOIN Uba_Acta010 ON (Uba_UserInfo.userid = Uba_Acta010.userid)
   left JOIN Uba_Acta011 ON (Uba_UserInfo.userid = Uba_Acta011.userid)
   left JOIN Uba_Acta012 ON (Uba_UserInfo.userid = Uba_Acta012.userid)
   left JOIN Uba_Acta013 ON (Uba_UserInfo.userid = Uba_Acta013.userid)
   left JOIN Uba_Acta014 ON (Uba_UserInfo.userid = Uba_Acta014.userid)
   left JOIN Uba_Acta015 ON (Uba_UserInfo.userid = Uba_Acta015.userid)
   left JOIN Uba_Acta016 ON (Uba_UserInfo.userid = Uba_Acta016.userid)
   left JOIN Uba_Acta017 ON (Uba_UserInfo.userid = Uba_Acta017.userid)
   left JOIN Uba_Acta018 ON (Uba_UserInfo.userid = Uba_Acta018.userid)
   left JOIN Uba_Acta019 ON (Uba_UserInfo.userid = Uba_Acta019.userid)
   left JOIN Uba_Acta020 ON (Uba_UserInfo.userid = Uba_Acta020.userid)
   left JOIN Uba_Acta021 ON (Uba_UserInfo.userid = Uba_Acta021.userid)
   left JOIN Uba_Acta022 ON (Uba_UserInfo.userid = Uba_Acta022.userid)
   left JOIN Uba_Acta033 ON (Uba_UserInfo.userid = Uba_Acta033.userid)
   left JOIN Uba_Acta034 ON (Uba_UserInfo.userid = Uba_Acta034.userid)
   left JOIN Uba_Acta035 ON (Uba_UserInfo.userid = Uba_Acta035.userid)
   left JOIN Uba_Acta036 ON (Uba_UserInfo.userid = Uba_Acta036.userid)
   left JOIN Uba_Acta037 ON (Uba_UserInfo.userid = Uba_Acta037.userid)
   left JOIN Uba_Acta038 ON (Uba_UserInfo.userid = Uba_Acta038.userid)
   left JOIN Uba_Acta039 ON (Uba_UserInfo.userid = Uba_Acta039.userid)
   left JOIN Uba_Acta040 ON (Uba_UserInfo.userid = Uba_Acta040.userid)
   left JOIN Uba_Acta041 ON (Uba_UserInfo.userid = Uba_Acta041.userid)
   left JOIN Uba_Acta042 ON (Uba_UserInfo.userid = Uba_Acta042.userid)
   left JOIN Uba_Acta043 ON (Uba_UserInfo.userid = Uba_Acta043.userid);