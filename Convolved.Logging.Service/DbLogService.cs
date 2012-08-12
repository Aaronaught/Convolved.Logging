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
using Convolved.Logging.Contracts;
using log4net;
using NHibernate;
using Data = Convolved.Logging.Data;

namespace Convolved.Logging.Service
{
    /// <summary>
    /// Implements a logging service which persists events to a database.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    public class DbLogService : ILogService
    {
        private readonly ILog log = LogManager.GetLogger(typeof(DbLogService));
        private readonly IStatelessSession session;
        private readonly LogMerger merger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbLogService"/> class using the specified
        /// NHibernate stateless session instance.
        /// </summary>
        /// <param name="session">An NHibernate stateless session which points to the database
        /// where log events will be written.</param>
        /// <param name="merger">The instance responsible for merging new events.</param>
        public DbLogService(IStatelessSession session, LogMerger merger)
        {
            if (session == null)
                throw new ArgumentNullException("session");
            if (merger == null)
                throw new ArgumentNullException("merger");
            this.session = session;
            this.merger = merger;
            log.Debug("DbLogService created");
        }

        /// <inheritdoc />
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Handle(LogEvent @event)
        {
            log.Debug("Handling single event");
            if (@event == null)
                return;
            using (var transaction = session.BeginTransaction())
            {
                Write(@event);
                log.Debug("Commit transaction");
                transaction.Commit();
            }
            merger.Wake();
        }

        /// <inheritdoc />
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Handle(LogEvent[] events)
        {
            log.Debug("Handling multiple events");
            if (events == null)
                return;
            using (var transaction = session.BeginTransaction())
            {
                foreach (var @event in events)
                    Write(@event);
                log.Debug("Commit transaction");
                transaction.Commit();
            }
            merger.Wake();
        }

        private void Write(LogEvent @event)
        {
            log.DebugFormat("Writing event, server = {0}, application = {1}, component = {2}, " +
                "date = {3}, thread = {4}, type = {5}, message = {6}",
                @event.Server, @event.Application, @event.Component, @event.WhenOccurred,
                @event.ThreadId, @event.EventType, @event.Message);
            session.Insert(new Data.LogStagingEvent
            {
                Server = @event.Server,
                Application = @event.Application,
                Component = @event.Component,
                WhenOccurred = @event.WhenOccurred,
                ThreadId = @event.ThreadId,
                EventType = @event.EventType,
                Message = @event.Message,
                Details = @event.Details
            });
        }
    }
}