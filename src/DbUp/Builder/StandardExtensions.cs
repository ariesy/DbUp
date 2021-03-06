﻿using System;
using System.Collections.Generic;
using System.Reflection;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.ScriptProviders;

/// <summary>
/// Configuration extensions for the standard stuff.
/// </summary>
// NOTE: DO NOT MOVE THIS TO A NAMESPACE
// Since the class just contains extension methods, we leave it in the root so that it is always discovered
// and people don't have to manually add using statements.
// ReSharper disable CheckNamespace
public static class StandardExtensions
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// Logs to a custom logger.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="log">The logger.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder LogTo(this UpgradeEngineBuilder builder, IUpgradeLog log)
    {
        builder.Configure(c => c.Log = log);
        return builder;
    }

    /// <summary>
    /// Logs to the console using pretty colours.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder LogToConsole(this UpgradeEngineBuilder builder)
    {
        return LogTo(builder, new ConsoleUpgradeLog());
    }

    /// <summary>
    /// Logs to System.Diagnostics.Trace.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder LogToTrace(this UpgradeEngineBuilder builder)
    {
        return LogTo(builder, new TraceUpgradeLog());
    }

    /// <summary>
    /// Uses a custom journal for recording which scripts were executed.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="journal">The custom journal.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder JournalTo(this UpgradeEngineBuilder builder, IJournal journal)
    {
        builder.Configure(c => c.Journal = journal);
        return builder;
    }

    /// <summary>
    /// Adds a custom script provider to the upgrader.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="scriptProvider">The script provider.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScripts(this UpgradeEngineBuilder builder, IScriptProvider scriptProvider)
    {
        builder.Configure(c => c.ScriptProviders.Add(scriptProvider));
        return builder;
    }

    /// <summary>
    /// Adds a static set of scripts to the upgrader.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="scripts">The scripts.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScripts(this UpgradeEngineBuilder builder, IEnumerable<SqlScript> scripts)
    {
        return WithScripts(builder, new StaticScriptProvider(scripts));
    }

    /// <summary>
    /// Adds a static set of scripts to the upgrader.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="scripts">The scripts.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScripts(this UpgradeEngineBuilder builder, params SqlScript[] scripts)
    {
        return WithScripts(builder, (IEnumerable<SqlScript>)scripts);
    }

    /// <summary>
    /// Adds a single static script to the upgrader.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="script">The script.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScript(this UpgradeEngineBuilder builder, SqlScript script)
    {
        return WithScripts(builder, script);
    }

    /// <summary>
    /// Adds a single static script to the upgrader.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="name">The name of the script. This should never change once executed.</param>
    /// <param name="contents">The script body.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScript(this UpgradeEngineBuilder builder, string name, string contents)
    {
        var script = new SqlScript(name, contents);
        return WithScripts(builder, script);
    }

    /// <summary>
    /// Adds all scripts from a folder on the file system.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="path">The directory path.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsFromFileSystem(this UpgradeEngineBuilder builder, string path)
    {
        return WithScripts(builder, new FileSystemScriptProvider(path));
    }

    /// <summary>
    /// Adds all scripts found as embedded resources in the given assembly.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assembly">The assembly.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this UpgradeEngineBuilder builder, Assembly assembly)
    {
        return WithScripts(builder, new EmbeddedScriptProvider(assembly, s => s.EndsWith(".sql", StringComparison.InvariantCultureIgnoreCase)));
    }

    /// <summary>
    /// Adds all scripts found as embedded resources in the given assembly, with a custom filter (you'll need to exclude non- .SQL files yourself).
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assembly">The assembly.</param>
    /// <param name="filter">The filter. Don't forget to ignore any non- .SQL files.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this UpgradeEngineBuilder builder, Assembly assembly, Func<string, bool> filter)
    {
        return WithScripts(builder, new EmbeddedScriptProvider(assembly, filter));
    }

    /// <summary>
    /// Adds all scripts found as embedded resources in the given assembly, or classes which inherit from IScript, with a custom filter (you'll need to exclude non- .SQL files yourself).
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assembly">The assembly.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsAndCodeEmbeddedInAssembly(this UpgradeEngineBuilder builder, Assembly assembly)
    {
        return WithScripts(builder, new EmbeddedScriptAndCodeProvider(assembly, s => s.EndsWith(".sql", StringComparison.InvariantCultureIgnoreCase)));
    }

    /// <summary>
    /// Adds all scripts found as embedded resources in the given assembly, or classes which inherit from IScript, with a custom filter (you'll need to exclude non- .SQL files yourself).
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assembly">The assembly.</param>
    /// <param name="filter">The Sql Script filter (only affects embdeeded scripts, does not filter IScript files). Don't forget to ignore any non- .SQL files.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsAndCodeEmbeddedInAssembly(this UpgradeEngineBuilder builder, Assembly assembly, Func<string, bool> filter)
    {
        return WithScripts(builder, new EmbeddedScriptAndCodeProvider(assembly, filter));
    }

    /// <summary>
    /// Adds a preprocessor that can replace portions of a script.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="preprocessor">The preprocessor.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithPreprocessor(this UpgradeEngineBuilder builder, IScriptPreprocessor preprocessor)
    {
        builder.Configure(c => c.ScriptPreprocessors.Add(preprocessor));
        return builder;
    }

    /// <summary>
    /// Adds a set of variables that will be replaced before scripts are executed.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="variables">The variables.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithVariables(this UpgradeEngineBuilder builder, IDictionary<string, string> variables)
    {
        builder.Configure(c => c.AddVariables(variables));
        return builder;
    }

    /// <summary>
    /// Adds a single variable that will be replaced before scripts are executed.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="variableName">The name of the variable.</param>
    /// <param name="value">The value to be substituted.</param>
    /// <returns></returns>
    public static UpgradeEngineBuilder WithVariable(this UpgradeEngineBuilder builder, string variableName, string value)
    {
        return WithVariables(builder, new Dictionary<string, string> { { variableName, value } });
    }
}

