﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Docfx.HtmlToPdf
{
    using System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Docfx.Common;

    public class ManifestItem
    {
        [JsonProperty(ManifestConstants.BuildManifestItem.Type)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ManifestItemType Type { get; set; }

        [JsonProperty(ManifestConstants.BuildManifestItem.OriginalType)]
        public string OriginalType { get; set; }

        [JsonProperty(ManifestConstants.BuildManifestItem.Original)]
        public string Original { get; set; }

        [JsonProperty(ManifestConstants.BuildManifestItem.Output)]
        public FileOutputs Output { get; set; }

        [JsonProperty(ManifestConstants.BuildManifestItem.SkipPublish, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool SkipPublish { get; set; }

        [JsonProperty(ManifestConstants.BuildManifestItem.SkipSchemaCheck, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool SkipSchemaCheck { get; set; }

        [JsonProperty(ManifestConstants.BuildManifestItem.IsThemeResource, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsThemeResource { get; set; }

        /// <summary>
        /// Article monikers, aggregation by docfx plugin.
        /// </summary>
        [JsonProperty(ManifestConstants.BuildManifestItem.Monikers, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string[] Monikers { get; set; }

        /// <summary>
        /// If it's separate version, version will have value. Otherwise, it's null/empty.
        /// </summary>
        [JsonProperty(ManifestConstants.BuildManifestItem.Version, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Version { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> Metadata { get; set; }

        public override string ToString()
        {
            return JsonUtility.ToJsonString(this);
        }
    }
}
