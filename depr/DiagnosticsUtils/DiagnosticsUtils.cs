using DiagnosticsModule.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiagnosticsModule
{
    public static class DiagnosticsUtils
    {
        public static string CollectionTitle<T>(ICollection<T> col, string name)
        {
            return CollectionTitle(name, col.Count);
        }
        public static string CollectionTitle<T>(IReadOnlyCollection<T> col, string name)
        {
            return CollectionTitle(name, col.Count);
        }
        public static string CollectionTitle(string name, int count)
        {
            return name + "(" + count + ")" + (count != 0 ? ": " : "");
        }

        public static string FlatString<T>(string name, IReadOnlyCollection<T> col) where T : IDiagnosticDataObject
        {
            return CollectionFlatString(name, col, o => o.ToDiagnosticShortString());
        }

        public static string CollectionFlatString<T>(string name, IReadOnlyCollection<T> collection, Func<T, string> converter)
        {
            string output = CollectionTitle(collection, name);

            if (collection.Count != 0)
            {
                output += String.Join(", ", collection.Select(c => converter(c)));
            }

            return output;
        }

        public static string ListString<T>(string name, Dictionary<int, T> dic, bool newline = false) where T : IDiagnosticDataObject
        {
            return CollectionListString(name, dic, o => o.ToDiagnosticLongString()) + (newline ?"\n":"");
        }

        public static string ListInternalString<T>(string name, ICollection<T> dic, bool newline = false) where T : IDiagnosticDataObject
        {

            string output = CollectionTitle(name, dic.Count);
            if (dic.Count != 0)
            {
                output += "\n   ";
                while (true)
                {
                    try
                    {
                        output += String.Join("\n   ", dic.Select(p => p.ToDiagnosticLongString()));
                        break;
                    }
                    catch (System.InvalidOperationException)
                    {
                        break;
                    }
                }
            }

            return output + (newline ? "\n" : "");
        }

        public static string CollectionListString<K,T>(string name, Dictionary<K, T> dic, Func<T, string> converter)
        {
            string output = CollectionTitle(name, dic.Count);
            if (dic.Count != 0)
            {
                output += "\n   ";
                while (true)
                {
                    try
                    {
                        output += String.Join("\n   ", dic.Select(p => p.Key + " : " + converter(p.Value)));
                        break;
                    }
                    catch (System.InvalidOperationException)
                    {

                    }
                }
            }

            return output;
        }

        public static string Value(string v, object val, bool newline = false)
        {
            return v + ": " + val + (newline ?  "\n" : "");
        }
    }
}
