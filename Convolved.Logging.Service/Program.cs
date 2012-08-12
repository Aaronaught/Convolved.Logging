/*
Copyright (C) 2012 Convolved Software

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using System;
using System.ServiceModel;
using Convolved.Hosting.Ninject;
using Convolved.Logging.Service.Modules;
using log4net.Config;
using Ninject;

namespace Convolved.Logging.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            new ServiceEnvironmentHost()
                .Name("LogService")
                .Modules(new NHibernateModule(), new MergeModule())
                .Service<DbLogService>()
                .InitializeWith(Merge)
                .RunWithCommandLineArgs(args);
        }

        private static void Merge(IKernel kernel)
        {
            var merger = kernel.Get<LogMerger>();
            merger.Wake();
        }
    }
}