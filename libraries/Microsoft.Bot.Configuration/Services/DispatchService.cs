﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Bot.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Newtonsoft.Json;

    public class DispatchService : LuisService
    {
        public DispatchService()
        {
            this.Type = ServiceTypes.Dispatch;
        }

        /// <summary>
        /// Gets or sets the service IDs to include in the dispatch model.
        /// </summary>
        [JsonProperty("serviceIds")]
        public List<string> ServiceIds { get; set; } = new List<string>();
    }
}
