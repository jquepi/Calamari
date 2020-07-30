﻿using System;
using Octopus.CoreUtilities;
using Octopus.Diagnostics;
using Sashimi.Server.Contracts;
using Sashimi.Server.Contracts.ActionHandlers;
using Sashimi.Server.Contracts.Calamari;
using Sashimi.Server.Contracts.CommandBuilders;
using Sashimi.Server.Contracts.Variables;

namespace Sashimi.Tests.Shared.Server
{
    public class TestActionHandlerContext<TCalamariProgram> : IActionHandlerContext
    {
        readonly ILog log;

        internal TestActionHandlerContext(ILog log)
        {
            this.log = log;
        }

        ILog IActionHandlerContext.Log => log;
        public Maybe<DeploymentTargetType> DeploymentTargetType { get; set; } = Maybe<DeploymentTargetType>.None;
        public Maybe<string> DeploymentTargetName { get; set; } = Maybe<string>.None;
        IActionAndTargetScopedVariables IActionHandlerContext.Variables => Variables;
        public TestVariableDictionary Variables { get; } = new TestVariableDictionary();
        public string EnvironmentId { get; set; } = null!;
        public Maybe<string> TenantId { get; set; } = Maybe<string>.None;

        public IRawShellCommandBuilder RawShellCommand()
            => throw new NotImplementedException();

        public ICalamariCommandBuilder CalamariCommand(CalamariFlavour tool, string toolCommand)
        {
            var builder = new TestCalamariCommandBuilder<TCalamariProgram>(tool, toolCommand);

            builder.SetVariables(Variables);

            return builder;
        }

        public IScriptCommandBuilder ScriptCommand()
            => throw new NotImplementedException();
    }
}