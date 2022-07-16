using Godot;
using System.Collections.Generic;
using System.Linq;

public class Main : Spatial
{
    [Export] private float MinimumLeadDistance = 100f;
    [Export] private float MaximumLeadDistance = 200f;
    [Export] private float MinimumGapDistance = 10f;
    [Export] private float MaximumGapDistance = 20f;
    [Export] private float MinimumSegmentsLength = 800f;
    [Export] private float MinimumMovementSpeed = 80f;

    private float movementSpeed = 0f;
    private float MovementSpeed
    {
        get => movementSpeed;
        set
        {
            movementSpeed = value;
            foreach (var segment in segments)
            {
                segment.MovementSpeed = movementSpeed;
            }
        }
    }

    private bool playerAlive = true;

    private PackedScene worldSegmentScene = (PackedScene)GD.Load("res://Scenes/WorldSegment.tscn");
    private List<WorldSegment> segments = new List<WorldSegment>();
    private Timer playerRespawnTimer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Randomize();
        playerRespawnTimer = GetNode<Timer>("PlayerDeathTimer");

        movementSpeed = MinimumMovementSpeed;
        EnsureSegmentsExist();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        CleanupOldSegments();
        EnsureSegmentsExist();

        // TODO increment movement speed over time?
        if (playerAlive)
        {
            GetNode<HUD>("HUD").Distance += MovementSpeed * delta;
        }
        else
        {
            MovementSpeed = movementSpeed * 0.6f;
        }
    }

    private void CleanupOldSegments()
    {
        while (segments.Count > 0 && segments.First().GetTailPosition().z > 100)
        {
            segments[0].QueueFree();
            segments.RemoveAt(0);
        }
    }

    private void EnsureSegmentsExist()
    {
        while (segments.Count < 1 || Mathf.Abs(segments.Last().GetTailPosition().z) < MinimumSegmentsLength)
        {
            AddSegment();
        }
    }

    private void AddSegment()
    {
        var segment = worldSegmentScene.Instance<WorldSegment>();
        if (segments.Count < 1)
        {
            segment.SpawnPosition = new Vector3(0, 0, 10f);
        }
        else
        {
            segment.SpawnPosition = segments.Last().GetTailPosition();
        }
        // TODO fill the other variables with procedurally harder values
        segment.LeadDistance = MinimumLeadDistance + (GD.Randf() * (MaximumLeadDistance - MinimumLeadDistance));
        segment.GapDistance = MinimumGapDistance + (GD.Randf() * (MaximumGapDistance - MinimumGapDistance));
        segment.MovementSpeed = MovementSpeed;

        segments.Add(segment);
        AddChild(segment);
    }

    public void OnPlayerDied()
    {
        MovementSpeed = 0f;
        playerRespawnTimer.Start(2f);
    }

    public void OnPlayerRespawn()
    {
        GetTree().ReloadCurrentScene();
    }
}
