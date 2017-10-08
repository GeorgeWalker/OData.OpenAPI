﻿//---------------------------------------------------------------------
// <copyright file="OpenApiDictionaryOfT.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.OpenAPI
{
    /// <summary>
    /// Open Api Dictionary of T.
    /// </summary>
    internal abstract class OpenApiDictionary<T> : Dictionary<string, T>,
        IOpenApiElement,
        IOpenApiWritable
        where T : IOpenApiElement, IOpenApiWritable
    {
        /// <summary>
        /// Write Any object to the given writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public virtual void Write(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            // { for json, empty for YAML
            writer.WriteStartObject();

            // path items
            foreach (var item in this)
            {
                writer.WriteRequiredObject(item.Key, item.Value);
            }

            // } for json, empty for YAML
            writer.WriteEndObject();
        }
    }
}
