using Godot;
using System;

public class Player : Spatial
{
    private bool isFlying = false;
    private bool isGliding = false;
    private Lane lane = Lane.Middle;

    [Export]
    private float jumpSpeed = 100;
    [Export]
    private float jumpDecay = 50;
    [Export]
    private float glideSpeed = -15;
    private float currentJumpVelocity = 0;

    private Transform originalTransform;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        originalTransform = Transform;
    }

    // TODO Handle mobile user input? Tap to jump, swipe to switch lane?
    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent.IsActionPressed("jump"))
        {
            Jump();
        }
        if (inputEvent.IsActionPressed("left"))
        {
            TryMoveLeft();
        }
        if (inputEvent.IsActionPressed("right"))
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

        switch (lane)
        {
            case Lane.Middle:
                lane = Lane.Left;
                break;

            case Lane.Right:
                lane = Lane.Middle;
                break;
        }
    }

    private void TryMoveRight()
    {
        if (!CanSwitchLane())
        {
            return;
        }

        switch (lane)
        {
            case Lane.Left:
                lane = Lane.Middle;
                break;

            case Lane.Middle:
                lane = Lane.Right;
                break;
        }
    }

    // TODO Does the game currently allow switching lanes? (jumping phase
    // disallowes lane switching)
    private bool CanSwitchLane()
    {
        return true;
    }

    public void OnGroundCollision(Area ground)
    {
        isFlying = false;
        Transform = originalTransform;
        // TODO
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (isFlying)
        {
            if (Input.IsActionPressed("glide") && currentJumpVelocity <= glideSpeed)
            {
                Displace(glideSpeed * delta);
            }
            else
            {
                currentJumpVelocity -= jumpDecay * delta;
                Displace(delta * currentJumpVelocity);
            }
        }
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


    enum Lane
    {
        Left,
        Middle,
        Right
    }
}
