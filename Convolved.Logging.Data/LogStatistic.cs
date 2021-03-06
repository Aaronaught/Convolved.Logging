﻿/*
Copyright (C) 2012 Convolved Software

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using System;

namespace Convolved.Logging.Data
{
    public class LogStatistic
    {
        public virtual int Id { get; set; }
        public virtual string Server { get; set; }
        public virtual string Application { get; set; }
        public virtual string Component { get; set; }
        public virtual string EventType { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual int Year { get; set; }
        public virtual int Month { get; set; }
        public virtual int Day { get; set; }
        public virtual int Hour { get; set; }
        public virtual int Count { get; set; }
    }
}