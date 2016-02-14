--Part 1: Introduction 
-- In this sample you will use an HDInsight query that analyzes website log files to get insight into how customers use the website. With this analysis, you can see the frequency of visits to the website in a day from external websites, and a summary of website errors that the users experience. 
-- In this tutorial, you'll learn how to use HDInsight to: 
-- *Connect to an Azure Storage Blob containing website log files 
-- *Create Hive tables to query those logs 
-- *Create Hive queries to analyze the data 

--Part 2: Prerequisites
-- The script here is only for your reference. You need to log in to https://yourclusteraddress.azurehdinsight.net/ in order to trigger the HDInsight service to load the sample data. 
-- For clusters provisioned before 10/15/2014, you could go to http://azure.microsoft.com/en-us/documentation/services/hdinsight/ for more details since those clusters do not have the sample data installed.

--Part 3: Website Log Data Loaded into Windows Azure Storage Blob 
-- The data stored in Windows Azure Storage Blob can be accessed by expanding a HDInsight cluster and double clicking the default container of your default storage account. The data for this sample can be found under the /HdiSamples/WebsiteLogSampleData/SampleLog path in your default container. 

--Part 4: Creating Hive table to Query Website Log Data 
-- The following Hive statement creates an external table that allows Hive to query data stored in Azure Blob Storage. External tables preserve the data in the original file format, while allowing Hive to perform queries against the data within the file. 
-- The Hive statement below creates a new table, named weblogs, by describing the fields within the files, the delimiter between fields, and the location of the file in Azure Blob Storage. In the Creating Hive Queries to Analyze Data section of this tutorial, you will perform queries on the data stored in this table. 
-- You could also create a table by right clicking on a certain database and select "Create Table". We will provide you a UI to help you to create such a table.

DROP TABLE IF EXISTS weblogs; 
-- create table weblogs on space-delimited website log data. 
-- In this sample we will use the default container. You could also use 'wasb://[container]@[storage account].blob.core.windows.net/Path/To/Data/' to access the data in other containers.
CREATE EXTERNAL TABLE IF NOT EXISTS weblogs(s_date date, s_time string, s_sitename string, cs_method string, cs_uristem string, 
			cs_uriquery string, s_port int, cs_username string, c_ip string, cs_useragent string, 
			cs_cookie string, cs_referer string, cs_host string, sc_status int, sc_substatus int,
			sc_win32status int, sc_bytes int, cs_bytes int, s_timetaken int ) 
ROW FORMAT DELIMITED FIELDS TERMINATED BY ' '
STORED AS TEXTFILE LOCATION '/HdiSamples/WebsiteLogSampleData/SampleLog/'
TBLPROPERTIES ('skip.header.line.count'='2');


-- The following HIVE queries create two new tables based on the queries run on the weblogs table. The new tables are called clienterrors and refersperday. 
-- The query for clienterrors extracts data from the weblogs table for HTTP status codes between 400 and 500, and groups them by the users facing those errors and the type of error codes. The range of status code between 400 and 500, represented by sc_status column in the weblogs table, corresponds to the errors clients get while accessing the website. The extracted data is then sorted on the number of occurrences of each error code and written to the clienterrors table. 
-- The query for refersperday extracts data from the weblogs table for all external websites referencing this website. The external website information is extracted from the cs_referer column of weblogs table. To make sure the referring links did not encounter an error, the table only shows data for pages that returned an HTTP status code between 200 and 300. The extracted data is then written to the refersperday table. 

DROP TABLE IF EXISTS ClientErrors; 
-- create table ClientErrors for storing errors users experienced and their frequencies
CREATE EXTERNAL TABLE ClientErrors(sc_status int, cs_referer string, cs_page string, cnt int)
ROW FORMAT DELIMITED FIELDS TERMINATED BY ',';

-- populate table ClientErrors with data from table weblogs
INSERT OVERWRITE TABLE ClientErrors 
SELECT sc_status, cs_referer, 
		concat(cs_uristem,'?', regexp_replace(cs_uriquery,'X-ARR-LOG-ID=[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}','')) cs_page,
		count(distinct c_ip) as cnt 
FROM weblogs 
WHERE sc_status >=400 and sc_status < 500
GROUP BY sc_status, cs_referer, concat(cs_uristem,'?', regexp_replace(cs_uriquery,'X-ARR-LOG-ID=[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}',''))
ORDER BY cnt;

DROP TABLE IF EXISTS RefersPerDay;
-- create table RefersPerDay for storing references from external websites
CREATE EXTERNAL TABLE IF NOT EXISTS RefersPerDay(year int, month int, day int, cs_referer string, cnt int)
ROW FORMAT DELIMITED FIELDS TERMINATED BY ',';

-- populate table RefersPerDay with data from the weblogs table
INSERT OVERWRITE TABLE RefersPerDay
SELECT year(s_date), month(s_date), day(s_date), cs_referer, count(distinct c_ip) as cnt
FROM weblogs
WHERE sc_status >=200 and sc_status <300
GROUP BY s_date, cs_referer
ORDER BY cnt desc;

--Part 6: Executing Queries and view the results
-- Select Submit/Submit(Advanced) in the HDInsight toolbar to execute the queries. You can also use Alt+Shift+S for a quick submission. After submitting the job, you can view details by right clicking on the cluster and select "View Hive Jobs". 
-- You can also expand the Hive databases and right click on the tables you just created, select "View Top 100 Rows" and sample the table you just created. 