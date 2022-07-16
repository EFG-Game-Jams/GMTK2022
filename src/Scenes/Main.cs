using Godot;
using System;

public class Main : Spatial
{
    [Export]
    private float MinimumSpawnTime = 2f;
    [Export]
    private float MaximumSpawnTime = 5f;

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private PackedScene obstacleScene = (PackedScene)GD.Load("res://Scenes/DiceFaceObstacle.tscn");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Randomize();

        SetRandomSpawnObstacleTime();
    }

    public void OnObstacleSpawnTimerTimeout()
    {
        var obstacle = obstacleScene.InstanceOrNull<DiceFaceObstacle>();
        obstacle.SpawnLocation = GetNode<Position3D>("ObstacleSpawnLocation").Transform.origin;
        AddChild(obstacle);

        SetRandomSpawnObstacleTime();
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
