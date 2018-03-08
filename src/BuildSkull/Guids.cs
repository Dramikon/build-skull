// Guids.cs
// MUST match guids.h
using System;

namespace BuildSkull
{
    public static class GuidList
    {
        public const string guidBuildSkullPkgString = "4dbd561c-41b1-45f8-8430-7c6105b77f94";
        public const string guidBuildSkullCmdSetString = "3dffb33c-1e09-436d-ba29-5833844d7360";
        public const string guidToolWindowPersistanceString = "9aacab4f-a6cd-4a00-8bc8-196bdfcc12bf";

        public static readonly Guid guidBuildSkullCmdSet = new Guid(guidBuildSkullCmdSetString);
    };
}