/*
Copyright (C) 2012 Convolved Software

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

CREATE TABLE [LogStatistics]
(
	[Id] int NOT NULL IDENTITY(1, 1)
		CONSTRAINT PK_LogStatistics PRIMARY KEY CLUSTERED,
	[Date] date NOT NULL,
	[Year] AS YEAR([Date]) PERSISTED,
	[Month] AS MONTH([Date]) PERSISTED,
	[Day] AS DAY([Date]) PERSISTED,
	[Hour] tinyint NOT NULL,
	[EventType] nvarchar(50) NOT NULL,
	[ServerName] nvarchar(50) NOT NULL,
	[ApplicationName] nvarchar(50) NOT NULL,
	[ComponentName] nvarchar(255) NOT NULL,
	[Count] int NOT NULL
)

CREATE INDEX IX_LogStatistics_Date_EventType_ApplicationName
ON [LogStatistics] ([Date], [EventType], [ApplicationName])
INCLUDE ([Year], [Month], [Day], [Hour], [ServerName], [ComponentName], [Count])

CREATE INDEX IX_LogStatistics_Date_ServerName_ApplicationName
ON [LogStatistics] ([Date], [ServerName], [ApplicationName])
INCLUDE ([Year], [Month], [Day], [Hour], [EventType], [ComponentName], [Count])

CREATE INDEX IX_LogStatistics_Date_ApplicationName_ComponentName
ON [LogStatistics] ([Date], [ApplicationName], [ComponentName])
INCLUDE ([Year], [Month], [Day], [Hour], [EventType], [ServerName], [Count])

CREATE INDEX IX_LogStatistics_Calendar_ApplicationName_ComponentName
ON [LogStatistics] ([Year], [Month], [Day], [Hour], [ApplicationName], [ComponentName])
INCLUDE ([Date], [EventType], [ServerName], [Count])

CREATE INDEX IX_LogStatistics_EventType_ServerName_ApplicationName
ON [LogStatistics] ([EventType], [ServerName], [ApplicationName])
INCLUDE ([Date], [Year], [Month], [Day], [Hour], [ComponentName], [Count])

CREATE INDEX IX_LogStatistics_EventType_ApplicationName_ComponentName
ON [LogStatistics] ([EventType], [ApplicationName], [ComponentName])
INCLUDE ([Date], [Year], [Month], [Day], [Hour], [ServerName], [Count])

CREATE INDEX IX_LogStatistics_ServerName_ApplicationName_ComponentName
ON [LogStatistics] ([ServerName], [ApplicationName], [ComponentName])
INCLUDE ([Date], [Year], [Month], [Day], [Hour], [EventType], [Count])

CREATE INDEX IX_LogStatistics_ApplicationName_ComponentName
ON [LogStatistics] ([ApplicationName], [ComponentName])
INCLUDE ([Date], [Year], [Month], [Day], [Hour], [EventType], [ServerName], [Count])