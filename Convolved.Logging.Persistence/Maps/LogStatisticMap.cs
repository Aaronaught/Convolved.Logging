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
    public class LogStatisticMap : ClassMap<LogStatistic>
    {
        public LogStatisticMap()
        {
            Table("LogStatistics");
            ReadOnly();
            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.Server, "ServerName").Length(50).Not.Nullable();
            Map(x => x.Application, "ApplicationName").Length(50).Not.Nullable();
            Map(x => x.Component, "ComponentName").Length(255).Not.Nullable();
            Map(x => x.Date);
            Map(x => x.Year);
            Map(x => x.Month);
            Map(x => x.Day);
            Map(x => x.Hour);
            Map(x => x.EventType, "Level").Length(50).Not.Nullable(); ;
            Map(x => x.Count);
        }
    }
}