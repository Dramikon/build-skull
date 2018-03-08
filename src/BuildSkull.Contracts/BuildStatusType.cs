using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSkull.Contracts
{
    /// <summary>
    /// Build status enum based on TFS API build run statuses
    /// </summary>
    [Flags]
    public enum BuildStatusType
    {
        // Summary:
        //     No status available.
        None = 0,
        //
        // Summary:
        //     Build is in progress.
        InProgress = 1,
        //
        // Summary:
        //     Build succeeded.
        Succeeded = 2,
        //
        // Summary:
        //     Build is partially succeeded.
        PartiallySucceeded = 4,
        //
        // Summary:
        //     Build failed.
        Failed = 8,
        //
        // Summary:
        //     Build is stopped.
        Stopped = 16,
        //
        // Summary:
        //     Build is not started.
        NotStarted = 32,
        //
        // Summary:
        //     All status applies.
        All = 63,
    }
}
