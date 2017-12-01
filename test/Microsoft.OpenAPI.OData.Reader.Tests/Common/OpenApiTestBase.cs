﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Xunit.Abstractions;

namespace Microsoft.OpenApi.OData.Tests
{
    public class OpenApiTestBase
    {
        private readonly ITestOutputHelper output;

        public OpenApiTestBase(ITestOutputHelper output)
        {
            this.output = output;
        }
    }
}
