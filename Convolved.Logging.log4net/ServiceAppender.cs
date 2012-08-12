/*
Copyright (C) 2012 Convolved Software

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Transactions;
using Convolved.Logging.Contracts;
using log4net.Appender;
using log4net.Core;

namespace Convolved.Logging.log4net
{
    /// <summary>
    /// Implements a log4net appender that publishes logging events to an
    /// <see cref="ILogService"/>.
    /// </summary>
    public class ServiceAppender : BufferingAppenderSkeleton
    {
        private ChannelFactory<ILogService> serviceChannelFactory;
        private ILogService service;

        /// <inheritdoc />
        public override void ActivateOptions()
        {
            base.ActivateOptions();
            if (string.IsNullOrEmpty(ApplicationName))
            {
                ErrorHandler.Error("Application name is not specified.");
                return;
            }
            try
            {
                serviceChannelFactory = new ChannelFactory<ILogService>(EndpointConfigurationName);
            }
            catch (Exception e)
            {
                ErrorHandler.Error(string.Format("Failed to create a service channel for type " +
                    "{0} using the endpoint configuration name '{1}'.",
                    typeof(ILogService).FullName, EndpointConfigurationName), e);
                return;
            }
        }

        /// <inheritdoc />
        protected override void SendBuffer(LoggingEvent[] events)
        {
            if ((events == null) || (serviceChannelFactory == null) || 
                string.IsNullOrEmpty(ApplicationName))
                return;
            using (var tsc = new TransactionScope(TransactionScopeOption.Suppress))
            {
                var currentService = GetLogService();
                var serviceEvents = events.Select(e => Map(e)).ToArray();
                currentService.Handle(serviceEvents);
            }
        }

        /// <inheritdoc />
        protected override void OnClose()
        {
            base.OnClose();
            SafeClose(service as ICommunicationObject);
            SafeClose(serviceChannelFactory);
        }

        /// <summary>
        /// Creates the log service based on the appender's current configuration.
        /// </summary>
        /// <returns>The <see cref="ILogService"/> instance where log events should be
        /// published.</returns>
        protected virtual ILogService CreateLogService()
        {
            return serviceChannelFactory.CreateChannel();
        }

        /// <summary>
        /// Gets the current log service.
        /// </summary>
        /// <returns>The current log service.</returns>
        protected ILogService GetLogService()
        {
            var co = service as ICommunicationObject;
            if ((co != null) && (co.State == CommunicationState.Faulted))
            {
                co.Abort();
                service = null;
            }
            if (service == null)
                service = CreateLogService();
            return service;
        }

        /// <summary>
        /// Maps a log4net <see cref="LoggingEvent"/> to its <see cref="LogEvent"/> contract
        /// representation.
        /// </summary>
        /// <param name="event">The logging event.</param>
        /// <returns>A new <see cref="LogEvent"/> which can be published to an
        /// <see cref="ILogService"/>.</returns>
        protected virtual LogEvent Map(LoggingEvent @event)
        {
            return new LogEvent
            {
                Server = Environment.MachineName,
                Application = ApplicationName,
                Component = @event.LoggerName,
                WhenOccurred = new DateTimeOffset(@event.TimeStamp),
                ThreadId = @event.ThreadName,
                EventType = @event.Level.ToString(),
                Message = @event.RenderedMessage,
                Details = @event.GetExceptionString()
            };
        }

        /// <summary>
        /// Closes the specified communication object, calling the
        /// <see cref="ICommunicationObject.Abort"/> method if it is in the faulted state.
        /// </summary>
        /// <param name="co">The communication object to close.</param>
        protected void SafeClose(ICommunicationObject co)
        {
            if (co == null)
                return;
            if (co.State == CommunicationState.Faulted)
                co.Abort();
            else
                co.Close();
        }

        /// <summary>
        /// Gets or sets the application name which will appear in log events.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the name of the service endpoint in the application configuration file.
        /// </summary>
        public string EndpointConfigurationName { get; set; }
    }
}