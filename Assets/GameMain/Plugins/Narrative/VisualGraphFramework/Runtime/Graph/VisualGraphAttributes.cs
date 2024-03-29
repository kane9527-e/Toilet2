﻿///-------------------------------------------------------------------------------------------------
// author: William Barry
// date: 2020
// Copyright (c) Bus Stop Studios.
///-------------------------------------------------------------------------------------------------

using System;

// ReSharper disable once CheckNamespace
namespace VisualGraphRuntime
{
    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeNameAttribute : Attribute
    {
        public string name;

        /// <summary>
        /// </summary>
        /// <param name="_name"></param>
        public NodeNameAttribute(string _name)
        {
            name = _name;
        }
    }

    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultNodeTypeAttribute : Attribute
    {
        public Type type;

        /// <summary>
        /// </summary>
        /// <param name="_name"></param>
        public DefaultNodeTypeAttribute(Type _type)
        {
            type = _type;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultPortTypeAttribute : Attribute
    {
        public Type type;

        /// <summary>
        /// </summary>
        /// <param name="_name"></param>
        public DefaultPortTypeAttribute(Type _type)
        {
            type = _type;
        }
    }

    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NodePortAggregateAttribute : Attribute
    {
        public enum PortAggregate
        {
            None,
            Single,
            Multiple
        }

        public PortAggregate InputPortAggregate = PortAggregate.Single;
        public PortAggregate OutputPortAggregate = PortAggregate.Multiple;

        /// <summary>
        /// </summary>
        /// <param name="_name"></param>
        public NodePortAggregateAttribute(PortAggregate InputPortDynamics = PortAggregate.Single,
            PortAggregate OutputPortDynamics = PortAggregate.Multiple)
        {
            InputPortAggregate = InputPortDynamics;
            OutputPortAggregate = OutputPortDynamics;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class PortCapacityAttribute : Attribute
    {
        //
        // Summary:
        //     Specify how many edges a port can have connected.
        public enum Capacity
        {
            //
            // Summary:
            //     Port can only have a single connection.
            Single = 0,

            //
            // Summary:
            //     Port can have multiple connections.
            Multi = 1
        }

        public Capacity InputPortCapacity = Capacity.Multi;
        public Capacity OutputPortCapacity = Capacity.Single;

        /// <summary>
        /// </summary>
        /// <param name="_name"></param>
        public PortCapacityAttribute(Capacity InputPortCapacity = Capacity.Multi,
            Capacity OutputPortCapacity = Capacity.Single)
        {
            this.InputPortCapacity = InputPortCapacity;
            this.OutputPortCapacity = OutputPortCapacity;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class GraphOrientationAttribute : Attribute
    {
        //
        // Summary:
        //     Graph element orientation.
        public enum Orientation
        {
            // Summary:
            //     Horizontal orientation used for nodes and connections flowing to the left or
            //     right.
            Horizontal = 0,

            //
            // Summary:
            //     Vertical orientation used for nodes and connections flowing up or down.
            Vertical = 1
        }

        public Orientation GrapOrientation = Orientation.Horizontal;

        /// <summary>
        /// </summary>
        /// <param name="_name"></param>
        public GraphOrientationAttribute(Orientation GrapOrientation = Orientation.Horizontal)
        {
            this.GrapOrientation = GrapOrientation;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class CustomNodeStyleAttribute : Attribute
    {
        public string style;

        /// <summary>
        /// </summary>
        /// <param name="_name"></param>
        public CustomNodeStyleAttribute(string style)
        {
            this.style = style;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class CanNotConnectSameNodeAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class PortTypeAttribute : Attribute
    {
        public Type InputType;
        public Type OutputType;

        /// <summary>
        /// </summary>
        /// <param name="_name"></param>
        public PortTypeAttribute(Type InputType, Type OutputType)
        {
            this.InputType = InputType;
            this.OutputType = OutputType;
        }
    }
}