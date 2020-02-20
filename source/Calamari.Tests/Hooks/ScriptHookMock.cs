﻿using System.Collections.Generic;
using Calamari.Hooks;
using Calamari.Integration.Processes;
using Calamari.Integration.Scripting;

namespace Calamari.Tests.Hooks
{
    /// <summary>
    /// A mock script wrapper that we can use to track if it has been executed or not
    /// </summary>
    public class ScriptHookMock : IScriptWrapper
    {
        /// <summary>
        /// This is how we know if this wrapper was called or not
        /// </summary>
        public bool WasCalled { get; private set; } = false;
        public int Priority => 1;
        public bool IsEnabled(ScriptSyntax scriptSyntax) => true;
        public IScriptWrapper NextWrapper { get; set; }

        public CommandResult ExecuteScript(Script script,
            ScriptSyntax scriptSyntax,
            IVariables variables,
            ICommandLineRunner commandLineRunner,
            Dictionary<string, string> environmentVars)
        {
            WasCalled = true;
            return NextWrapper.ExecuteScript(script, scriptSyntax, variables, commandLineRunner, environmentVars);
        }
    }
}
