// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Plugins;
using Workers;

namespace Callers
{
    // When this method is JIT-compiled in an AssemblyLoadContext where Worker comes
    // from one context (ALC1) and MyType comes from a different context (ALC2), the
    // runtime detects the ALC mismatch while resolving the Worker.Process MemberRef
    // and throws a MissingMethodException with diagnostic information.
    public class Caller
    {
        public static void Call(Worker w, MyType t)
        {
            w.Process(t);
        }
    }
}
