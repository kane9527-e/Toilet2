using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace TextVariable
{
    public static class TextVariableProcessor
    {
        private static List<TextVariable> _variables;

        public static Assembly AssemblyEditor => Assembly.Load("TextVariable");

        /// <summary>
        ///     获取所有指定的反射类实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetAllReflectionClassIns<T>() where T : class
        {
            var reference = new List<T>();
            var types = AssemblyEditor.GetTypes();
            foreach (var type in types)
                if (type.BaseType == typeof(T) && type != typeof(T))
                {
                    var ins = Activator.CreateInstance(type) as T;
                    reference.Add(ins);
                }

            return reference;
        }

        public static string ProcessVariable(string allText)
        {
            if (_variables == null)
                _variables = GetAllReflectionClassIns<TextVariable>();

            var tokens = GetTokens(allText);

            foreach (var token in tokens)
            foreach (var v in _variables)
                if (v.Detect(token))
                {
                    allText = allText.Replace(string.Format("{{{0}}}", token), v.Process(token));
                    break;
                }

            return allText;
        }

        private static List<string> GetTokens(string str)
        {
            var regex = new Regex(@"(?<=\{)[^}]*(?=\})", RegexOptions.IgnoreCase);
            var matches = regex.Matches(str);

            // Results include braces (undesirable)
            return matches.Cast<Match>().Select(m => m.Value).Distinct().ToList();
        }
    }
}