DROP TABLE IF EXISTS hvac;

--create the hvac table on comma-separated sensor data
CREATE EXTERNAL TABLE hvac(date STRING, time STRING, targettemp BIGINT,
    actualtemp BIGINT, 
    system BIGINT, 
    systemage BIGINT, 
    buildingid BIGINT)
ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' 
STORED AS TEXTFILE LOCATION 'wasb://uba@ubastorage.blob.core.chinacloudapi.cn/HdiSamples/SensorSampleData/hvac/';

DROP TABLE IF EXISTS building;

--create the building table on comma-separated building data
CREATE EXTERNAL TABLE building(buildingid BIGINT, buildingmgr STRING, 
    buildingage BIGINT, 
    hvacproduct STRING,
    country STRING) 
ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' 
STORED AS TEXTFILE LOCATION 'wasb://uba@ubastorage.blob.core.chinacloudapi.cn/HdiSamples/SensorSampleData/building/';

DROP TABLE IF EXISTS hvac_temperatures;

--create the hvac_temperatures table by selecting from the hvac table
CREATE TABLE hvac_temperatures AS
SELECT *, targettemp - actualtemp AS temp_diff, 
    IF((targettemp - actualtemp) > 5, 'COLD', 
    IF((targettemp - actualtemp) < -5, 'HOT', 'NORMAL')) AS temprange, 
    IF((targettemp - actualtemp) > 5, '1', IF((targettemp - actualtemp) < -5, '1', 0)) AS extremetemp
FROM hvac;

DROP TABLE IF EXISTS hvac_building;

--create the hvac_building table by joining the building table and the hvac_temperatures table
CREATE TABLE hvac_building AS 
SELECT h.*, b.country, b.hvacproduct, b.buildingage, b.buildingmgr
FROM building b JOIN hvac_temperatures h ON b.buildingid = h.buildingid;

