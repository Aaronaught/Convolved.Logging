/*
Copyright (C) 2012 Convolved Software

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using System;
using System.ServiceModel;

namespace Convolved.Logging.Contracts
{
    /// <summary>
    /// Defines a log service which handles application log events.
    /// </summary>
    [ServiceContract(Namespace = Namespace.Default, Name = "Logger", 
        SessionMode = SessionMode.NotAllowed)]
    public interface ILogService
    {
        /// <summary>
        /// Handles a log event.
        /// </summary>
        /// <param name="event">The log event.</param>
        [OperationContract(Name = "HandleOne", IsOneWay = true)]
        void Handle(LogEvent @event);

        /// <summary>
        /// Handles a series of log events.
        /// </summary>
        /// <param name="events">The log events.</param>
        [OperationContract(Name = "HandleMultiple", IsOneWay = true)]
        void Handle(LogEvent[] events);
    }
}