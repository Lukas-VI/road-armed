using Godot;

namespace RoadArmed.Game.Character.Animation;

public partial class UpperBodyAimController : Node
{
    [Export]
    public NodePath SearchRootPath { get; set; } = new("../VisualRoot/Model");

    [Export]
    public float ChestPitchWeight { get; set; } = 0.45f;

    [Export]
    public float ChestYawWeight { get; set; } = 0.35f;

    [Export]
    public float ArmPitchWeight { get; set; } = 0.6f;

    [Export]
    public float ArmYawWeight { get; set; } = 0.2f;

    [Export]
    public float MaxPitchRadians { get; set; } = 0.75f;

    [Export]
    public float MaxYawRadians { get; set; } = 0.65f;

    private Components.CharacterRuntimeContext? _runtime;
    private Skeleton3D? _skeleton;
    private int _chestBone = -1;
    private int _leftUpperArm = -1;
    private int _rightUpperArm = -1;

    public string DebugName => nameof(UpperBodyAimController);

    public void Setup(Components.CharacterRuntimeContext runtime)
    {
        _runtime = runtime;
        _runtime.ActiveAimController = DebugName;

        var root = !SearchRootPath.IsEmpty ? GetNodeOrNull<Node>(SearchRootPath) : null;
        if (root == null)
        {
            return;
        }

        _skeleton = FindSkeleton(root);
        if (_skeleton == null)
        {
            return;
        }

        RefreshBones();
    }

    public override void _Process(double delta)
    {
        if (_runtime == null || _skeleton == null)
        {
            return;
        }

        if (!_runtime.IsAiming && !_runtime.IsFiring)
        {
            return;
        }

        float relativeYaw = WrapAngle(_runtime.ViewYaw - _runtime.BodyYaw);
        float clampedYaw = Mathf.Clamp(relativeYaw, -MaxYawRadians, MaxYawRadians);
        float clampedPitch = Mathf.Clamp(-_runtime.ViewPitch, -MaxPitchRadians, MaxPitchRadians);

        ApplyBoneOffset(_chestBone, clampedPitch * ChestPitchWeight, clampedYaw * ChestYawWeight, false);
        ApplyBoneOffset(_leftUpperArm, clampedPitch * ArmPitchWeight, clampedYaw * ArmYawWeight, true);
        ApplyBoneOffset(_rightUpperArm, clampedPitch * ArmPitchWeight, clampedYaw * ArmYawWeight, false);
    }

    private void ApplyBoneOffset(int boneIndex, float pitch, float yaw, bool isLeft)
    {
        if (_skeleton == null || boneIndex < 0)
        {
            return;
        }

        float sideSign = isLeft ? -1f : 1f;
        var rest = _skeleton.GetBonePoseRotation(boneIndex);
        var yawRot = new Quaternion(Vector3.Up, yaw * sideSign);
        var pitchRot = new Quaternion(Vector3.Right, pitch);
        _skeleton.SetBonePoseRotation(boneIndex, rest * yawRot * pitchRot);
    }

    private void RefreshBones()
    {
        if (_skeleton == null)
        {
            return;
        }

        _chestBone = FindBone(_skeleton, "spine.003", "spine.004", "spine.005", "chest", "upperchest");
        _leftUpperArm = FindBone(_skeleton, "upper_arm.L", "leftupperarm");
        _rightUpperArm = FindBone(_skeleton, "upper_arm.R", "rightupperarm");
    }

    private static int FindBone(Skeleton3D skeleton, params string[] names)
    {
        foreach (string name in names)
        {
            int exact = skeleton.FindBone(name);
            if (exact >= 0)
            {
                return exact;
            }
        }

        for (int i = 0; i < skeleton.GetBoneCount(); i++)
        {
            string current = skeleton.GetBoneName(i).ToString().ToLowerInvariant();
            foreach (string candidate in names)
            {
                if (current.Contains(candidate.ToLowerInvariant()))
                {
                    return i;
                }
            }
        }

        return -1;
    }

    private static Skeleton3D? FindSkeleton(Node root)
    {
        foreach (Node child in root.GetChildren())
        {
            if (child is Skeleton3D skeleton)
            {
                return skeleton;
            }

            var nested = FindSkeleton(child);
            if (nested != null)
            {
                return nested;
            }
        }

        return null;
    }

    private static float WrapAngle(float radians)
    {
        while (radians > Mathf.Pi) radians -= Mathf.Tau;
        while (radians < -Mathf.Pi) radians += Mathf.Tau;
        return radians;
    }
}
