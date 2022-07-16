using Godot;

public class Ramp : Spatial
{
    [Export]
    public float Speed = 100;
    [Export]
    public float DespawnZThreshold = 10;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

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
