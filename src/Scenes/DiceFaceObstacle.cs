using Godot;
using System.Collections.Generic;
using System.Linq;

public class DiceFaceObstacle : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export]
    public float Speed = 100;
    [Export]
    public float DespawnZThreshold = 10;

    public bool IsNeutralized = false;

    [Export]
    public Vector3 SpawnLocation { get; set; } = Vector3.Zero;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetFaceArea().SetMeta(MetaNames.ColliderTag, ColliderTag.DieFace);

        foreach (var node in GetHoles())
        {
            node.GetNode("Area").SetMeta(MetaNames.ColliderTag, ColliderTag.Hole);
        }

        var newTransform = Transform;
        newTransform.origin = SpawnLocation;
        Transform = newTransform;
    }

    private Area GetFaceArea() => GetNode<Area>("Face/Area");

    private IEnumerable<DiceFaceObstacleBall> GetHoles() => GetChildren()
        .Cast<Node>()
        .Where(c => c.Name.StartsWith("BlackBall"))
        .Cast<DiceFaceObstacleBall>();

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Displace(Speed * delta);

        if (Transform.origin.z > DespawnZThreshold)
        {
            QueueFree();
        }
    }

    private void Displace(float z)
    {
        var newTransform = Transform;
        newTransform.origin = new Vector3(
            Transform.origin.x,
            Transform.origin.y,
            Transform.origin.z + z);
        Transform = newTransform;
    }
}
