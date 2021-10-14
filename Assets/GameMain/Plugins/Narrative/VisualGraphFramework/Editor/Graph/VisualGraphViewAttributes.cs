using System;

namespace VisualGraphEditor
{
    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomNodeViewAttribute : Attribute
    {
        public Type type;

        /// <summary>
        /// </summary>
        /// <param name="_name"></param>
        public CustomNodeViewAttribute(Type type)
        {
            this.type = type;
        }
    }

    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomPortViewAttribute : Attribute
    {
        public Type type;

        /// <summary>
        /// </summary>
        /// <param name="_name"></param>
        public CustomPortViewAttribute(Type type)
        {
            this.type = type;
        }
    }
}