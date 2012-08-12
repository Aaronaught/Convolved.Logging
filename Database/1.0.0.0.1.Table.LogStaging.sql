/*
Copyright (C) 2012 Convolved Software

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

CREATE TABLE [LogStaging]
(
	[Id] bigint NOT NULL,
	[Date] datetimeoffset NOT NULL,
	[Thread] nvarchar(255) NOT NULL,
	[EventType] nvarchar(50) NOT NULL,
	[ServerName] nvarchar(50) NOT NULL,
	[ApplicationName] nvarchar(50) NOT NULL,
	[ComponentName] nvarchar(255) NOT NULL,
	[Message] nvarchar(max) NOT NULL,
	[Details] nvarchar(max)
)