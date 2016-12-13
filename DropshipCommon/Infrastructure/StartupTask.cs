using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DropshipCommon.Infrastructure
{
    /// <summary>
    /// Interface which should be implemented by tasks run on startup
    /// </summary>
    public interface IStartupTask
    {
        /// <summary>
        /// Execute task
        /// </summary>
        void Execute();

        /// <summary>
        /// Order
        /// </summary>
        int Order { get; }
    }

    
}
