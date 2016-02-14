DROP TABLE IF EXISTS Uba_BaseInfo;
CREATE TABLE Uba_BaseInfo(Appid STRING,Ip string,hd string,operator string,dr string,ch string,version string,fm string,
	os string,browser string,bversion string,ulev int,checkin TINYINT,fheadimge TINYINT,
	fsch TINYINT,fqq TINYINT,fmobile TINYINT,fse TINYINT,fgpk TINYINT, fpk TINYINT,
	umemo3 string,umemo4 string,umemo5 string, userid bigint ,
	sex string,grade string,qq tinyint, mobile tinyint,sch string,frs int,mdou int,sta TINYINT,kb TINYINT,rtm TIMESTAMP,
	amemo1 string,amemo2 string,amemo3 string,amemo4 string,amemo5 string, actid string,atime bigint
	)
ROW FORMAT DELIMITED FIELDS TERMINATED BY '\\\\'
STORED AS TEXTFILE LOCATION 'wasb://uba@ubastorage.blob.core.chinacloudapi.cn/';
--query atime
DROP TABLE IF EXISTS Uba_UserAtime;
CREATE TABLE Uba_UserAtime(userid bigint,atime bigint,adate string);
FROM Uba_BaseInfo
INSERT OVERWRITE TABLE Uba_UserAtime
select  userid,max(atime),from_unixtime(atime,'yyyy-MM-dd') where userid is not null and userid>0 and sex !=''
 group by userid,from_unixtime(atime,'yyyy-MM-dd');
--query  all info 
select t1.userid,t1.sex,t1.ulev,t1.grade,t1.sta,t1.sch,t1.frs,t1.qq,t1.mobile,t1.rtm,t1.atime,t1.mdou,t1.kb,t2.adate
  from Uba_BaseInfo t1  JOIN Uba_UserAtime t2 ON (t1.userid = t2.userid and t1.atime=t2.atime);