DROP TABLE IF EXISTS Uba_Raw;
CREATE EXTERNAL TABLE Uba_Raw(json_response STRING)
STORED AS TEXTFILE LOCATION 'wasb://test@ubastorage.blob.core.chinacloudapi.cn/';
DROP TABLE IF EXISTS Uba_Tweets;
CREATE TABLE Uba_Tweets(Appid STRING,Ip string);
ROW FORMAT DELIMITED;
FIELDS TERMINATED BY '\f';
COLLECTION ITEMS TERMINATED BY '|';
from Uba_Raw
INSERT OVERWRITE TABLE Uba_Tweets
select 
 CAST(get_json_object(json_response, '$.base') as STRING),

select * from Uba_Tweets limit 100