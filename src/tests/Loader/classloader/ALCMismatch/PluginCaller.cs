// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Plugins;
using Workers;

namespace Callers
{
    // This class is loaded into an ALC where PluginWorker comes from one context
    // and PluginType comes from another.  Invoking Call via reflection triggers JIT
    // compilation of the call-site, at which point the runtime tries to resolve
    // Worker.Process(MyType) and discovers the ALC mismatch.
    public class Caller
    {
        public static void Call(Worker w, MyType t)
        {
            w.Process(t);
        }
    }
}
