﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Microsoft.Bot.Builder.AI.QnA
{
    /// <summary>
    /// This class represents all the trace info that we collect from the QnAMaker Middleware.
    /// </summary>
    public class QnAMakerTraceInfo
    {
        /// <summary>
        /// Gets or sets message which instigated the query to QnAMaker.
        /// </summary>
        /// <value>
        /// Message which instigated the query to QnAMaker.
        /// </value>
        [JsonProperty("message")]
        public Activity Message { get; set; }

        /// <summary>
        /// Gets or sets results that QnAMaker returned.
        /// </summary>
        /// <value>
        /// Results that QnAMaker returned.
        /// </value>
        [JsonProperty("queryResults")]
        public QueryResult[] QueryResults { get; set; }

        /// <summary>
        /// Gets or sets iD of the Knowledgebase that is being used.
        /// </summary>
        /// <value>
        /// ID of the Knowledgebase that is being used.
        /// </value>
        [JsonProperty("knowledgeBaseId")]
        public string KnowledgeBaseId { get; set; }

        /// <summary>
        /// Gets or sets the minimum score threshold, used to filter returned results.
        /// </summary>
        /// <remarks>Scores are normalized to the range of 0.0 to 1.0
        /// before filtering.</remarks>
        /// <value>
        /// The minimum score threshold, used to filter returned results.
        /// </value>
        [JsonProperty("scoreThreshold")]
        public float ScoreThreshold { get; set; }

        /// <summary>
        /// Gets or sets number of ranked results that are asked to be returned.
        /// </summary>
        /// <value>
        /// Number of ranked results that are asked to be returned.
        /// </value>
        [JsonProperty("top")]
        public int Top { get; set; }

        /// <summary>
        /// Gets or sets the filters used to return answers that have the specified metadata.
        /// </summary>
        /// <value>
        /// The filters used to return answers that have the specified metadata.
        /// </value>
        [JsonProperty("strictFilters")]
        public Metadata[] StrictFilters { get; set; }

        /// <summary>
        /// Gets or sets miscellaneous data to boost answers.
        /// </summary>
        /// <value>
        /// Miscellaneous data to boost answers.
        /// </value>
        [JsonProperty("metadataBoost")]
        public Metadata[] MetadataBoost { get; set; }
    }
}
