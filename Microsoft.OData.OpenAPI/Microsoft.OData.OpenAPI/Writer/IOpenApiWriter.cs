﻿//---------------------------------------------------------------------
// <copyright file="IOpenApiWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.OpenAPI
{
    /// <summary>
    /// Interface for writing Open API documentation.
    /// </summary>
    internal interface IOpenApiWriter
    {
        /// <summary>
        /// Write the start object.
        /// </summary>
        void WriteStartObject();

        /// <summary>
        /// Write the end object.
        /// </summary>
        void WriteEndObject();

        /// <summary>
        /// Write the start array.
        /// </summary>
        void WriteStartArray();

        /// <summary>
        /// Write the end array.
        /// </summary>
        void WriteEndArray();

        /// <summary>
        /// Write the start property.
        /// </summary>
        void WriteStartProperty(string name);

        /// <summary>
        /// Write the end property.
        /// </summary>
        void WriteEndProperty();

        /// <summary>
        /// Write the property name.
        /// </summary>
        // void WritePropertyName(string name);

        /// <summary>
        /// Write the property name & property value.
        /// </summary>
        // void WriteProperty(string name, string value);

        /// <summary>
        /// Write the property name & property value.
        /// </summary>
        void WriteOptionalProperty(string name, object value);

        /// <summary>
        /// Write the string value.
        /// </summary>
        void WriteValue(string value);

        /// <summary>
        /// Write the decimal value.
        /// </summary>
        void WriteValue(decimal value);

        /// <summary>
        /// Write the int value.
        /// </summary>
        void WriteValue(int value);

        /// <summary>
        /// Write the boolean value.
        /// </summary>
        void WriteValue(bool value);

        /// <summary>
        /// Write the null value.
        /// </summary>
        void WriteNull();

        /// <summary>
        /// Write the object value.
        /// </summary>
        /// <param name="value"></param>
        void WriteValue(object value);

        /// <summary>
        /// Flush the writer.
        /// </summary>
        void Flush();
    }
}
