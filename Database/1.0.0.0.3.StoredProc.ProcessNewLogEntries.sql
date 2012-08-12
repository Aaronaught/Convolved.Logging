/*
Copyright (C) 2012 Convolved Software

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

CREATE PROCEDURE ProcessNewLogEntries
AS

BEGIN TRAN

SET NOCOUNT ON

DECLARE @NewLogEntries TABLE
(
	[Date] datetimeoffset NOT NULL,
	[Thread] nvarchar(255) NOT NULL,
	[EventType] nvarchar(50) NOT NULL,
	[ServerName] nvarchar(50) NOT NULL,
	[ApplicationName] nvarchar(50) NOT NULL,
	[ComponentName] nvarchar(255) NOT NULL,
	[Message] nvarchar(max) NOT NULL,
	[Details] nvarchar(max) NULL
)

DELETE FROM [LogStaging]
OUTPUT
	deleted.[Date],
	deleted.[Thread],
	deleted.[EventType],
	deleted.[ServerName],
	deleted.[ApplicationName],
	deleted.[ComponentName],
	deleted.[Message],
	deleted.[Details]
	INTO @NewLogEntries
	
IF NOT EXISTS
(
	SELECT 1
	FROM @NewLogEntries
)
BEGIN
	SELECT 0 AS InsertedCount
	
	COMMIT
	RETURN
END

DECLARE @InsertedLogEntries TABLE
(
	Id bigint NOT NULL PRIMARY KEY
)

INSERT [Log] ([Date], [Thread], [EventType], [ServerName], [ApplicationName], [ComponentName], [Message], [Details])
	OUTPUT inserted.Id INTO @InsertedLogEntries
SELECT [Date], [Thread], [EventType], [ServerName], [ApplicationName], [ComponentName], [Message], [Details]
FROM @NewLogEntries

DECLARE
	@LowId int,
	@HighId int

SELECT
	@LowId = MIN(Id),
	@HighId = MAX(Id)
FROM @InsertedLogEntries;

WITH LogStatisticUpdates AS
(
	SELECT
		CAST([Date] AS date) AS [Date],
		DATEPART(HOUR, [Date]) AS [Hour],
		[ServerName],
		[ApplicationName],
		[ComponentName],
		[EventType],
		COUNT(*) AS [Count]
	FROM [Log]
	WHERE Id >= @LowId
	AND Id <= @HighId
	GROUP BY
		CAST([Date] AS date),
		DATEPART(HOUR, [Date]),
		[ServerName],
		[ApplicationName],
		[ComponentName],
		[EventType]
)
MERGE LogStatistics s
USING LogStatisticUpdates u
	ON s.[Date] = u.[Date]
	AND s.[Hour] = u.[Hour]
	AND s.[EventType] = u.[EventType]
	AND s.[ServerName] = u.[ServerName]
	AND s.[ApplicationName] = u.[ApplicationName]
	AND s.[ComponentName] = u.[ComponentName]
WHEN MATCHED THEN
	UPDATE SET s.[Count] = s.[Count] + u.[Count]
WHEN NOT MATCHED THEN
	INSERT ([Date], [Hour], [ServerName], [ApplicationName], [ComponentName], [EventType], [Count])
	VALUES (u.[Date], u.[Hour], u.[ServerName], u.[ApplicationName], u.[ComponentName], u.[EventType], u.[Count]);

SELECT COUNT(*) AS InsertedCount
FROM @InsertedLogEntries
	
COMMIT