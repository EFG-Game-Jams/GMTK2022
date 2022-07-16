using Godot;
using System;

public class DiceFaceObstacle : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export]
    public float Speed = 100;
    [Export]
    public float DespawnZThreshold = 10;

    [Export]
    public Vector3 SpawnLocation { get; set; } = Vector3.Zero;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var newTransform = Transform;
        newTransform.origin = SpawnLocation;
        Transform = newTransform;
    }

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
