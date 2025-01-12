﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GlyphJsonToCsharp.Helpers;
using Humanizer;
using Newtonsoft.Json;

namespace GlyphJsonToCsharp
{
    public class CodeWriter
    {
        public string Write(string jsonFile)
        {
            Console.Write("Reading json file");
            var content = File.ReadAllText(jsonFile);
            
            var icons = JsonConvert.DeserializeObject<Dictionary<string, int>>(content);
            
            Console.Write("Generating C# code...");

            var classTemplate = ReadResourceContent("EnumTemplate.txt");
            var propertyTemplate = ReadResourceContent("EnumPropertyTemplate.txt");

            var properties = new List<string>();

            foreach (var icon in icons)
            {
                var property = propertyTemplate
                    .Replace("$name$", icon.Key)
                    .Replace("$code$", icon.Value.ToString("X").ToLower())
                    .Replace("$dotnet_name$",  DotNetNameHelper.ToDotNetName(icon.Key));
                
                properties.Add(property);
            }

            var separator = Environment.NewLine + Environment.NewLine;
            var code = string.Join(separator, properties);

            var className = DotNetNameHelper.ToDotNetName(Path.GetFileName(jsonFile).Replace(".json", string.Empty))+"Font";

            return classTemplate.Replace("$properties$", code).Replace("$class$", className);
        }
        
        public string ReadResourceContent(string fileName)
        {
            var assembly = typeof(CodeWriter).Assembly;

            string resourceName = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(fileName));
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
