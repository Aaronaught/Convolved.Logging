/*
Copyright (C) 2012 Convolved Software

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using System;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.Wcf;
using Ninject.Modules;
using Ninject.Web.Common;
using Convolved.Logging.Persistence.Maps;

namespace Convolved.Logging.Service.Modules
{
    class NHibernateModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ISessionFactory>()
                .ToMethod(ctx => CreateSessionFactory())
                .InSingletonScope();
            Bind<ISession>()
                .ToMethod(ctx => CreateSession(ctx))
                .InRequestScope();
            Bind<IStatelessSession>()
                .ToMethod(ctx => CreateStatelessSession(ctx))
                .InRequestScope();
        }

        private static ISession CreateSession(IContext context)
        {
            var sessionFactory = context.Kernel.Get<ISessionFactory>();
            return sessionFactory.OpenSession();
        }

        private static IStatelessSession CreateStatelessSession(IContext context)
        {
            var sessionFactory = context.Kernel.Get<ISessionFactory>();
            return sessionFactory.OpenStatelessSession();
        }

        private static ISessionFactory CreateSessionFactory()
        {
            var settings = Properties.Settings.Default;
            var configuration = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008
                    .ConnectionString(settings.LogDbConnectionString)
                    .AdoNetBatchSize(1000)
                )
                .Mappings(m => m.FluentMappings
                    .AddFromAssemblyOf<LogStagingEventMap>())
                .BuildConfiguration();
            return configuration.BuildSessionFactory();
        }
    }
}