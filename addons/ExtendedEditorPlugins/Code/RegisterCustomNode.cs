using System;
using Godot;

namespace ExtendedEditorPlugins;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
sealed class RegisterCustomNodeAttribute : Attribute {
    public readonly Type NodeType;

    public RegisterCustomNodeAttribute(Type nodeType) {
        NodeType = nodeType;
    }

    public bool IsValid() => NodeType.IsAssignableTo(typeof(Node));
}
