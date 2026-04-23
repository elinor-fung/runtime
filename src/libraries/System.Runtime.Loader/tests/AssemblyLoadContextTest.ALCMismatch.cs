// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Tests for the ALC-mismatch diagnostic in MissingMethodException (memberload.cpp).
//
// Scenario:
//   ALCMismatch.PluginWorker.dll  defines  Workers.Worker.Process(Plugins.MyType)
//   ALCMismatch.PluginCaller.dll  defines  Callers.Caller.Call(Worker w, MyType t) { w.Process(t); }
//   ALCMismatch.PluginType.dll    defines  Plugins.MyType
//
// To trigger the mismatch two AssemblyLoadContexts are set up:
//   ALC1  loads PluginWorker (which resolves its PluginType dependency into ALC1).
//   ALC2  delegates PluginWorker → ALC1's copy, but loads its own independent PluginType.
//
// When PluginCaller is loaded in ALC2 and Caller.Call is JIT-compiled:
//   - The MemberRef for w.Process(t) has MyType resolved from ALC2.
//   - Worker.Process (from ALC1) expects ALC1's MyType.
//   - FindMethod returns null → ThrowMissingMethodException
//   → FindSignatureTypeMismatch detects the ALC mismatch and annotates the message.

using System.IO;
using System.Reflection;
using Xunit;

namespace System.Runtime.Loader.Tests
{
    public partial class AssemblyLoadContextTest
    {
        // ALC that loads assemblies from a directory; an optional delegate can
        // intercept resolution for specific assembly names.
        private class IsolatingALC : AssemblyLoadContext
        {
            private readonly string _dir;
            private readonly Func<AssemblyName, Assembly?>? _override;

            public IsolatingALC(string name, string dir,
                                 Func<AssemblyName, Assembly?>? @override = null)
                : base(name, isCollectible: false)
            {
                _dir = dir;
                _override = @override;
            }

            protected override Assembly? Load(AssemblyName assemblyName)
            {
                Assembly? overridden = _override?.Invoke(assemblyName);
                if (overridden != null)
                    return overridden;

                string path = Path.Combine(_dir, assemblyName.Name + ".dll");
                return File.Exists(path) ? LoadFromAssemblyPath(path) : null;
            }
        }

        // Invokes the method and returns the MissingMethodException, whether it is
        // thrown directly (during JIT) or wrapped in a TargetInvocationException.
        private static MissingMethodException InvokeExpectingMissingMethod(
            MethodInfo method, object? target, object?[] args)
        {
            try
            {
                method.Invoke(target, args);
            }
            catch (TargetInvocationException tie) when (tie.InnerException is MissingMethodException mme)
            {
                return mme;
            }
            catch (MissingMethodException mme)
            {
                return mme;
            }

            throw new InvalidOperationException(
                $"Expected MissingMethodException but {method.Name} returned without throwing.");
        }

        // -------------------------------------------------------------------------
        // Test 1: both ALCs load PluginType from a file path.
        //   The message must identify the type, assembly name, file path, and the
        //   phrase "different contexts".
        // -------------------------------------------------------------------------
        [Fact]
        [SkipOnMono("ALC mismatch MissingMethodException diagnostic is a CoreCLR-specific feature")]
        public static void MissingMethodException_ALCMismatch_PathLoad_ContainsDiagnosticInfo()
        {
            string testDir = Path.GetDirectoryName(typeof(AssemblyLoadContextTest).Assembly.Location)!;

            // ALC1: loads PluginWorker and resolves its PluginType dependency from the test dir.
            var alc1 = new IsolatingALC("ALC1-PathLoad", testDir);
            Assembly workerAsm = alc1.LoadFromAssemblyPath(
                Path.Combine(testDir, "ALCMismatch.PluginWorker.dll"));

            // ALC2: delegates PluginWorker to ALC1 but loads its own PluginType from path.
            var alc2 = new IsolatingALC("ALC2-PathLoad", testDir,
                name => name.Name == "ALCMismatch.PluginWorker" ? workerAsm : null);
            Assembly callerAsm = alc2.LoadFromAssemblyPath(
                Path.Combine(testDir, "ALCMismatch.PluginCaller.dll"));

            // Worker instance from ALC1; MyType instance from ALC2's fresh PluginType.
            object workerInstance = Activator.CreateInstance(workerAsm.GetType("Workers.Worker")!)!;
            Type callerType = callerAsm.GetType("Callers.Caller")!;
            MethodInfo callMethod = callerType.GetMethod("Call",
                BindingFlags.Public | BindingFlags.Static)!;
            object myTypeInstance = Activator.CreateInstance(
                callMethod.GetParameters()[1].ParameterType)!;

            // JIT-compiling Caller.Call in ALC2's context detects that the MemberRef for
            // Worker.Process uses ALC2's MyType while the actual method expects ALC1's MyType.
            MissingMethodException ex = InvokeExpectingMissingMethod(callMethod, null,
                new object?[] { workerInstance, myTypeInstance });

            Assert.Contains("Plugins.MyType", ex.Message);
            Assert.Contains("ALCMismatch.PluginType", ex.Message);
            Assert.Contains("loaded at", ex.Message);
            Assert.Contains("different contexts", ex.Message);
        }

