using Godot;
using System;

public class Main : Spatial
{
    [Export]
    private float MinimumSpawnTime = 2f;
    [Export]
    private float MaximumSpawnTime = 5f;
    [Export]
    private float ObstacleMovementSpeed = 100f;
    [Export]
    private float ObstacleDespawnZ = 10f;

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private PackedScene obstacleScene = (PackedScene)GD.Load("res://Scenes/ProceduralDice.tscn");
    private PackedScene rampScene = (PackedScene)GD.Load("res://Scenes/Ramp.tscn");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Randomize();

        SetRandomSpawnObstacleTime();
    }

    public void OnObstacleSpawnTimerTimeout()
    {
        SpawnObstacleAndRamp();
        SetRandomSpawnObstacleTime();
    }

    private void SpawnObstacleAndRamp()
    {
        var spawnLocation = GetNode<ObstacleSpawnLocation>("ObstacleSpawnLocation");

        var obstacle = obstacleScene.Instance<CsgDiceBuilder>();
        var obstacleTransform = obstacle.Transform;
        obstacleTransform.origin = spawnLocation.Transform.origin;
        obstacle.Transform = obstacleTransform;
        obstacle.DieVariant = (int)(GD.Randi() % 6) + 1;
        obstacle.MovementSpeed = ObstacleMovementSpeed;
        obstacle.DespawnZThreshold = ObstacleDespawnZ;
        AddChild(obstacle);

        var ramp = rampScene.Instance<Ramp>();
        var rampTransform = ramp.Transform;
        rampTransform.origin = new Vector3(
            spawnLocation.Transform.origin.x,
            0,
            spawnLocation.Transform.origin.z + spawnLocation.RampLead);
        ramp.Transform = rampTransform;
        ramp.MovementSpeed = ObstacleMovementSpeed;
        ramp.DespawnZThreshold = ObstacleDespawnZ;
        AddChild(ramp);
    }

    private void SetRandomSpawnObstacleTime()
    {
        var timer = GetNode<Timer>("ObstacleSpawnTimer");
        timer.Start(GetRandomWaitTime());
    }

    //
    private float GetRandomWaitTime()
    {
        var range = MaximumSpawnTime - MinimumSpawnTime;
        return MinimumSpawnTime + (GD.Randf() * range);
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
