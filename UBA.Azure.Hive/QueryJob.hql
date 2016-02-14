DROP TABLE IF EXISTS ubalogs;
CREATE EXTERNAL TABLE IF NOT EXISTS ubalogs(appid int, ip string,hb string,operator int,dr string,ch string,version string,
fm int,os string,browser string,bversion string,umemo1 STRING,umemo2 STRING,umemo3 STRING,umemo4 STRING,umemo5 STRING,userid BIGINT,usex STRING) 
ROW FORMAT DELIMITED FIELDS TERMINATED BY '\\' 
STORED AS TEXTFILE LOCATION 'wasb://test@ubastorage.blob.core.chinacloudapi.cn/' ;
select userid,count(1) from ubalogs group by userid;
select * from ubalogs limit 10