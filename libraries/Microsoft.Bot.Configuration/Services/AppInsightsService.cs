﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Bot.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Bot.Configuration.Encryption;
    using Newtonsoft.Json;

    public class AppInsightsService : AzureService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppInsightsService"/> class.
        /// </summary>
        public AppInsightsService()
            : base(ServiceTypes.AppInsights)
        {
        }

        /// <summary>
        /// Gets or sets instrumentation Key.
        /// </summary>
        [JsonProperty("instrumentationKey")]
        public string InstrumentationKey { get; set; }

        /// <summary>
        /// Gets or sets applicationId for programatic access to appInsights.
        /// </summary>
        [JsonProperty("applicationId")]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets apiKeys.
        /// </summary>
        [JsonProperty("apiKeys")]
        public Dictionary<string, string> ApiKeys { get; set; } = new Dictionary<string, string>();

        /// <inheritdoc/>
        public override void Encrypt(string secret)
        {
            base.Encrypt(secret);

            if (!string.IsNullOrEmpty(this.InstrumentationKey))
            {
                this.InstrumentationKey = this.InstrumentationKey.Encrypt(secret);
            }

            if (this.ApiKeys != null)
            {
                foreach (var key in this.ApiKeys.Keys.ToArray())
                {
                    if (!string.IsNullOrEmpty(this.ApiKeys[key]))
                    {
                        this.ApiKeys[key] = this.ApiKeys[key].Encrypt(secret);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override void Decrypt(string secret)
        {
            base.Decrypt(secret);
            if (!string.IsNullOrEmpty(this.InstrumentationKey))
            {
                this.InstrumentationKey = this.InstrumentationKey.Decrypt(secret);
            }

            if (this.ApiKeys != null)
            {
                foreach (var key in this.ApiKeys.Keys.ToArray())
                {
                    if (!string.IsNullOrEmpty(this.ApiKeys[key]))
                    {
                        this.ApiKeys[key] = this.ApiKeys[key].Decrypt(secret);
                    }
                }
            }
        }
    }
}
