/*
Copyright (C) 2012 Convolved Software

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

CREATE TABLE [Log]
(
	[Id] bigint NOT NULL IDENTITY(1, 1)
		CONSTRAINT PK_Log PRIMARY KEY CLUSTERED,
	[Date] datetimeoffset NOT NULL,
	[Thread] nvarchar(255) NOT NULL,
	[EventType] nvarchar(50) NOT NULL,
	[ServerName] nvarchar(50) NOT NULL,
	[ApplicationName] nvarchar(50) NOT NULL,
	[ComponentName] nvarchar(255) NOT NULL,
	[Message] nvarchar(max) NOT NULL,
	[Details] nvarchar(max) NULL
)

CREATE INDEX IX_Log_Date_EventType_ApplicationName_ComponentName
ON [Log] ([Date], [EventType], [ApplicationName], [ComponentName])
INCLUDE ([ServerName], [Thread], [Message])

CREATE INDEX IX_Log_Date_ServerName_ApplicationName_ComponentName
ON [Log] ([Date], [ServerName], [ApplicationName], [ComponentName])
INCLUDE ([EventType], [Thread], [Message])

CREATE INDEX IX_Log_Date_ApplicationName_ComponentName_EventType
ON [Log] ([Date], [ApplicationName], [ComponentName], [EventType])
INCLUDE ([ServerName], [Thread], [Message])

CREATE INDEX IX_Log_Date_ComponentName_EventType
ON [Log] ([Date], [ComponentName], [EventType])
INCLUDE ([ServerName], [ApplicationName], [Thread], [Message])

CREATE INDEX IX_Log_ServerName_ApplicationName_ComponentName
ON [Log] ([ServerName], [ApplicationName], [ComponentName])
INCLUDE ([Date], [EventType], [Thread], [Message])

CREATE INDEX IX_Log_ApplicationName_ComponentName_EventType
ON [Log] ([ApplicationName], [ComponentName], [EventType])
INCLUDE ([Date], [ServerName], [Thread], [Message])

CREATE INDEX IX_Log_ComponentName_EventType
ON [Log] ([ComponentName], [EventType])
INCLUDE ([Date], [ServerName], [ApplicationName], [Thread], [Message])