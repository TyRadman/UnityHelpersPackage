using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class MethodButtonAttribute : Attribute
{
    public readonly string Label;

    public MethodButtonAttribute()
    {

    }

    public MethodButtonAttribute(string label)
    {
        Label = label;
    }
}