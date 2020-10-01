using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace SetVersion
{
    class Program
    {
        static string GetFullVersion(Version version)
        {
            if (version.Revision != -1)
            {
                return version.Major + "." + version.Minor + "." + version.Build + "." + version.Revision;
            }

            return version.Major + "." + version.Minor + "." + version.Build + ".0";
        }

        static void Main(string[] args)
        {
            Version version = new Version(0, 0, 0, 0);

            string path = Directory.GetCurrentDirectory();
            if (args.Length > 0)
            {
                path = args[0];
            }

            List<string> projectFiles = new List<string>();
            foreach (FileInfo filename in new DirectoryInfo(path).GetFiles("*.csproj", SearchOption.AllDirectories))
            {
                XDocument document = XDocument.Load(filename.FullName);
                foreach (XElement propertyGroup in document.Root.Elements("PropertyGroup"))
                {
                    foreach (XElement item in propertyGroup.Elements("Version"))
                    {
                        projectFiles.Add(filename.FullName);
                        Version projectVersion = Version.Parse(item.Value);
                        if (projectVersion > version)
                        {
                            version = projectVersion;
                        }
                    }
                }
            }

            if (version.Revision == -1)
                version = new Version(version.Major, version.Minor, version.Build + 1);
            else
                version = new Version(version.Major, version.Minor, version.Build + 1, version.Revision);

            foreach (string filename in projectFiles)
            {
                XDocument document = XDocument.Load(filename);
                foreach (XElement propertyGroup in document.Root.Elements("PropertyGroup"))
                {
                    foreach (XElement item in propertyGroup.Elements("Version"))
                    {
                        item.Value = version.ToString();
                    }

                    foreach (XElement item in propertyGroup.Elements("AssemblyVersion"))
                    {
                        item.Value = GetFullVersion(version);
                    }

                    foreach (XElement item in propertyGroup.Elements("FileVersion"))
                    {
                        item.Value = GetFullVersion(version);
                    }
                }

                document.Save(filename);
            }
        }
    }
}
