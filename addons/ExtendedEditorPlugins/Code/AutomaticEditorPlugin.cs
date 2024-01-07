#if TOOLS
using System;
using System.Collections.Generic;
using Godot;

namespace ExtendedEditorPlugins; 

[Tool]
public partial class AutomaticEditorPlugin : EditorPlugin {
    private readonly List<string> _registeredCustomTypes = new(32);

    protected virtual void EnterTree() {}
    protected virtual void ExitTree() {}
    
    public sealed override void _EnterTree() {
        base._EnterTree();
        _registeredCustomTypes.Clear();
        RegisterCustomNodesWithAttribute();
        EnterTree();
    }

    public sealed override void _ExitTree() {
        base._ExitTree();
        ExitTree();
        ClearRegisteredCustomTypes();
    }

    public void AddCustomType<T>(Texture2D icon = null) where T : Node {
        AddCustomType(typeof(T), icon);
    }

    public void AddCustomType(Type type, Texture2D icon = null) {
        if (type == null) {
            GD.PrintErr($"Type is null");
            return;
        }
        
        var baseType = type.BaseType;

        if (baseType == null) {
            GD.PrintErr($"Base type is null {type.Name}");
            return;
        }
        
        var pathAttribute = GetPathAttribute(type);
        if (pathAttribute == null) {
            GD.PrintErr($"Path attribute is not defined for {type.Name}");
            return;
        }

        var script = GD.Load<Script>(pathAttribute.Path);

        AddCustomType(type.Name, baseType.Name, script, icon);
    }

    public new void AddCustomType(string type, string @base, Script script, Texture2D icon) {
        base.AddCustomType(type, @base, script, icon);
        _registeredCustomTypes.Add(type);
    }

    private ScriptPathAttribute GetPathAttribute(Type type) {
        ScriptPathAttribute pathAttribute = null;
        var attributes = type.GetCustomAttributes(typeof(ScriptPathAttribute), false);
        foreach (var attribute in attributes) {
            if (attribute is ScriptPathAttribute pathAttributeTyped) {
                pathAttribute = pathAttributeTyped;
                break;
            }
        }

        return pathAttribute;
    }
    
    private void ClearRegisteredCustomTypes() {
        foreach (var typeName in _registeredCustomTypes) {
            RemoveCustomType(typeName);
        }
        _registeredCustomTypes.Clear();
    }

    private void RegisterCustomNodesWithAttribute() {
        var registerAttributes = new List<RegisterCustomNodeAttribute>(256);
        
        var assembly = GetType().Assembly;
        foreach (var type in assembly.GetTypes()) {
            var attributes = type.GetCustomAttributes(typeof(RegisterCustomNodeAttribute), false);
            
            RegisterCustomNodeAttribute registerAttribute = null;
            foreach (var attribute in attributes) {
                if (attribute is RegisterCustomNodeAttribute attributeTyped && attributeTyped.IsValid()) {
                    registerAttribute = attributeTyped;
                    break;
                }
            }

            if (registerAttribute != null) {
                registerAttributes.Add(registerAttribute);
            }
        }

        foreach (var registerAttribute in registerAttributes) {
            AddCustomType(registerAttribute.NodeType);
        }
    }
    
}
#endif