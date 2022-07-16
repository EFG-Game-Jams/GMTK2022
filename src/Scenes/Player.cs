using Godot;
using System;

public class Player : Spatial
{
    private bool isFlying = false;
    private bool isGliding = false;
    private Lane currentLane = Lane.Middle;

    [Export]
    private float jumpSpeed = 100;
    [Export]
    private float jumpDecay = 50;
    [Export]
    private float glideSpeed = -15;
    [Export]
    private float laneXOffset = 3.2f;
    [Export]
    private float radius = 1f;

    private float currentJumpVelocity = 0;

    private Transform originalTransform;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        originalTransform = Transform;
        Scale = new Vector3(radius, radius, radius);
    }

    // TODO Handle mobile user input? Tap to jump, swipe to switch lane?
    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent.IsActionPressed("jump"))
        {
            Jump();
        }
        if (inputEvent.IsActionReleased("left"))
        {
            TryMoveLeft();
        }
        if (inputEvent.IsActionReleased("right"))
        {
            TryMoveRight();
        }
        if (inputEvent.IsActionPressed("reset"))
        {
            isFlying = false;
            isGliding = false;
            currentJumpVelocity = 0f;
            Transform = originalTransform;
        }
    }

    private void Jump()
    {
        if (isFlying)
        {
            return;
        }

        currentJumpVelocity = jumpSpeed;
        isFlying = true;
    }

    private void TryMoveLeft()
    {
        if (!CanSwitchLane())
        {
            return;
        }

        switch (currentLane)
        {
            case Lane.Middle:
                SwitchToLane(Lane.Left);
                break;

            case Lane.Right:
                SwitchToLane(Lane.Middle);
                break;
        }
    }

    private void TryMoveRight()
    {
        if (!CanSwitchLane())
        {
            return;
        }

        switch (currentLane)
        {
            case Lane.Left:
                SwitchToLane(Lane.Middle);
                break;

            case Lane.Middle:
                SwitchToLane(Lane.Right);
                break;
        }
    }

    // TODO Does the game currently allow switching lanes? (jumping phase
    // disallowes lane switching)
    private bool CanSwitchLane()
    {
        return !isFlying;
    }

    private void SwitchToLane(Lane lane)
    {
        float x = lane == Lane.Left ? -laneXOffset : (lane == Lane.Right ? laneXOffset : 0f);
        var newTransform = Transform;
        newTransform.origin = new Vector3(
            originalTransform.origin.x + x,
            Transform.origin.y,
            Transform.origin.z);
        Transform = newTransform;

        currentLane = lane;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (Transform.origin.y < radius)
        {
            OnGroundCollision();
        }

        if (isFlying)
        {
            if (Input.IsActionPressed("glide") && currentJumpVelocity <= glideSpeed)
            {
                isGliding = true;
                Displace(glideSpeed * delta);
            }
            else
            {
                isGliding = false;
                currentJumpVelocity -= jumpDecay * delta;
                Displace(delta * currentJumpVelocity);
            }
        }
    }

    private void OnGroundCollision()
    {
        isFlying = false;

        var newTransform = Transform;
        newTransform.origin = new Vector3(
            Transform.origin.x,
            originalTransform.origin.y,
            originalTransform.origin.z);
        Transform = newTransform;
        // TODO
    }

    private void Displace(float y)
    {
        var newTransform = Transform;
        newTransform.origin = new Vector3(
            Transform.origin.x,
            Transform.origin.y + y,
            Transform.origin.z);
        Transform = newTransform;
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
    }

    public void OnCollisionEntered(Area other)
    {
        var tag = (ColliderTag)other.GetMeta(MetaNames.ColliderTag);
        GD.Print(tag);
    }

    enum Lane
    {
        Left,
        Middle,
        Right
    }
}
