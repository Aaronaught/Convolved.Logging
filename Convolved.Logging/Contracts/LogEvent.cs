/*
Copyright (C) 2012 Convolved Software

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using System;
using System.Runtime.Serialization;

namespace Convolved.Logging.Contracts
{
    /// <summary>
    /// Encapsulates a single event logged by an application.
    /// </summary>
    [DataContract(Namespace = Namespace.Default)]
    public class LogEvent
    {
        /// <summary>
        /// Gets or sets the name of the server on which the event occurred.
        /// </summary>
        [DataMember]
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets the name of the active application.
        /// </summary>
        [DataMember]
        public string Application { get; set; }

        /// <summary>
        /// Gets or sets the source of the event (e.g. class or component name).
        /// </summary>
        [DataMember]
        public string Component { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the event was logged.
        /// </summary>
        [DataMember]
        public DateTimeOffset WhenOccurred { get; set; }

        /// <summary>
        /// Gets or sets the unique (per-process) identifier of the thread on which the event
        /// occurred.
        /// </summary>
        [DataMember]
        public string ThreadId { get; set; }

        /// <summary>
        /// Gets or sets the type of event (e.g. error, warning, info).
        /// </summary>
        [DataMember]
        public string EventType { get; set; }

        /// <summary>
        /// Gets or sets the event message.
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets additional details associated with the message, e.g. an exception message.
        /// </summary>
        [DataMember]
        public string Details { get; set; }
    }
}