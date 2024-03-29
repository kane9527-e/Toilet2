﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.IO;

namespace GameMain.Scripts.Editor.DataTableGenerator
{
    public sealed partial class DataTableProcessor
    {
        private sealed class DateTimeProcessor : GenericDataProcessor<DateTime>
        {
            public override bool IsSystem => true;

            public override string LanguageKeyword => "DateTime";

            public override string[] GetTypeStrings()
            {
                return new[]
                {
                    "datetime",
                    "system.datetime"
                };
            }

            public override DateTime Parse(string value)
            {
                return DateTime.Parse(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter,
                string value)
            {
                binaryWriter.Write(Parse(value).Ticks);
            }
        }
    }
}