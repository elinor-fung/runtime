// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Plugins;

namespace Workers
{
    public class Worker
    {
        // Method whose parameter type (MyType) will be loaded from two separate
        // AssemblyLoadContexts, triggering an ALC-mismatch MissingMethodException.
        public void Process(MyType arg) { }
    }
}
