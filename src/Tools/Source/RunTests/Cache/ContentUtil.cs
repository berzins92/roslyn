﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RunTests.Cache
{
    internal sealed class ContentUtil
    {
        private readonly TestExecutionOptions _options;
        private readonly MD5 _hash = MD5.Create();
        private readonly Dictionary<string, string> _fileToChecksumMap = new Dictionary<string, string>();

        internal ContentUtil(TestExecutionOptions options)
        {
            _options = options;
        }

        internal ContentFile GetTestResultContentFile(AssemblyInfo assemblyInfo)
        {
            var content = BuildTestResultContent(assemblyInfo);
            var checksum = GetChecksum(content);
            return new ContentFile(checksum: checksum, content: content);
        }

        private string BuildTestResultContent(AssemblyInfo assemblyInfo)
        {
            var builder = new StringBuilder();
            var assemblyPath = assemblyInfo.AssemblyPath;
            builder.AppendLine($"Assembly: {Path.GetFileName(assemblyPath)} {GetFileChecksum(assemblyPath)}");
            builder.AppendLine($"Display Name: {assemblyInfo.DisplayName}");
            builder.AppendLine($"Results File Name; {assemblyInfo.ResultsFileName}");

            var configFilePath = $"{assemblyPath}.config";
            var configFileChecksum = File.Exists(configFilePath)
                ? GetFileChecksum(configFilePath)
                : "<no config file>";
            builder.AppendLine($"Config: {Path.GetFileName(configFilePath)} {configFileChecksum}");

            builder.AppendLine($"Xunit: {Path.GetFileName(_options.XunitPath)} {GetFileChecksum(_options.XunitPath)}");
            AppendReferences(builder, assemblyPath);
            builder.AppendLine("Options:");
            builder.AppendLine($"\t{nameof(_options.Test64)} - {_options.Test64}");
            builder.AppendLine($"\t{nameof(_options.UseHtml)} - {_options.UseHtml}");
            builder.AppendLine($"\t{nameof(_options.Trait)} - {_options.Trait}");
            builder.AppendLine($"\t{nameof(_options.NoTrait)} - {_options.NoTrait}");
            builder.AppendLine($"Extra Options: {assemblyInfo.ExtraArguments}");
            AppendExtra(builder, assemblyPath);

            return builder.ToString();
        }

        private void AppendReferences(StringBuilder builder, string assemblyPath)
        {
            builder.AppendLine("References:");

            var binariesPath = Path.GetDirectoryName(assemblyPath);
            var assemblyUtil = new AssemblyUtil(binariesPath);
            var visitedSet = new HashSet<string>();
            var missingSet = new HashSet<string>();
            var toVisit = new Queue<AssemblyName>(assemblyUtil.GetReferencedAssemblies(assemblyPath));
            var references = new List<Tuple<string, string>>();

            while (toVisit.Count > 0)
            {
                var current = toVisit.Dequeue();
                if (!visitedSet.Add(current.FullName))
                {
                    continue;
                }

                string currentPath;
                if (assemblyUtil.TryGetAssemblyPath(current, out currentPath))
                {
                    foreach (var name in assemblyUtil.GetReferencedAssemblies(currentPath))
                    {
                        toVisit.Enqueue(name);
                    }

                    var currentHash = GetFileChecksum(currentPath);
                    references.Add(Tuple.Create(current.Name, currentHash));
                }
                else if (assemblyUtil.IsKnownMissingAssembly(current))
                {
                    references.Add(Tuple.Create(current.Name, "<missing light up reference>"));
                }
                else
                {
                    missingSet.Add(current.FullName);
                }
            }

            references.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            foreach (var pair in references)
            {
                builder.AppendLine($"\t{pair.Item1} {pair.Item2}");
            }

            // Error if there are any referenced assemblies that we were unable to resolve.
            if (missingSet.Count > 0)
            {
                var errorBuilder = new StringBuilder();
                errorBuilder.AppendLine($"Unable to resolve {missingSet.Count} referenced assemblies");
                foreach (var item in missingSet.OrderBy(x => x))
                {
                    errorBuilder.AppendLine($"\t{item}");
                }
                throw new Exception(errorBuilder.ToString());
            }
        }

        private void AppendExtra(StringBuilder builder, string assemblyPath)
        {
            builder.AppendLine("Extra Files:");
            var all = new[]
            {
                "*.targets",
                "*.props"
            };

            var binariesPath = Path.GetDirectoryName(assemblyPath);
            foreach (var ext in all)
            {
                foreach (var file in Directory.EnumerateFiles(binariesPath, ext))
                {
                    builder.AppendLine($"\t{Path.GetFileName(file)} - {GetFileChecksum(file)}");
                }
            }
        }

        private string GetChecksum(string content)
        {
            var contentBytes = Encoding.UTF8.GetBytes(content);
            var hashBytes = _hash.ComputeHash(contentBytes);
            return HashBytesToString(hashBytes);
        }

        private static string HashBytesToString(byte[] hash)
        {
            var data = BitConverter.ToString(hash);
            return data.Replace("-", "");
        }

        private string GetFileChecksum(string filePath)
        {
            string checksum;
            if (_fileToChecksumMap.TryGetValue(filePath, out checksum))
            {
                return checksum;
            }

            checksum = GetFileChecksumCore(filePath);
            _fileToChecksumMap.Add(filePath, checksum);
            return checksum;
        }

        private string GetFileChecksumCore(string filePath)
        {
            var bytes = File.ReadAllBytes(filePath);
            var hashBytes = _hash.ComputeHash(bytes);
            return HashBytesToString(hashBytes);
        }
    }
}
