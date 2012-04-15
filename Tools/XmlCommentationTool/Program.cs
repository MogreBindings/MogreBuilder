namespace MogreXml
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;

    class Program
    {
        #region Fields

        private static string managedNamespace;
        private static string nativeNamespace;
        private static StringBuilder stringBuilder = new StringBuilder();
        private static Dictionary<string, XmlDocument> typeDict;

        #endregion Fields

        #region Methods

        #region Public Static Methods

        public static void Main(string[] args)
        {
            // bloody stupid commandline switch parser
            if (args.Length < 3)
            {
                ShowHelpAndQuit();
            }

            string assemblyName = args[0];
            nativeNamespace = args[1];
            managedNamespace = args[2];
            string xmlDir = @".\xml\";

            if (args.Length > 3 )
            {
                for (int i = 3; i < args.Length; i++)
                {
                    if (args[i].StartsWith("-d:"))
                    {
                        xmlDir = args[i].Split(':')[1];
                    }

                }
            }

            // read xml files
            var files = Directory.GetFiles(xmlDir, "*.xml");
            Console.WriteLine("Reading xml files...");
            typeDict = new Dictionary<string, XmlDocument>(files.Length);

            foreach (string file in files)
            {
                var doc = new XmlDocument();
                try
                {
                    doc.Load(file);
                    var node = doc.SelectSingleNode("doxygen/compounddef/compoundname");
                    var name = node.InnerText.Replace(nativeNamespace + "::", string.Empty);
                    typeDict.Add(name, doc);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Warning: Exception while loading " + file);
                    Console.WriteLine("Exception: " + ex.Message);
                }
            }

            // read mogre types
            var asm = Assembly.LoadFrom(assemblyName);
            var types = asm.GetExportedTypes();

            string xmlFile = Path.Combine(Path.GetDirectoryName(assemblyName), Path.GetFileNameWithoutExtension(assemblyName) + ".xml");

            Console.WriteLine("Writing xml comment file...");
            using (var xtw = new XmlTextWriter(xmlFile, null))
            {
                xtw.Formatting = Formatting.Indented;
                xtw.WriteStartDocument();
                xtw.WriteStartElement("doc");

                xtw.WriteStartElement("assembly");
                xtw.WriteElementString("name", Path.GetFileNameWithoutExtension(assemblyName));
                xtw.WriteEndElement();

                xtw.WriteStartElement("members");
                foreach (Type type in types)
                {
                    if (type.IsPublic)
                    {
                        ExportType(xtw, type);
                    }
                }

                xtw.WriteEndElement();
                xtw.WriteEndElement();

            }

            var logList = new List<String>(stringBuilder.ToString().Split(new[]{"\r", "\n"}, StringSplitOptions.RemoveEmptyEntries));
            stringBuilder = new StringBuilder();
            logList.Sort();
            foreach (string line in logList)
            {
                stringBuilder.AppendLine(line);
            }

            File.WriteAllText("MogreXml.log", stringBuilder.ToString());

            Console.WriteLine("Finished.");
        }

        #endregion Public Static Methods

        #region Private Static Methods

        private static void ExportEnum(XmlTextWriter xtw, Type nt, Dictionary<string, List<XmlNode>> memberDict)
        {
            List<XmlNode> nodeList;
            string enumSearchValue = nt.Name.ToUpperInvariant();

            // special case for OperationTypes
            if (nt.Name == "OperationTypes")
            {
                enumSearchValue = "OPERATIONTYPE";
            }

            if (memberDict.TryGetValue(enumSearchValue, out nodeList))
            {
                var node = nodeList[0];
                WriteMemberToXML(xtw, "T:" + nt, node);

                // write enum values, assumes Ogre and Mogre use the same name
                var importEnumNode = node.FirstChild;
                while (importEnumNode != null)
                {
                    if (importEnumNode.Name == "enumvalue")
                    {
                        WriteMemberToXML(xtw,
                                         "F:" + nt + "." + importEnumNode.SelectSingleNode("name").InnerText,
                                         importEnumNode);
                    }

                    importEnumNode = importEnumNode.NextSibling;
                }
            }
            else
            {
                Log("Enum not found: " + nt.Name);
            }
        }

        private static void ExportEvent(XmlTextWriter xtw, Type nt, EventInfo ei)
        {
            XmlDocument doc = null;
            var eventSearchValues = new string[2];
            eventSearchValues[0] = nt.Name + "::" + "Listener";
            eventSearchValues[1] = nt.Name + "Listener";

            string eventName = "E:" + nt + "." + ei.Name;

            foreach (string eventSearchValue in eventSearchValues)
            {
                if (typeDict.TryGetValue(eventSearchValue, out doc))
                {
                    break;
                }
            }

            if (doc != null)
            {
                var memberDict = GetMemberDict(doc);
                List<XmlNode> nodeList;

                if (memberDict.TryGetValue(ei.Name.ToUpperInvariant(), out nodeList))
                {
                    var node = nodeList[0];
                    WriteMemberToXML(xtw, eventName, node);
                    return;
                }
            }

            if (ei.DeclaringType == nt)
            {
                Log("Event not found: " + eventName);
            }
            else
            {
                Log("Event (inherited) not found: " + eventName);
            }
        }

        private static void ExportType(XmlTextWriter xtw, Type type)
        {
            if (type.FullName.StartsWith(managedNamespace + "."))
            {
                string typeSearchValue = type.FullName.Replace(managedNamespace + ".", string.Empty).Replace(".", "::");

                if (type.Name.Contains("_NativePtr"))
                {
                    typeSearchValue = type.Name.Replace("_NativePtr", string.Empty);
                }
                if (type.Name.StartsWith("Const_"))
                {
                    typeSearchValue = type.Name.Replace("Const_", string.Empty);
                }

                XmlDocument doc;

                if (typeDict.TryGetValue(typeSearchValue, out doc))
                {
                    // write type
                    WriteMemberToXML(xtw,
                                     "T:" + type,
                                     doc.SelectSingleNode("doxygen/compounddef"));

                    ExportTypeMembers(xtw, type, GetMemberDict(doc));
                }
                else if (typeDict.TryGetValue(nativeNamespace, out doc))
                {
                    // export top-level types
                    var memberDict = GetMemberDict(doc);

                    if (!type.IsEnum)
                    {
                        List<XmlNode> nodeList;
                        if (memberDict.TryGetValue(typeSearchValue.ToUpperInvariant(), out nodeList))
                        {
                            XmlNode node = nodeList[0];
                            WriteMemberToXML(xtw,
                                             "T:" + type,
                                             node);
                        }
                        else
                        {
                            Log("Type not found: " + type);
                        }
                    }
                    else
                    {
                        ExportEnum(xtw, type, memberDict);
                    }
                }

            }
        }

        private static void ExportTypeMembers(XmlTextWriter xtw, Type type, Dictionary<string, List<XmlNode>> memberDict)
        {
            var members = type.GetMembers();
            foreach (MemberInfo member in members)
            {
                bool skip;
                bool findOverload = false;
                bool addSets = false;
                string[] memberSearchValues = new string[3];
                string name = string.Empty;

                switch (member.MemberType)
                {
                    case MemberTypes.Constructor:
                        {
                            var ci = (ConstructorInfo)member;
                            skip = !ci.IsPublic;
                            findOverload = true;
                            memberSearchValues[0] = type.Name.ToUpperInvariant();
                            name = "M:" + type + ".#ctor" + GetParameterString(ci.GetParameters());
                        }
                        break;

                    case MemberTypes.Method:
                        {
                            var mi = (MethodInfo)member;
                            skip = !mi.IsPublic || mi.IsSpecialName;
                            findOverload = true;
                            memberSearchValues[0] = mi.Name.ToUpperInvariant();
                            name = "M:" + type + "." + mi.Name + GetParameterString(mi.GetParameters());
                        }
                        break;

                    case MemberTypes.Property:
                        {
                            var pi = (PropertyInfo)member;
                            skip = !pi.CanRead;
                            memberSearchValues[0] = pi.Name.ToUpperInvariant();
                            memberSearchValues[1] = "GET" + pi.Name.ToUpperInvariant();
                            memberSearchValues[2] = "M" + pi.Name.ToUpperInvariant();
                            name = "P:" + type + "." + pi.Name;
                            addSets = pi.CanWrite;
                        }
                        break;

                    case MemberTypes.Field:
                        {
                            var fi = (FieldInfo)member;
                            skip = !fi.IsPublic;
                            memberSearchValues[0] = fi.Name.ToUpperInvariant();
                            name =  "F:" + type + "." + fi.Name;
                        }
                        break;

                    case MemberTypes.NestedType:
                        {
                            skip = true;
                            var nt = (Type)member;
                            if (nt.IsNestedPublic)
                            {
                                // handle nested enums
                                if (nt.IsEnum)
                                {
                                    ExportEnum(xtw, nt, memberDict);
                                }
                            }
                        }
                        break;

                    case MemberTypes.Event:
                        skip = true;
                        ExportEvent(xtw, type, (EventInfo)member);
                        break;

                    default:
                        skip = true;
                        Log("Ignored member: " + type + "." + member.Name);
                        break;
                }

                if (!(skip || IsIgnoredName(member.Name, member.MemberType)))
                {
                    var nodeList = new List<XmlNode>();

                    foreach (string memberSearchValue in memberSearchValues)
                    {
                        if (string.IsNullOrEmpty(memberSearchValue))
                        {
                            continue;
                        }

                        if (memberDict.TryGetValue(memberSearchValue, out nodeList))
                        {
                            break;
                        }
                    }

                    if (nodeList != null)
                    {
                        XmlNode node;
                        if (!findOverload || nodeList.Count == 1)
                        {
                            node = nodeList[0];
                        }
                        else
                        {
                            if (!TryGetOverload((MethodBase)member, nodeList, out node))
                            {
                                Log("Overload not found: " + name);
                            }
                        }

                        if (!WriteMemberToXML(xtw, member, name, node, addSets))
                        {
                            Log("Empty description: " + name);
                        }
                    }
                    else
                    {
                        if (member.DeclaringType == type)
                        {
                            Log("Member not found: " + name);
                        }
                        else
                        {
                            Log("Member (inherited) not found: " + name);
                        }
                    }
                }
            }
        }

        private static Dictionary<string, List<XmlNode>> GetMemberDict(XmlDocument doc)
        {
            var memberDict = new Dictionary<string, List<XmlNode>>();
            var importNode = doc.SelectSingleNode("doxygen/compounddef");
            if (importNode != null)
            {
                var importSectionNode = importNode.FirstChild;
                while (importSectionNode != null)
                {
                    if (importSectionNode.Name == "sectiondef")
                    {
                        var importMemberNode = importSectionNode.FirstChild;
                        while (importMemberNode != null)
                        {
                            var name = importMemberNode.SelectSingleNode("name").InnerText.ToUpperInvariant();
                            if (!memberDict.ContainsKey(name))
                            {
                                memberDict.Add(name, new List<XmlNode>{ importMemberNode });
                            }
                            else
                            {
                                memberDict[name].Add(importMemberNode);
                            }

                            importMemberNode = importMemberNode.NextSibling;
                        }
                    }

                    importSectionNode = importSectionNode.NextSibling;
                }
            }

            return memberDict;
        }

        private static string GetParameterString(ParameterInfo[] paras)
        {
            string paraName = string.Empty;
            foreach (ParameterInfo pi in paras)
            {
                paraName += pi.ParameterType.FullName;

                if (pi != paras[paras.Length - 1])
                {
                    paraName += ",";
                }
            }

            if (paras.Length > 0)
            {
                paraName = "(" + paraName + ")";
            }

            return paraName;
        }

        private static bool IsIgnoredName(string name, MemberTypes memberType)
        {
            bool retValue = false;

            switch (memberType)
            {
                case MemberTypes.Method:
                    switch (name)
                    {
                        case "Equals":
                        case "Dispose":
                        case "ToString":
                        case "GetHashCode":
                        case "GetType":
                        case "_Init_CLRObject":
                        case "DestroyNativePtr":
                            retValue = true;
                            break;
                    }
                    break;

                case MemberTypes.Property:
                    switch (name)
                    {
                        case "IsNull":
                        case "Target":
                        case "Unique":
                        case "UseCount":
                        case "_CLRHandle":
                        case "NativePtr":
                            retValue = true;
                            break;
                    }
                    break;
            }

            return retValue;
        }

        private static void Log(string message)
        {
            stringBuilder.AppendLine(message);
        }

        private static void ShowHelpAndQuit()
        {
            Console.WriteLine(
                @"
Usage: MogreXml <AssemblyPath> <NativeNamespace> <ManagedNamespace> [options]

Options:
-d:Directory                 Directory that contains the doxygen xml files [default is .\xml\].

Example: MogreXml .\Mogre.dll Ogre Mogre");

            Environment.Exit(0);
        }

        private static string[] SplitNativeParameterNames(string source)
        {
            // remove optional
            string val = Regex.Replace(source, @"=\w*", string.Empty);

            // remove some other crap
            val = Regex.Replace(val, @"(?![\s,])\W", string.Empty);

            // split parameters
            var split = val.Split(new[]{ ',' }, StringSplitOptions.RemoveEmptyEntries);

            // get parameter name
            for (int i = 0; i < split.Length; i++)
            {
                split[i] = Regex.Replace(split[i].Trim(), @".*\s", string.Empty);
            }

            return split;
        }

        private static bool TryGetOverload(MethodBase mb, List<XmlNode> nodes, out XmlNode retNode)
        {
            var parameters = mb.GetParameters();
            int mostParameters = 0;
            retNode = nodes[0];

            foreach (XmlNode node in nodes)
            {
                var nativeParas = SplitNativeParameterNames(node.SelectSingleNode("argsstring").InnerText);

                int matchCount = 0;
                
                for (int i = 0; i < parameters.Length && i < nativeParas.Length; i++)
                {
                    if (nativeParas[i].ToUpperInvariant() == parameters[i].Name.ToUpperInvariant())
                    {
                        matchCount++;
                    }
                }

                if (matchCount == parameters.Length)
                {
                    retNode = node;
                    return true;
                }

                // fallback to the one with the most matched parameters
                if (mostParameters < matchCount)
                {
                    mostParameters = matchCount;
                    retNode = node;
                }
            }

            return false;
        }

        private static bool WriteMemberToXML(XmlTextWriter xtw, MemberInfo mi, string name, XmlNode description, bool addSets)
        {
            if (description == null)
            {
                return false;
            }

            // prefer the brief description
            var descNode = description.SelectSingleNode("briefdescription/para") ?? description.SelectSingleNode("detaileddescription/para");

            if (descNode == null || string.IsNullOrEmpty(descNode.InnerText))
            {
                return false;
            }

            xtw.WriteStartElement("member");

            xtw.WriteAttributeString("name", name);

            XmlNode paraList = descNode.SelectSingleNode("parameterlist");

            // write summary
            var summary = descNode.InnerText;
            if (paraList != null && !string.IsNullOrEmpty(paraList.InnerText))
            {
                summary = summary.Replace(paraList.InnerText, string.Empty);
            }

            if (addSets)
            {
                summary = "Sets/" + summary;
            }

            xtw.WriteElementString("summary", summary);

            // write parametes
            MethodBase mb = mi as MethodBase;
            if (mb != null && paraList != null)
            {
                var paraNameList = new List<string>();
                foreach (ParameterInfo pi in mb.GetParameters())
                {
                    paraNameList.Add(pi.Name.ToUpperInvariant());
                }

                var paraItem = paraList.SelectSingleNode("parameteritem");
                while (paraItem != null)
                {
                    var paraName = paraItem.SelectSingleNode("parameternamelist/parametername");
                    if (paraName != null)
                    {
                        if (paraNameList.Contains(paraName.InnerText.ToUpperInvariant()))
                        {
                            xtw.WriteStartElement("param");
                            xtw.WriteAttributeString("name", paraName.InnerText);
                            xtw.WriteValue(paraItem.SelectSingleNode("parameterdescription").InnerText);
                            xtw.WriteEndElement();
                        }
                    }

                    paraItem = paraItem.NextSibling;
                }
            }

            xtw.WriteEndElement();

            return true;
        }

        private static void WriteMemberToXML(XmlTextWriter xtw, string name, XmlNode description)
        {
            name = name.Replace('+', '.');
            WriteMemberToXML(xtw, null, name, description, false);
        }

        #endregion Private Static Methods

        #endregion Methods
    }
}