﻿using System.Text;

namespace Dunet.GenerateUnionRecord;

internal static class UnionRecordSource
{
    public static string GenerateRecord(UnionRecord record)
    {
        var builder = new StringBuilder();

        foreach (var import in record.Imports)
        {
            builder.AppendLine(import);
        }

        if (record.Namespace is not null)
        {
            builder.AppendLine($"namespace {record.Namespace};");
        }

        builder.Append($"abstract partial record {record.Name}");
        builder.AppendTypeParams(record.TypeParameters);
        builder.AppendLine();
        builder.AppendLine("{");
        builder.AppendLine($"    private {record.Name}() {{}}");
        builder.AppendLine();

        builder.AppendLine("    public abstract TMatchOutput Match<TMatchOutput>(");
        for (int i = 0; i < record.Members.Count; ++i)
        {
            var member = record.Members[i];
            builder.Append($"        System.Func<{member.Name}");
            builder.AppendTypeParams(member.TypeParameters);
            builder.Append($", TMatchOutput> {member.Name.ToMethodParameterCase()}");
            if (i < record.Members.Count - 1)
            {
                builder.Append(",");
            }
            builder.AppendLine();
        }
        builder.AppendLine("    );");
        builder.AppendLine();

        foreach (var member in record.Members)
        {
            builder.Append($"    public sealed partial record {member.Name}");
            builder.AppendTypeParams(member.TypeParameters);
            builder.Append($" : {record.Name}");
            builder.AppendTypeParams(record.TypeParameters);
            builder.AppendLine();
            builder.AppendLine("    {");
            builder.AppendLine("        public override TMatchOutput Match<TMatchOutput>(");
            for (int i = 0; i < record.Members.Count; ++i)
            {
                var memberParam = record.Members[i];
                builder.Append($"            System.Func<{memberParam.Name}");
                builder.AppendTypeParams(memberParam.TypeParameters);
                builder.Append($", TMatchOutput> {memberParam.Name.ToMethodParameterCase()}");
                if (i < record.Members.Count - 1)
                {
                    builder.Append(",");
                }
                builder.AppendLine();
            }
            builder.AppendLine($"        ) => {member.Name.ToMethodParameterCase()}(this);");
            builder.AppendLine("    }");
            builder.AppendLine();
        }

        builder.AppendLine("}");

        return builder.ToString();
    }
}
