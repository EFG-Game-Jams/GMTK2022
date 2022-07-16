using Godot;
using System;

public class Player : Spatial
{
    private bool isAlive = true;
    private bool isFlying = false;
    private bool isGliding = false;
    private bool isGlidingAllowed = true;
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
    private float laneSwitchingSpeed = 4f;
    [Export]
    private float radius = 1f;

    private float currentJumpVelocity = 0;

    private Transform originalTransform;

    [Signal]
    public delegate void PlayerDied();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        originalTransform = Transform;
        Scale = new Vector3(radius, radius, radius);
    }

    // TODO Handle mobile user input? Tap to jump, swipe to switch lane?
    public override void _Input(InputEvent inputEvent)
    {
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

    public void Reset()
    {
        isAlive = true;
        isFlying = false;
        isGliding = false;
        isGlidingAllowed = true;
        currentJumpVelocity = 0f;
        currentLane = Lane.Middle;
        Transform = originalTransform;
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
        currentLane = lane;
    }

    public override void _PhysicsProcess(float delta)
    {
        if (Transform.origin.y < 0)
        {
            OnPlayerDeath();
        }

        if (isFlying)
        {
            if (Input.IsActionPressed("glide") && currentJumpVelocity <= glideSpeed && isGlidingAllowed)
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

        float xOffset = currentLane == Lane.Left ? -laneXOffset : (currentLane == Lane.Right ? laneXOffset : 0f);
        var updatedOrigin = Transform.origin.LinearInterpolate(
            new Vector3(
                originalTransform.origin.x + xOffset,
                Transform.origin.y,
                Transform.origin.z),
            laneSwitchingSpeed * delta);
        var newTransform = Transform;
        newTransform.origin = updatedOrigin;
        Transform = newTransform;
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

    public void OnCollisionEntered(Area other)
    {
        var tag = (ColliderTag)other.GetMeta(MetaNames.ColliderTag);
        switch (tag)
        {
            case ColliderTag.DieFace:
                if (!other.GetParent<CsgDiceBuilder>().IsNeutralized)
                {
                    this.SetOrigin(Transform.origin.x, Transform.origin.y, other.GlobalTransform.origin.z + radius + radius);
                    OnPlayerDeath();
                }
                break;

            case ColliderTag.Hole:
                {
                    isGlidingAllowed = false;
                    var parent = other.GetParent().GetParent().GetParent<CsgDiceBuilder>();
                    if (!parent.IsNeutralized)
                    {
                        var yError = other.GlobalTransform.origin.y - Transform.origin.y;
                        currentJumpVelocity = yError * 4;
                        OnHoleCollision(parent);
                    }
                }
                break;

            case ColliderTag.Ramp:
                Jump();
                break;

            case ColliderTag.Ground:
                OnGroundCollision();
                break;

            default:
                GD.PrintErr($"Unexpected {nameof(ColliderTag)} value {tag}");
                break;
        }
    }

    private void OnGroundCollision()
    {
        isFlying = false;
        isGlidingAllowed = true;

        var newTransform = Transform;
        newTransform.origin = new Vector3(
            Transform.origin.x,
            originalTransform.origin.y,
            originalTransform.origin.z);
        Transform = newTransform;
    }

    private void OnPlayerDeath()
    {
        if (!isAlive)
        {
            return;
        }

        isAlive = false;
        EmitSignal(nameof(PlayerDied));
    }

    private void OnHoleCollision(CsgDiceBuilder obstacle)
    {
        obstacle.IsNeutralized = true;
    }

    enum Lane
    {
        Left,
        Middle,
        Right
    }
}
