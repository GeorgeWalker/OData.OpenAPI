﻿//---------------------------------------------------------------------
// <copyright file="OpenApiParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.OpenAPI
{
    enum OpenApiParameterLocation
    {
        Query,
        Header,
        Path,
        Cookie
    }

    internal class OpenApiParameter : IOpenApiElement
    {
        public string Name { get; set; }

        public OpenApiParameterLocation In { get; set; }

        public string Description { get; set; }

        public bool Required { get; set; }

        public bool Deprecated { get; set; }

        public bool AllowEmptyValue { get; set; }

        public virtual void Write(IOpenApiWriter writer)
        {
        }
    }
}