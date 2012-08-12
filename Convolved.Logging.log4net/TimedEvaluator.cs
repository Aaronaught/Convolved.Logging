#region Copyright & License
//
// Copyright 2001-2005 The Apache Software Foundation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;
using log4net;

namespace log4net.Core
{
    /// <summary>
    /// An evaluator that triggers after specified time
    /// </summary>
    /// <remarks>
    /// <para>
    /// This evaluator will trigger if the specified time period 
    /// <see cref="Threshold"/> has passed since last check.
    /// </para>
    /// </remarks>
    /// <author>Robert Sevcik</author>
    public class TimeEvaluator : ITriggeringEventEvaluator
    {
        /// <summary>
        /// The time threshold for triggering in seconds. Zero means it won't trigger at all.
        /// </summary>
        private int m_threshold;

        /// <summary>
        /// The time of last check. This gets updated when the object is created and when the evaluator triggers.
        /// </summary>
        private DateTime m_lasttime;

        /// <summary>
        /// The default time threshold for triggering in seconds. Zero means it won't trigger at all.
        /// </summary>
        const int DEFAULT_THRESHOLD = 0;

        /// <summary>
        /// Create a new evaluator using the <see cref="DEFAULT_THRESHOLD"/> time threshold in seconds.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Create a new evaluator using the <see cref="DEFAULT_THRESHOLD"/> time threshold in seconds.
        /// </para>
        /// <para>
        /// This evaluator will trigger if the specified time period 
        /// <see cref="Threshold"/> has passed since last check.
        /// </para>
        /// </remarks>
        public TimeEvaluator()
            : this(DEFAULT_THRESHOLD)
        {
        }

        /// <summary>
        /// Create a new evaluator using the specified time threshold in seconds.
        /// </summary>
        /// <param name="threshold_seconds">
        /// The time threshold in seconds to trigger after.
        /// Zero means it won't trigger at all.
        /// </param>
        /// <remarks>
        /// <para>
        /// Create a new evaluator using the specified time threshold in seconds.
        /// </para>
        /// <para>
        /// This evaluator will trigger if the specified time period 
        /// <see cref="Threshold"/> has passed since last check.
        /// </para>
        /// </remarks>
        public TimeEvaluator(int threshold_seconds)
        {
            m_threshold = threshold_seconds;
            m_lasttime = DateTime.Now;
        }

        /// <summary>
        /// the time threshold in seconds to trigger after
        /// </summary>
        /// <value>
        /// The time threshold in seconds to trigger after.
        /// Zero means it won't trigger at all.
        /// </value>
        /// <remarks>
        /// <para>
        /// This evaluator will trigger if the specified time period 
        /// <see cref="Threshold"/> has passed since last check.
        /// </para>
        /// </remarks>
        public int Threshold
        {
            get { return m_threshold; }
            set { m_threshold = value; }
        }

        /// <summary>
        /// Is this <paramref name="loggingEvent"/> the triggering event?
        /// </summary>
        /// <param name="loggingEvent">The event to check</param>
        /// <returns>This method returns <c>true</c>, if the specified time period 
        /// <see cref="Threshold"/> has passed since last check.. 
        /// Otherwise it returns <c>false</c></returns>
        /// <remarks>
        /// <para>
        /// This evaluator will trigger if the specified time period 
        /// <see cref="Threshold"/> has passed since last check.
        /// </para>
        /// </remarks>
        public bool IsTriggeringEvent(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
            {
                throw new ArgumentNullException("loggingEvent");
            }

            // disable the evaluator if threshold is zero
            if (m_threshold == 0) return false;

            lock (this) // avoid triggering multiple times
            {
                TimeSpan passed = DateTime.Now.Subtract(m_lasttime);

                if (passed.TotalSeconds > m_threshold)
                {
                    m_lasttime = DateTime.Now;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}