using Godot;
using System;

[Tool]
public class CsgDiceBuilder : Spatial
{
    [Export] private float border = 0;
    [Export] private float spacing = 0;
    [Export] private float holeZ = 0;
    [Export] private float holeRadius = 0;
    [Export] private int holeSides = 0;
    [Export] private float holeHeight = 0;

    [Export] private int dieVariant = 0;
    public int DieVariant { get => dieVariant; set => dieVariant = value; }

    [Export] public float MovementSpeed { get; set; } = 100f;
    [Export] public float DespawnZThreshold = 10f;

    public bool IsNeutralized = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var children = GetChildren();
        if (children.Count != 8)
            return;

        // rebuild CSG (editor only)
        if (Engine.EditorHint)
        {
            ProcessForEditor();
        }
        else
        {
            ProcessForGame(delta);
        }

        // update visibility based on selected variant
        for (int i = 1; i < children.Count; ++i)
        {
            if (children[i] is Spatial node)
                node.Visible = (dieVariant < 0 || (i - 1) == dieVariant);
        }
    }

    private void ProcessForEditor()
    {
        var children = GetChildren();
        for (int i = 1; i < children.Count; ++i)
        {
            if (children[i] is CSGMesh csgMesh)
                RebuildDie(csgMesh, i - 1);
        }

        // update die collider
        Area area = GetChild(0) as Area;
        area.Monitoring = false;
        area.SetMeta(MetaNames.ColliderTag, ColliderTag.DieFace);

        // // update hole collider
        CollisionShape shape = GetChild(1).GetChild(0).GetChild(0).GetChild(0) as CollisionShape;
        CylinderShape cylinder = shape.Shape as CylinderShape;
        cylinder.Radius = holeRadius;
        cylinder.Height = holeHeight;
    }

    private void ProcessForGame(float delta)
    {
        // Update movement
        Displace(MovementSpeed * delta);

        if (Transform.origin.z > DespawnZThreshold)
        {
            QueueFree();
        }
    }

    private void RebuildDie(Node root, int die)
    {
        if (root.GetChildCount() != 7)
            return;

        for (int i = 0; i < root.GetChildCount(); ++i)
        {
            CSGCylinder child = root.GetChild(i) as CSGCylinder;

            child.Visible = HoleVisible(die, i);

            Vector3 pos = GetHolePosition(i);
            Transform tsf = child.Transform;
            if ((tsf.origin - pos).Length() > .00001f)
            {
                //GD.Print(tsf.origin, pos);
                tsf.origin = pos;
                child.Transform = tsf;
            }

            if (holeRadius != child.Radius)
                child.Radius = holeRadius;

            if (holeSides != child.Sides)
                child.Sides = holeSides;

            Area trigger = child.GetChild(0) as Area;
            trigger.Monitoring = false;
            trigger.SetMeta(MetaNames.ColliderTag, ColliderTag.Hole);
        }
    }

    private bool HoleVisible(int die, int hole)
    {
        var dieHoles2D = new byte[7, 7] {
            {0,0,0,0,0,0,0},
            {0,0,0,1,0,0,0},
            {1,0,0,0,0,0,1},
            {0,0,1,1,1,0,0},
            {1,0,1,0,1,0,1},
            {1,0,1,1,1,0,1},
            {1,1,1,0,1,1,1},
        };
        // seems godot breaks multidim arrays? what...
        // works fine if we flatten it, so this dirty mess will do
        /*dieHoles = new byte[7*7];            
        for (int i=0; i<7; ++i)
            for (int j=0; j<7; ++j)
                dieHoles[i*7+j] = dieHoles2D[i,j];*/

        /*GD.Print("indices:", die, hole);
        GD.Print(dieHoles);
        GD.Print(dieHoles.Length);*/
        //return dieHoles[die*7 + hole] != 0;
        return dieHoles2D[die, hole] != 0;
    }

    private Vector3 GetHolePosition(int hole)
    {
        Vector3 pos = new Vector3(border, border, holeZ);
        switch (hole)
        {
            case 3:
                pos.x += spacing;
                break;
            case 4:
            case 5:
            case 6:
                pos.x += spacing * 2;
                break;
        }
        switch (hole)
        {
            case 1:
            case 3:
            case 5:
                pos.y += spacing;
                break;
            case 2:
            case 6:
                pos.y += spacing * 2;
                break;
        }
        return pos;
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