﻿//---------------------------------------------------------------------
// <copyright file="OpenApiYamlWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;

namespace Microsoft.OData.OpenAPI
{
    /// <summary>
    /// YAML writer.
    /// </summary>
    internal class OpenApiYamlWriter : OpenApiWriterBase
    {
        protected override int IndentShift { get { return -1; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiYamlWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        public OpenApiYamlWriter(TextWriter textWriter)
            : this(textWriter, new OpenApiWriterSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiYamlWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="settings">The writer settings.</param>
        public OpenApiYamlWriter(TextWriter textWriter, OpenApiWriterSettings settings)
            : base(textWriter, settings)
        {
        }

        /// <summary>
        /// Write YAML start object.
        /// </summary>
        public override void WriteStartObject()
        {
            Scope preScope = CurrentScope();

            Scope curScope = StartScope(ScopeType.Object);

            IncreaseIndentation();

            if (preScope != null && preScope.Type == ScopeType.Array)
            {
                curScope.IsInArray = true;
            }
        }

        /// <summary>
        /// Write YAML end object.
        /// </summary>
        public override void WriteEndObject()
        {
            Scope current = EndScope(ScopeType.Object);

            if (current.ObjectCount == 0)
            {
                Writer.Write(JsonConstants.WhiteSpaceForEmptyObjectArray);
            }

            DecreaseIndentation();
        }

        /// <summary>
        /// Write YAML start array.
        /// </summary>
        public override void WriteStartArray()
        {
            StartScope(ScopeType.Array);
            IncreaseIndentation();
        }

        /// <summary>
        /// Write YAML end array.
        /// </summary>
        public override void WriteEndArray()
        {
            EndScope(ScopeType.Array);
            DecreaseIndentation();
        }

        /// <summary>
        /// Write the start property.
        /// </summary>
        public override void WriteStartProperty(string name)
        {
            ValifyCanWritePropertyName(name);

            Scope current = CurrentScope();

            if (current.ObjectCount == 0)
            {
                if (current.IsInArray)
                {
                    Writer.WriteLine();

                    WritePrefixIndentation();

                    Writer.Write(JsonConstants.PrefixOfArrayItem);
                }
                else
                {
                    WriteIndentation();
                }
            }
            else
            {
                Writer.WriteLine();
                WriteIndentation();
            }

            Writer.Write(name);
            Writer.Write(JsonConstants.NameValueSeparator);

            ++current.ObjectCount;

            base.WriteStartProperty(name);
        }

        /// <summary>
        /// Write property name.
        /// </summary>
        /// <param name="name">The property name.</param>
        public override void WritePropertyName(string name)
        {
            ValifyCanWritePropertyName(name);

            Scope current = CurrentScope();

            if (current.ObjectCount == 0)
            {
                if (current.IsInArray)
                {
                    Writer.WriteLine();

                    WritePrefixIndentation();

                    Writer.Write(JsonConstants.PrefixOfArrayItem);
                }
                else
                {
                    WriteIndentation();
                }
            }
            else
            {
                Writer.WriteLine();
                WriteIndentation();
            }

            Writer.Write(name);
            Writer.Write(JsonConstants.NameValueSeparator);

            ++current.ObjectCount;
        }

        public override void WriteValue(string value)
        {
            WriteArrayPrefix();

            value = value.Replace("\n", "\\n");
            Writer.Write(value);
        }

        public override void WriteValue(decimal value)
        {
            WriteArrayPrefix();
            Writer.Write(value);
        }

        public override void WriteValue(int value)
        {
            WriteArrayPrefix();
            Writer.Write(value);
        }

        public override void WriteValue(bool value)
        {
            WriteArrayPrefix();
            Writer.Write(value);
        }

        /// <summary>
        /// the empty scalar as “null”.
        /// </summary>
        public override void WriteNull()
        {
            WriteArrayPrefix();
            // nothing here
        }

        private void WriteArrayPrefix()
        {
            if (IsArrayScope())
            {
                Writer.WriteLine();
                WriteIndentation();
                Writer.Write(JsonConstants.PrefixOfArrayItem);
            }
        }
    }
}
