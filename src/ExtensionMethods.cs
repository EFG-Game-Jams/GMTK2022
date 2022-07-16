public static class ExtensionMethods
{
    public static void SetOrigin(this Godot.Spatial node, float x, float y, float z)
    {
        SetOrigin(node, new Godot.Vector3(x, y, z));
    }
    public static void SetOrigin(this Godot.Spatial node, Godot.Vector3 origin)
    {
        var newTransform = node.Transform;
        newTransform.origin = origin;
        node.Transform = newTransform;
    }

    public static void MoveOrigin(this Godot.Spatial node, float x, float y, float z)
    {
        MoveOrigin(node, new Godot.Vector3(x, y, z));
    }
    public static void MoveOrigin(this Godot.Spatial node, Godot.Vector3 displacement)
    {
        var newTransform = node.Transform;
        newTransform.origin += displacement;
        node.Transform = newTransform;
    }
}