using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Calamari.Commands;
using Calamari.Commands.Support;
using Calamari.Common;
using Calamari.Common.Extensions;
using Calamari.Common.Variables;
using Calamari.Deployment;
using Calamari.Deployment.Conventions;
using Calamari.Deployment.Journal;
using Calamari.HealthChecks;
using Calamari.Hooks;
using Calamari.Integration.Certificates;
using Calamari.Integration.FileSystem;
using Calamari.Integration.Packages;
using Calamari.Integration.Processes;
using Calamari.Integration.Proxies;
using Calamari.Integration.Scripting;
using Calamari.Integration.Substitutions;
using Calamari.Util.Environments;
using Calamari.Variables;

namespace Calamari
{
    public abstract class CalamariFlavourProgram
    {
        readonly ILog log;

        protected CalamariFlavourProgram(ILog log)
        {
            this.log = log;
        }
        
        protected int Run(CommonOptions options)
        {
            log.Verbose($"Calamari Version: {typeof(CalamariFlavourProgram).Assembly.GetInformationalVersion()}");

            if (options.Command.Equals("version", StringComparison.OrdinalIgnoreCase))
                return 0;

            var envInfo = string.Join($"{Environment.NewLine}  ", EnvironmentHelper.SafelyGetEnvironmentInformation());
            log.Verbose($"Environment Information: {Environment.NewLine}  {envInfo}");

            EnvironmentHelper.SetEnvironmentVariable(SpecialVariables.CalamariWorkingDirectory, Environment.CurrentDirectory);
            ProxyInitializer.InitializeDefaultProxy();

            using (var container = BuildContainer(options).Build())
            {
                container.Resolve<VariableLogger>().LogVariables();

                try
                {
                    var command = container.ResolveNamed<ICommand>(options.Command);
                    return command.Execute();
                }
                catch (Exception e) when (e is ComponentNotRegisteredException || e is DependencyResolutionException) 
                {
                    throw new CommandException($"Could not find the command {options.Command}");
                }
            }
        }

        protected virtual ContainerBuilder BuildContainer(CommonOptions options)
        {
            var fileSystem = CalamariPhysicalFileSystem.GetPhysicalFileSystem();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(fileSystem).As<ICalamariFileSystem>();
            builder.RegisterType<VariablesFactory>().AsSelf();
            builder.Register(c => c.Resolve<VariablesFactory>().Create(options)).As<IVariables>().SingleInstance();
            builder.RegisterType<ScriptEngine>().As<IScriptEngine>();
            builder.RegisterType<VariableLogger>().AsSelf();
            builder.RegisterInstance(log).As<ILog>().SingleInstance();
            builder.RegisterType<CalamariCertificateStore>().As<ICertificateStore>().SingleInstance();
            builder.RegisterType<FreeSpaceChecker>().As<IFreeSpaceChecker>().SingleInstance();
            builder.RegisterType<DeploymentJournalWriter>().As<IDeploymentJournalWriter>().SingleInstance();
            builder.RegisterType<CommandLineRunner>().As<ICommandLineRunner>().SingleInstance();
            builder.RegisterType<PackageStore>().As<IPackageStore>().SingleInstance();
            builder.RegisterType<CombinedPackageExtractor>().As<ICombinedPackageExtractor>();
            builder.RegisterType<FileSubstituter>().As<IFileSubstituter>();
            builder.RegisterType<SubstituteInFiles>().As<ISubstituteInFiles>();
            builder.RegisterType<ExtractPackage>().As<IExtractPackage>();


            var assemblies = GetAllAssembliesToRegister(options).ToArray();

            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<IScriptWrapper>()
                .Except<TerminalScriptWrapper>()
                .As<IScriptWrapper>()
                .SingleInstance();

            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<IDoesDeploymentTargetTypeHealthChecks>()
                .As<IDoesDeploymentTargetTypeHealthChecks>()
                .SingleInstance();

            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<ICommand>()
                .Where(t => t.GetCustomAttribute<CommandAttribute>().Name.Equals(options.Command, StringComparison.OrdinalIgnoreCase))
                .As<ICommand>();

            return builder;
        }

        static IEnumerable<Assembly> GetAllAssembliesToRegister(CommonOptions options)
        {
            yield return typeof(CalamariFlavourProgram).Assembly; // Calamari
            yield return typeof(ApplyDeltaCommand).Assembly; // Calamari.Shared
        }
    }
}