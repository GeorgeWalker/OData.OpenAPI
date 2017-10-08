﻿//---------------------------------------------------------------------
// <copyright file="OpenApiRequestBody.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.OpenAPI
{
    /// <summary>
    /// Request Body Object
    /// </summary>
    internal class OpenApiRequestBody : IOpenApiElement, IOpenApiWritable
    {
        /// <summary>
        /// Write Any object to the given writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void Write(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            // { for json, empty for YAML
            writer.WriteStartObject();

            // } for json, empty for YAML
            writer.WriteEndObject();
        }
    }
}
