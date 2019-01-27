﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Bot.Configuration
{
    using Microsoft.Bot.Configuration.Encryption;
    using Newtonsoft.Json;

    public class CosmosDbService : AzureService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosDbService "/> class.
        /// </summary>
        public CosmosDbService()
            : base(ServiceTypes.CosmosDB)
        {
        }

        /// <summary>
        /// Gets or sets endpoint for CosmosDB.
        /// </summary>
        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets key for accessing CosmosDB.
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets database.
        /// </summary>
        [JsonProperty("database")]
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets collection.
        /// </summary>
        [JsonProperty("collection")]
        public string Collection { get; set; }

        /// <inheritdoc/>
        public override void Encrypt(string secret)
        {
            base.Encrypt(secret);

            if (!string.IsNullOrEmpty(this.Key))
            {
                this.Key = this.Key.Encrypt(secret);
            }
        }

        /// <inheritdoc/>
        public override void Decrypt(string secret)
        {
            base.Decrypt(secret);

            if (!string.IsNullOrEmpty(this.Key))
            {
                this.Key = this.Key.Decrypt(secret);
            }
        }
    }
}
