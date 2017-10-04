﻿//---------------------------------------------------------------------
// <copyright file="OpenApiWriterTestHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;

namespace Microsoft.OData.OpenAPI.Tests
{
    internal static class OpenApiWriterTestHelper
    {
        internal static string WriteToJson(this IOpenApiElement element,
            Action<IOpenApiWriter, IOpenApiElement> before = null,
            Action<IOpenApiWriter, IOpenApiElement> after = null)
        {
            Action<IOpenApiWriter> action = writer =>
            {
                before?.Invoke(writer, element);
                element?.Write(writer);
                after?.Invoke(writer, element);
                writer?.Flush();
            };

            return Write(OpenApiTarget.Json, action);
        }

        internal static string WriteToYaml(this IOpenApiElement element,
            Action<IOpenApiWriter, IOpenApiElement> before = null,
            Action<IOpenApiWriter, IOpenApiElement> after = null)
        {
            Action<IOpenApiWriter> action = writer =>
            {
                before?.Invoke(writer, element);
                element?.Write(writer);
                after?.Invoke(writer, element);
                writer?.Flush();
            };

            return Write(OpenApiTarget.Yaml, action);
        }

        internal static string Write(this IOpenApiElement element,
            OpenApiTarget target,
            Action<IOpenApiWriter, IOpenApiElement> before = null,
            Action<IOpenApiWriter, IOpenApiElement> after = null)
        {
            Action<IOpenApiWriter> action = writer =>
            {
                before?.Invoke(writer, element);
                element?.Write(writer);
                after?.Invoke(writer, element);
                writer?.Flush();
            };

            return Write(target, action);
        }

        internal static string Write(OpenApiTarget target, Action<IOpenApiWriter> action)
        {
            MemoryStream stream = new MemoryStream();
            IOpenApiWriter writer;
            if (target == OpenApiTarget.Yaml)
            {
                writer = new OpenApiYamlWriter(new StreamWriter(stream));
            }
            else
            {
                writer = new OpenApiJsonWriter(new StreamWriter(stream));
            }

            action(writer);
            writer.Flush();
            stream.Position = 0;
            return new StreamReader(stream).ReadToEnd();
        }
    }
}
