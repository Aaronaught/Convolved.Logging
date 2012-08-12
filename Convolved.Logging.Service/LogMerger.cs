/*
Copyright (C) 2012 Convolved Software

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Convolved.Threading.Timeouts;
using log4net;
using NHibernate;

namespace Convolved.Logging.Service
{
    /// <summary>
    /// Merges new (unprocessed) log entries into permanent log and statistics stores.
    /// </summary>
    public class LogMerger : IDisposable
    {
        private readonly ILog log = LogManager.GetLogger(typeof(LogMerger));
        private readonly ISessionFactory sessionFactory;
        private readonly ITimeout timeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMerger"/> class using the specified
        /// NHibernate session factory.
        /// </summary>
        /// <param name="sessionFactory">A session factory configured to open sessions to the log
        /// database.</param>
        /// <param name="timeout">A timeout instance which will be used to trigger merges after new
        /// events arrive.</param>
        public LogMerger(ISessionFactory sessionFactory, ITimeout timeout)
        {
            if (sessionFactory == null)
                throw new ArgumentNullException("sessionFactory");
            if (timeout == null)
                throw new ArgumentNullException("timeout");
            this.sessionFactory = sessionFactory;
            this.timeout = timeout;
            this.timeout.Elapsed += TimedMerge;
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            timeout.Elapsed -= TimedMerge;
        }

        /// <summary>
        /// Schedules a merge to run after the timeout elapses.
        /// </summary>
        public void Wake()
        {
            log.DebugFormat("Restarting merge check timeout");
            timeout.Start();
        }

        /// <summary>
        /// Merges new, unprocessed log events into the primary log and statistics tables.
        /// </summary>
        protected virtual void Merge()
        {
            const string procedureName = "ProcessNewLogEntries";
            log.Debug("Merge requested - creating session");
            int rowCount = 0;
            using (var session = sessionFactory.OpenStatelessSession())
            using (var transaction = session.BeginTransaction())
            {
                log.DebugFormat("Executing {0} procedure", procedureName);
                rowCount = session.CreateSQLQuery("EXEC " + procedureName).UniqueResult<int>();
                log.Debug("Commit transaction");
                transaction.Commit();
            }
            if (rowCount > 0)
            {
                log.InfoFormat("Merged {0} new log events", rowCount);
                Wake();
            }
        }

        private void TimedMerge(object sender, ElapsedEventArgs e)
        {
            try
            {
                Merge();
            }
            catch (Exception ex)
            {
                log.Error("Failed to merge new events", ex);
            }
        }
    }
}