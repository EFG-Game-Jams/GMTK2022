using Godot;
using System;

public class HUD : Control
{
    private int score = 0;
    private float distance = 0;

    [Export] public int Score {
        get => score;
        set {
            score = value;
            GetNode<Label>("Score").Text = $"Score\n{score}";
            //GD.Print(GetNode<Label>("Score").Text);
        }
    }
    [Export] public float Distance {
        get => distance;
        set {
            distance = value;
            GetNode<Label>("Distance").Text = $"Distance\n{(int)distance} m";
        }
    }
}
