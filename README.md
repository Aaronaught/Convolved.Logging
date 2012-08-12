Convolved.Logging
===========================

Overview
--------
Convolved Logging is an evolving set of tools for capturing and centralizing the storage and analysis of log events across all components of a distributed system or application.

What's Included
---------------
The current version contains the very basics of what you need to start capturing and querying log events:

* Database creation scripts (SQL Server)
* Data Contracts for WCF Integration
* Entity Model and Fluent NHibernate maps for querying data
* Service endpoint (WCF self-host) for log writing and statistics calculation
* A log4net appender to forward messages to the service endpoint.

This tool set works very well as a drop-in enhancement for systems that already emit logs but do not yet have mature aggregation or monitoring.

What's *Not* Included
-------------------
The aim of this project is to eventually include:

* ASP.NET MVC integration, including a drop-in UI library with embedded controllers/views/routes, and a maintenance page to monitor and trim log sizes
* An installer for the service endpoint
* Event sinks, e.g. alerts sent out for critical failures
* A logging interceptor for automatic logging of method entry/exit/errors
* A more comprehensive set of instrumentation (performance counters, etc.)
* NLog integration

In other words, it's a manual deployment right now, it *just* handles logging, and when it comes to *what to do* with the log data, you are currently on your own.

Usage
-----
You'll need the following in order to run the full solution:

* SQL Server 2008 (or Express)
* An application server to run the service endpoint
* MSMQ installed on all participating servers
* Existing logging code using log4net

**Installing the Database**

1. Choose or create a SQL Server database to hold the data.
2. Run each of the scripts in the `Database` folder in sequence.
3. If necessary, create or assign a user for the service endpoint (should have the `db_datawriter` role).

**Installing/Configuring the Service Endpoint** (Convolved.Logging.Service)

1. Create a message queue called `convolved.log.events` on the application server. You can change the queue name in the `app.config` if you want.
2. Add an HTTP reservation for the log service on port 8155 using the [netsh command](http://msdn.microsoft.com/en-us/library/ms733768.aspx). The port can be changed in `app.config` if necessary.
3. Edit the `LogDbConnectionString` to point to the database instance created above.
4. Install the service using the .NET `installutil` and run it.

**Sending Log Events to the Service Endpoint**

1. Import all of the .dll files in the build output of `Convolved.Logging.log4net` into your project.
2. Configure the log endpoint in the `<system.serviceModel>` section of your project's `app.config` or `web.config`:

    ```xml
	<system.serviceModel>
	  <client>
        <endpoint name="logEndpoint"
		  address="net.msmq://localhost/private/convolved.log.events"
		  binding="netMsmqBinding" bindingConfiguration="logBinding"
		  contract="Convolved.Logging.Contracts.ILogService">
		</endpoint>
	  </client>
	</system.serviceModel>
    ```
	
3. Configure the log appender in the `<log4net>` section of your project's `app.config` or `web.config`:

    ```xml
	<appender name="LogServiceAppender" type="Convolved.Logging.log4net.ServiceAppender,Convolved.Logging.log4net">
      <bufferSize value="0" />
      <lossy value="false" />
      <ApplicationName value="My Application"/>
      <EndpointConfigurationName value="logEndpoint" />
      <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%date [%thread] %-5level %logger [%property{NDC}] - %message" />
      </layout>
      <filter type="log4net.Filter.LevelRangeFilter">
        <levelMin value="INFO" />
        <levelMax value="FATAL" />
      </filter>
    </appender>
	```
	
After you have completed all of the above steps, your applications will start sending log messages to the service via MSMQ; the service will continuously merge these events into the database for reporting and analysis.

Why Bother?
-----------
Some of you might be asking: *why should I add all of this overhead instead of just using the `AdoNetAppender` already in log4net?*

Convolved Logging uses a number of tricks to improve throughput as well as fault-tolerance and potentially handle extreme numbers of log messages:

* Sending a message over MSMQ is a local operation, with near-zero latency. A SQL `INSERT` is remote, unless your entire infrastructure runs on one server (in which case, you should have stopped reading at the word *distributed*).
* The `AdoNetAppender` uses synchronous blocking calls. If your database is down or unreachable, this will cause potentially important log events to be lost and, more importantly, cripple your application's performance, even when using timeouts or lossy mode. MSMQ sends are, by definition, fire-and-forget; using a transactional queue prevents any events from being lost due to server or network interruptions.
* The log service endpoint itself writes events to a staging table, without any indexes or constraints. These events are subsequently merged in bulk by a background process which waits for an idle period (up to a configurable maximum); this allows you to minimize read/write contention on the queryable tables while remaining within the boundaries of a Service Level Agreement.
* Coarser log statistics (totals per hour, event type, etc.) are kept, allowing many dashboard-style interfaces to be built without hitting the main log entry table.
* Collected log events will contain references to the Server and Application that generated them, which is important for analysis when components (assemblies/types) are shared across multiple projects and services are clustered or load-balanced.

Performance Considerations
--------------------------
Although this library does everything it can to maximize throughput and minimize latency, it is not necessarily a free pass to log *everything*. In any distributed system, latency is not zero and bandwidth is not infinite. Forcing too much information through the logs may hinder performance of either the service endpoint or your own application (if using the appender).

As with most systems, you should only log what you can reasonably expect to be useful; typically you will only want to log at the "info" level rather than "debug". If you do choose to log debug information, it should normally be a setting that is explicitly turned on at runtime and is automatically turned off after a certain time (depending on your system's typical load).

Be careful, and be smart. Too little logging will leave you unable to diagnose production failures, but too much logging may create failures of its own. You will need to assess for yourself which applications and log levels are appropriate to pass through the service appender/endpoint.

Copyright and License
---------------------
Copyright (C) 2012 Convolved Software

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.