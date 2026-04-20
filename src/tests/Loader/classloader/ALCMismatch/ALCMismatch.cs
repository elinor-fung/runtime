// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Tests for the ALC-mismatch diagnostic in MissingMethodException.
//
// Scenario:
//   PluginWorker.dll  defines  Workers.Worker.Process(Plugins.MyType)
//   PluginCaller.dll  defines  Callers.Caller.Call(Worker w, MyType t) { w.Process(t); }
//   PluginType.dll    defines  Plugins.MyType
//
// To trigger the mismatch:
//   - Load PluginWorker into ALC1 (which also loads PluginType into ALC1).
//   - Load PluginCaller into ALC2, but make ALC2 delegate "PluginWorker" to ALC1
//     while loading its own independent copy of "PluginType".
//
// When PluginCaller.Caller.Call is JIT-compiled in ALC2's context:
//   - The MemberRef for w.Process(t) has MyType resolved from ALC2 (PluginType in ALC2).
//   - But Worker.Process on ALC1's Worker expects ALC1's MyType.
//   - FindMethod returns null  ->  ThrowMissingMethodException
//   -> FindSignatureTypeMismatch detects the ALC mismatch and includes diagnostic info.

using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

class ALCMismatchTest
{
    // ---------------------------------------------------------------------------
    // Custom ALC: loads assemblies from a directory; for specific names an
    // optional delegate can return a pre-loaded Assembly instead.
    // ---------------------------------------------------------------------------
    class TestALC : AssemblyLoadContext
    {
        private readonly string _dir;
        private readonly Func<AssemblyName, Assembly?>? _overrideLoad;

        public TestALC(string name, string dir,
                       Func<AssemblyName, Assembly?>? overrideLoad = null)
            : base(name, isCollectible: false)
        {
            _dir = dir;
            _overrideLoad = overrideLoad;
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            Assembly? overridden = _overrideLoad?.Invoke(assemblyName);
            if (overridden != null)
                return overridden;

            string path = Path.Combine(_dir, assemblyName.Name + ".dll");
            if (File.Exists(path))
                return LoadFromAssemblyPath(path);

            return null;
        }
    }

    // ---------------------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------------------

    // Invoke callMethod and expect a MissingMethodException (either thrown directly
    // by the JIT-compile path or wrapped in TargetInvocationException by Invoke).
    static MissingMethodException InvokeExpectingMissingMethod(
        MethodInfo callMethod, object? target, object?[] args)
    {
        try
        {
            callMethod.Invoke(target, args);
        }
        catch (TargetInvocationException tie) when (tie.InnerException is MissingMethodException mme)
        {
            return mme;
        }
        catch (MissingMethodException mme)
        {
            return mme;
        }

        throw new Exception(
            $"Expected MissingMethodException from {callMethod.Name} but no exception was thrown.");
    }

    static void AssertContains(string message, string expected, string field)
    {
        if (!message.Contains(expected))
            throw new Exception($"Expected '{expected}' ({field}) in message:\n  {message}");
    }

    // ---------------------------------------------------------------------------
    // Test 1: both ALCs load PluginType from a file path.
    //   - ALC1 loads PluginWorker (+ its dependency PluginType) from path.
    //   - ALC2 delegates PluginWorker -> ALC1 but loads its own PluginType from path.
    //   - MissingMethodException message must contain the type name, assembly name,
    //     "loaded at" path, and "different contexts".
    // ---------------------------------------------------------------------------
    static void TestALCMismatch_PathLoad()
    {
        string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        // ALC1: loads everything from the test directory.
        var alc1 = new TestALC("ALC1-PathLoad", testDir);
        Assembly workerAsm = alc1.LoadFromAssemblyPath(Path.Combine(testDir, "PluginWorker.dll"));

        // ALC2: for "PluginWorker" return ALC1's copy; everything else (including
        //       PluginType) is loaded fresh from the test directory into ALC2.
        var alc2 = new TestALC("ALC2-PathLoad", testDir,
            asmName => asmName.Name == "PluginWorker" ? workerAsm : null);
        Assembly callerAsm = alc2.LoadFromAssemblyPath(Path.Combine(testDir, "PluginCaller.dll"));

        // Worker instance from ALC1.
        Type workerType = workerAsm.GetType("Workers.Worker")!;
        object workerInstance = Activator.CreateInstance(workerType)!;

        // MyType instance from ALC2's PluginType (the 2nd parameter type of Caller.Call).
        Type callerType = callerAsm.GetType("Callers.Caller")!;
        MethodInfo callMethod = callerType.GetMethod("Call",
            BindingFlags.Public | BindingFlags.Static)!;
        Type myTypeALC2 = callMethod.GetParameters()[1].ParameterType;
        object myTypeInstance = Activator.CreateInstance(myTypeALC2)!;

        // Invoke triggers JIT-compile of Caller.Call in ALC2's context.
        // The MemberRef for w.Process(t) uses ALC2's MyType; Worker.Process needs ALC1's MyType.
        MissingMethodException mme = InvokeExpectingMissingMethod(callMethod, null,
            new object?[] { workerInstance, myTypeInstance });

        string msg = mme.Message;
        Console.WriteLine($"  Message: {msg}");

        AssertContains(msg, "Plugins.MyType",   "type name");
        AssertContains(msg, "PluginType",        "assembly name");
        AssertContains(msg, "loaded at",         "path variant indicator");
        AssertContains(msg, "different contexts","ALC mismatch text");
    }

