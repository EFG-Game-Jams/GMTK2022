using Godot;
using System;

public class WorldSegment : Spatial
{
    [Export]
    public float LeadDistance = 100f;
    [Export]
    public float GapDistance = 20f;
    [Export]
    public float MovementSpeed = 100f;

    public Vector3 SpawnPosition { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.SetOrigin(SpawnPosition);

        var ramp = GetNode<Ramp>("Ramp");
        ramp.SetOrigin(0, 0, -LeadDistance);

        var obstacle = GetNode<CsgDiceBuilder>("Obstacle");
        obstacle.SetOrigin(new Vector3(0, 6f, -TotalLength));
        obstacle.DieVariant = (int)(GD.Randi() % 6) + 1;

        var groundScale = LeadDistance / 2f;
        var ground = GetNode<MeshInstance>("Ground");
        ground.SetOrigin(0, -1, -groundScale);
        ground.Scale = new Vector3(6, 1, groundScale);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        this.MoveOrigin(0, 0, MovementSpeed * delta);
    }

    public Vector3 GetTailPosition() => Transform.origin + new Vector3(0, 0, -TotalLength);
    public float TotalLength { get => LeadDistance + GapDistance; }
}
