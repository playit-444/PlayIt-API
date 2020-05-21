﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Networking.Data.General.Messaging
{
    /// <summary>
    /// Simple interface for creating an announcement
    /// </summary>
    public interface IAnnouncement
    {
        /// <summary>
        /// The message of the announcement
        /// </summary>
        public string Message { get; set; }
    }
}
