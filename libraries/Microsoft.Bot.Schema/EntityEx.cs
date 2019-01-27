﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Bot.Schema
{
    public partial class Entity
    {
        /// <summary>
        /// Gets or sets properties that are not otherwise defined by the <see cref="Entity"/> type but that
        /// might appear in the REST JSON object.
        /// </summary>
        /// <value>The extended properties for the object.</value>
        /// <remarks>With this, properties not represented in the defined type are not dropped when
        /// the JSON object is deserialized, but are instead stored in this property. Such properties
        /// will be written to a JSON object when the instance is serialized.</remarks>
        [JsonExtensionData(ReadData = true, WriteData = true)]
        public JObject Properties { get; set; } = new JObject();

        /// <summary>
        /// Retrieve internal payload.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetAs<T>()
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Set internal payload.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void SetAs<T>(T obj)
        {
            var entity = JsonConvert.DeserializeObject<Entity>(JsonConvert.SerializeObject(obj));
            this.Type = entity.Type;
            this.Properties = entity.Properties;
        }
    }
}
