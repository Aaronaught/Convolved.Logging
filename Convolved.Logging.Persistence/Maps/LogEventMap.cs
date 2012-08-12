/*
Copyright (C) 2012 Convolved Software

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using System;
using FluentNHibernate.Mapping;
using Convolved.Logging.Data;

namespace Convolved.Logging.Persistence.Maps
{
    public class LogEventMap : ClassMap<LogEvent>
    {
        public LogEventMap()
        {
            Table("Log");
            ReadOnly();
            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.WhenOccurred, "Date");
            Map(x => x.Server, "ServerName").Length(50).Not.Nullable();
            Map(x => x.Application, "ApplicationName").Length(50).Not.Nullable();
            Map(x => x.Component, "ComponentName").Length(255).Not.Nullable();
            Map(x => x.ThreadId, "Thread").Length(255).Not.Nullable();
            Map(x => x.EventType, "EventType").Length(50).Not.Nullable();
            Map(x => x.Message, "Message").Length(int.MaxValue).Not.Nullable();
            Map(x => x.Details, "Details").Length(int.MaxValue).Nullable();
        }
    }
}