        // -------------------------------------------------------------------------
        // Test 2: ALC1 loads PluginType from a byte array (no file path).
        //   The message must say "loaded from byte array" for that side.
        // -------------------------------------------------------------------------
        [Fact]
        [SkipOnMono("ALC mismatch MissingMethodException diagnostic is a CoreCLR-specific feature")]
        public static void MissingMethodException_ALCMismatch_ByteLoad_ContainsDiagnosticInfo()
        {
            string testDir = Path.GetDirectoryName(typeof(AssemblyLoadContextTest).Assembly.Location)!;
            string pluginTypePath = Path.Combine(testDir, "ALCMismatch.PluginType.dll");

            // ALC1: load PluginType from a byte array so that it has no associated file path.
            Assembly? pluginTypeByteAsm = null;
            var alc1 = new IsolatingALC("ALC1-ByteLoad", testDir, name =>
            {
                if (name.Name == "ALCMismatch.PluginType" && pluginTypeByteAsm != null)
                    return pluginTypeByteAsm;
                return null;
            });
            pluginTypeByteAsm = alc1.LoadFromStream(
                new MemoryStream(File.ReadAllBytes(pluginTypePath)));
            Assembly workerAsm = alc1.LoadFromAssemblyPath(
                Path.Combine(testDir, "ALCMismatch.PluginWorker.dll"));

            // ALC2: delegates PluginWorker to ALC1; loads PluginType from path.
            var alc2 = new IsolatingALC("ALC2-ByteLoad", testDir,
                name => name.Name == "ALCMismatch.PluginWorker" ? workerAsm : null);
            Assembly callerAsm = alc2.LoadFromAssemblyPath(
                Path.Combine(testDir, "ALCMismatch.PluginCaller.dll"));

            object workerInstance = Activator.CreateInstance(workerAsm.GetType("Workers.Worker")!)!;
            Type callerType = callerAsm.GetType("Callers.Caller")!;
            MethodInfo callMethod = callerType.GetMethod("Call",
                BindingFlags.Public | BindingFlags.Static)!;
            object myTypeInstance = Activator.CreateInstance(
                callMethod.GetParameters()[1].ParameterType)!;

            MissingMethodException ex = InvokeExpectingMissingMethod(callMethod, null,
                new object?[] { workerInstance, myTypeInstance });

            Assert.Contains("Plugins.MyType", ex.Message);
            Assert.Contains("ALCMismatch.PluginType", ex.Message);
            Assert.Contains("loaded from byte array", ex.Message);
            Assert.Contains("different contexts", ex.Message);
        }

        // -------------------------------------------------------------------------
        // Test 3: Worker and Caller are in the same ALC so MyType is consistent.
        //   No exception should be thrown.
        // -------------------------------------------------------------------------
        [Fact]
        public static void MissingMethodException_SameALC_Succeeds()
        {
            string testDir = Path.GetDirectoryName(typeof(AssemblyLoadContextTest).Assembly.Location)!;

            // Single ALC: both PluginWorker and PluginCaller share the same PluginType.
            var alc = new IsolatingALC("ALC-Same", testDir);
            Assembly workerAsm = alc.LoadFromAssemblyPath(
                Path.Combine(testDir, "ALCMismatch.PluginWorker.dll"));
            Assembly callerAsm = alc.LoadFromAssemblyPath(
                Path.Combine(testDir, "ALCMismatch.PluginCaller.dll"));

            object workerInstance = Activator.CreateInstance(workerAsm.GetType("Workers.Worker")!)!;
            Type callerType = callerAsm.GetType("Callers.Caller")!;
            MethodInfo callMethod = callerType.GetMethod("Call",
                BindingFlags.Public | BindingFlags.Static)!;
            object myTypeInstance = Activator.CreateInstance(
                callMethod.GetParameters()[1].ParameterType)!;

            // Should succeed without any exception.
            callMethod.Invoke(null, new object?[] { workerInstance, myTypeInstance });
        }
    }
}
