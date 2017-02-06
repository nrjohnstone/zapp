﻿using Newtonsoft.Json;

namespace Zapp.Config
{
    /// <summary>
    /// Represents the class translated from the json section.
    /// </summary>
    public class RestConfig
    {
        /// <summary>
        /// Represents the port for the rest-api.
        /// </summary>
        [JsonProperty("port")]
        public int Port { get; set; } = 6464;
    }
}