    // ---------------------------------------------------------------------------
    // Test 2: ALC1 loads PluginType from a byte array (no path).
    //   - The assembly-info for ALC1's PluginType should say "(loaded from byte array)".
    //   - The assembly-info for ALC2's PluginType should say "(loaded at '...')".
    // ---------------------------------------------------------------------------
    static void TestALCMismatch_ByteLoad()
    {
        string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        string pluginTypePath = Path.Combine(testDir, "PluginType.dll");

        // ALC1: load PluginType from bytes before loading PluginWorker so that the
        //       Load override can return the already-loaded byte assembly.
        Assembly? pluginTypeByteAsm = null;

        var alc1 = new TestALC("ALC1-ByteLoad", testDir, asmName =>
        {
            if (asmName.Name == "PluginType" && pluginTypeByteAsm != null)
                return pluginTypeByteAsm;
            return null;
        });

        // Pre-load PluginType from bytes into ALC1 so the Load handler above can
        // return it when PluginWorker.dll's dependency is resolved.
        byte[] pluginTypeBytes = File.ReadAllBytes(pluginTypePath);
        pluginTypeByteAsm = alc1.LoadFromStream(new MemoryStream(pluginTypeBytes));

        Assembly workerAsm = alc1.LoadFromAssemblyPath(Path.Combine(testDir, "PluginWorker.dll"));

        // ALC2: delegate PluginWorker to ALC1; load its own PluginType from path.
        var alc2 = new TestALC("ALC2-ByteLoad", testDir,
            asmName => asmName.Name == "PluginWorker" ? workerAsm : null);
        Assembly callerAsm = alc2.LoadFromAssemblyPath(Path.Combine(testDir, "PluginCaller.dll"));

        Type workerType = workerAsm.GetType("Workers.Worker")!;
        object workerInstance = Activator.CreateInstance(workerType)!;

        Type callerType = callerAsm.GetType("Callers.Caller")!;
        MethodInfo callMethod = callerType.GetMethod("Call",
            BindingFlags.Public | BindingFlags.Static)!;
        Type myTypeALC2 = callMethod.GetParameters()[1].ParameterType;
        object myTypeInstance = Activator.CreateInstance(myTypeALC2)!;

        MissingMethodException mme = InvokeExpectingMissingMethod(callMethod, null,
            new object?[] { workerInstance, myTypeInstance });

        string msg = mme.Message;
        Console.WriteLine($"  Message: {msg}");

        AssertContains(msg, "Plugins.MyType",       "type name");
        AssertContains(msg, "PluginType",             "assembly name");
        AssertContains(msg, "loaded from byte array", "byte-array variant indicator");
        AssertContains(msg, "different contexts",     "ALC mismatch text");
    }

    // ---------------------------------------------------------------------------
    // Test 3: both Worker and Caller are in the same ALC.
    //   - Both use the same PluginType instance  ->  no mismatch  ->  no exception.
    // ---------------------------------------------------------------------------
    static void TestNoMismatch_SameALC()
    {
        string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        // Single ALC: loads everything from the test directory.
        var alc = new TestALC("ALC-Same", testDir);
        Assembly workerAsm = alc.LoadFromAssemblyPath(Path.Combine(testDir, "PluginWorker.dll"));
        Assembly callerAsm = alc.LoadFromAssemblyPath(Path.Combine(testDir, "PluginCaller.dll"));

        Type workerType = workerAsm.GetType("Workers.Worker")!;
        object workerInstance = Activator.CreateInstance(workerType)!;

        // MyType from ALC's own PluginType (same instance as Worker.Process expects).
        Type callerType = callerAsm.GetType("Callers.Caller")!;
        MethodInfo callMethod = callerType.GetMethod("Call",
            BindingFlags.Public | BindingFlags.Static)!;
        Type myType = callMethod.GetParameters()[1].ParameterType;
        object myTypeInstance = Activator.CreateInstance(myType)!;

        // Should succeed without throwing any exception.
        callMethod.Invoke(null, new object?[] { workerInstance, myTypeInstance });
    }

    // ---------------------------------------------------------------------------
    // Entry point
    // ---------------------------------------------------------------------------
    static int Main()
    {
        bool passed = true;

        void Run(string name, Action test)
        {
            try
            {
                test();
                Console.WriteLine($"PASS: {name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAIL: {name}");
                Console.WriteLine($"  {ex}");
                passed = false;
            }
        }

        Run("TestALCMismatch_PathLoad",   TestALCMismatch_PathLoad);
        Run("TestALCMismatch_ByteLoad",   TestALCMismatch_ByteLoad);
        Run("TestNoMismatch_SameALC",     TestNoMismatch_SameALC);

        return passed ? 100 : -1;
    }
}
