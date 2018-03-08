using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSkull.Contracts
{
    public enum BuildCompilationStatusType
    {
        // Summary:
        //     Status is unknown.
        Unknown = 0,
        //
        // Summary:
        //     Build has failed.
        Failed = 1,
        //
        // Summary:
        //     Build has succeeded.
        Succeeded = 2,
    }
}
