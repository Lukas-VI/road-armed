using Godot;
using RoadArmed.Game.Input;
using RoadArmed.Game.Shared;

namespace RoadArmed.Game.Character.Components;

public partial class CharacterAnimationDriver : Node, ICharacterAnimationDriver
{
    [Export]
    public bool UseLayeredAnimationTree { get; set; } = true;

    [Export]
    public NodePath SearchRootPath { get; set; } = new("../VisualRoot/Model");

    [Export]
    public NodePath AnimationTreePath { get; set; } = new();

    [Export]
    public NodePath AnimationPlayerPath { get; set; } = new();

    [Export]
    public string IdleAnimation { get; set; } = AssetNaming.AnimationKeys.IdleUnarmed;

    [Export]
    public string WalkAnimation { get; set; } = AssetNaming.AnimationKeys.WalkForward;

    [Export]
    public string RunAnimation { get; set; } = AssetNaming.AnimationKeys.RunForward;

    [Export]
    public string ReloadAnimation { get; set; } = AssetNaming.AnimationKeys.ReloadRifle;

    [Export]
    public string FireAnimation { get; set; } = AssetNaming.AnimationKeys.FireRifle;

    private AnimationTree? _animationTree;
    private AnimationPlayer? _animationPlayer;
    private string _idleClip = string.Empty;
    private string _walkClip = string.Empty;
    private string _runClip = string.Empty;
    private string _reloadClip = string.Empty;
    private string _fireClip = string.Empty;
    private bool _treeReady;
    private bool _lastFiring;
    private bool _lastReloading;
    private string _lastBaseClip = string.Empty;

    public string DebugName => nameof(CharacterAnimationDriver);

    public void Setup(CharacterRuntimeContext runtime)
    {
        _animationTree = !AnimationTreePath.IsEmpty ? GetNodeOrNull<AnimationTree>(AnimationTreePath) : null;
        _animationPlayer = !AnimationPlayerPath.IsEmpty ? GetNodeOrNull<AnimationPlayer>(AnimationPlayerPath) : FindAnimationPlayer();

        GD.Print($"[CharacterAnimationDriver] AnimationPlayer: {(_animationPlayer != null ? _animationPlayer.GetPath().ToString() : "None")}");

        if (_animationPlayer != null)
        {
            GD.Print($"[CharacterAnimationDriver] Animation list: {string.Join(", ", _animationPlayer.GetAnimationList())}");
            ResolveClips();
            ForceLoopIfPossible(_idleClip);
            ForceLoopIfPossible(_walkClip);
            ForceLoopIfPossible(_runClip);
            GD.Print($"[CharacterAnimationDriver] Resolved clips | idle={_idleClip} walk={_walkClip} run={_runClip} fire={_fireClip} reload={_reloadClip}");
            if (UseLayeredAnimationTree)
            {
                BuildLayeredAnimationTree();
            }
        }

        if (_animationTree != null)
        {
            _animationTree.Active = _treeReady;
        }

        runtime.ActiveAnimationDriver = DebugName;
    }

    public void Apply(CharacterRuntimeContext runtime, ActorIntent intent, double delta)
    {
        runtime.ActiveAnimationDriver = DebugName;
        runtime.ActiveAnimationClip = string.Empty;

        if (_treeReady && _animationTree != null)
        {
            string baseClip = ResolveBaseClip(runtime);
            runtime.ActiveAnimationClip = baseClip;

            _animationTree.Set("parameters/IdleWalkBlend/blend_amount", baseClip == _walkClip ? 1f : 0f);
            _animationTree.Set("parameters/LocomotionBlend/blend_amount", baseClip == _runClip ? 1f : 0f);

            if (runtime.IsFiring && !_lastFiring)
            {
                _animationTree.Set("parameters/FireShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
            }
            if (runtime.IsReloading && !_lastReloading)
            {
                _animationTree.Set("parameters/ReloadShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
            }

            _lastFiring = runtime.IsFiring;
            _lastReloading = runtime.IsReloading;
            _lastBaseClip = baseClip;
            return;
        }

        // Fallback if layered tree could not be built.
        if (_animationPlayer == null)
        {
            return;
        }

        string nextClip = runtime.IsReloading ? _reloadClip : runtime.IsFiring ? _fireClip : ResolveBaseClip(runtime);
        runtime.ActiveAnimationClip = nextClip;
        if (string.IsNullOrWhiteSpace(nextClip) || !_animationPlayer.HasAnimation(nextClip))
        {
            return;
        }

        if (_lastBaseClip == nextClip && _animationPlayer.IsPlaying())
        {
            return;
        }

        _lastBaseClip = nextClip;
        _animationPlayer.Play(nextClip);
    }

    private string ResolveBaseClip(CharacterRuntimeContext runtime)
    {
        return runtime.LocomotionState switch
        {
            LocomotionState.Sprinting when HasClip(_runClip) => _runClip,
            LocomotionState.Moving when HasClip(_walkClip) => _walkClip,
            _ when HasClip(_idleClip) => _idleClip,
            _ => string.Empty
        };
    }

    private void BuildLayeredAnimationTree()
    {
        _treeReady = false;
        if (_animationPlayer == null)
        {
            return;
        }

        if (_animationTree == null)
        {
            _animationTree = new AnimationTree { Name = "RuntimeAnimationTree" };
            AddChild(_animationTree);
            _animationTree.Owner = GetTree().EditedSceneRoot ?? this;
        }

        if (!HasClip(_idleClip) || !HasClip(_walkClip) || !HasClip(_runClip))
        {
            return;
        }

        var blendTree = new AnimationNodeBlendTree();

        var idleNode = new AnimationNodeAnimation { Animation = _idleClip };
        var walkNode = new AnimationNodeAnimation { Animation = _walkClip };
        var runNode = new AnimationNodeAnimation { Animation = _runClip };
        var fireNode = new AnimationNodeAnimation { Animation = _fireClip };
        var reloadNode = new AnimationNodeAnimation { Animation = _reloadClip };
        var idleWalkBlend = new AnimationNodeBlend2();
        var locomotionBlend = new AnimationNodeBlend2();
        var fireShot = new AnimationNodeOneShot
        {
            FadeInTime = 0.05f,
            FadeOutTime = 0.12f,
            MixMode = AnimationNodeOneShot.MixModeEnum.Blend,
            FilterEnabled = true
        };
        var reloadShot = new AnimationNodeOneShot
        {
            FadeInTime = 0.05f,
            FadeOutTime = 0.18f,
            MixMode = AnimationNodeOneShot.MixModeEnum.Blend,
            FilterEnabled = true
        };

        blendTree.AddNode("Idle", idleNode, new Vector2(0f, 0f));
        blendTree.AddNode("Walk", walkNode, new Vector2(180f, 0f));
        blendTree.AddNode("Run", runNode, new Vector2(360f, 0f));
        blendTree.AddNode("FireAnim", fireNode, new Vector2(360f, -180f));
        blendTree.AddNode("ReloadAnim", reloadNode, new Vector2(360f, 180f));
        blendTree.AddNode("IdleWalkBlend", idleWalkBlend, new Vector2(120f, 0f));
        blendTree.AddNode("LocomotionBlend", locomotionBlend, new Vector2(260f, 0f));
        blendTree.AddNode("FireShot", fireShot, new Vector2(460f, -40f));
        blendTree.AddNode("ReloadShot", reloadShot, new Vector2(640f, -20f));

        blendTree.ConnectNode("IdleWalkBlend", 0, "Idle");
        blendTree.ConnectNode("IdleWalkBlend", 1, "Walk");
        blendTree.ConnectNode("LocomotionBlend", 0, "IdleWalkBlend");
        blendTree.ConnectNode("LocomotionBlend", 1, "Run");
        blendTree.ConnectNode("FireShot", 0, "LocomotionBlend");
        blendTree.ConnectNode("FireShot", 1, "FireAnim");
        blendTree.ConnectNode("ReloadShot", 0, "FireShot");
        blendTree.ConnectNode("ReloadShot", 1, "ReloadAnim");
        blendTree.ConnectNode("output", 0, "ReloadShot");

        ApplyUpperBodyFilters(fireShot, _fireClip);
        ApplyUpperBodyFilters(reloadShot, _reloadClip);

        _animationTree.AnimPlayer = _animationTree.GetPathTo(_animationPlayer);
        _animationTree.TreeRoot = blendTree;
        _animationTree.Active = true;
        _animationTree.Set("parameters/IdleWalkBlend/blend_amount", 0f);
        _animationTree.Set("parameters/LocomotionBlend/blend_amount", 0f);
        _treeReady = true;
    }

    private void ApplyUpperBodyFilters(AnimationNodeOneShot oneShot, string clipName)
    {
        if (_animationPlayer == null || string.IsNullOrWhiteSpace(clipName) || !_animationPlayer.HasAnimation(clipName))
        {
            return;
        }

        var animation = _animationPlayer.GetAnimation(clipName);
        if (animation == null)
        {
            return;
        }

        for (int i = 0; i < animation.GetTrackCount(); i++)
        {
            string path = animation.TrackGetPath(i).ToString();
            if (IsUpperBodyTrack(path))
            {
                oneShot.SetFilterPath(new NodePath(path), true);
            }
        }
    }

    private static bool IsUpperBodyTrack(string trackPath)
    {
        string lower = trackPath.ToLowerInvariant();
        if (lower.Contains("thigh") || lower.Contains("shin") || lower.Contains("toe") || lower.Contains("foot") || lower.Contains("heel") || lower.Contains("pelvis"))
        {
            return false;
        }

        return lower.Contains("spine")
            || lower.Contains("chest")
            || lower.Contains("upper_arm")
            || lower.Contains("forearm")
            || lower.Contains("hand")
            || lower.Contains("thumb")
            || lower.Contains("palm")
            || lower.Contains("f_index")
            || lower.Contains("f_middle")
            || lower.Contains("f_ring")
            || lower.Contains("f_pinky")
            || lower.Contains("jaw")
            || lower.Contains("eye")
            || lower.Contains("neck")
            || lower.Contains("head");
    }

    
    private void ResolveClips()
    {
        _idleClip = ResolveAnimationName(IdleAnimation, "idle", "action");
        _walkClip = ResolveAnimationName(WalkAnimation, "walk");
        _runClip = ResolveAnimationName(RunAnimation, "run");
        _reloadClip = ResolveAnimationName(ReloadAnimation, "reload");
        _fireClip = ResolveAnimationName(FireAnimation, "fire");
    }
    private void ForceLoopIfPossible(string clipName)
    {
        if (_animationPlayer == null || string.IsNullOrWhiteSpace(clipName) || !_animationPlayer.HasAnimation(clipName))
        {
            return;
        }

        var animation = _animationPlayer.GetAnimation(clipName);
        if (animation != null)
        {
            animation.LoopMode = Godot.Animation.LoopModeEnum.Linear;
        }
    }

    private string ResolveAnimationName(string primary, params string[] fallbacks)
    {
        if (_animationPlayer == null)
        {
            return string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(primary) && _animationPlayer.HasAnimation(primary))
        {
            return primary;
        }

        var allKeys = new System.Collections.Generic.List<string>();
        if (!string.IsNullOrWhiteSpace(primary))
        {
            allKeys.Add(primary);
        }
        allKeys.AddRange(fallbacks);

        foreach (string key in allKeys)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                continue;
            }

            string expected = key.ToLowerInvariant();
            foreach (string clip in _animationPlayer.GetAnimationList())
            {
                if (clip.ToLowerInvariant().Contains(expected))
                {
                    return clip;
                }
            }
        }

        return string.Empty;
    }

    private bool HasClip(string clipName)
    {
        return !string.IsNullOrWhiteSpace(clipName) && _animationPlayer != null && _animationPlayer.HasAnimation(clipName);
    }

    private AnimationPlayer? FindAnimationPlayer()
    {
        Node searchRoot = !SearchRootPath.IsEmpty ? GetNodeOrNull<Node>(SearchRootPath) ?? this : this;
        return FindAnimationPlayerRecursive(searchRoot);
    }

    private static AnimationPlayer? FindAnimationPlayerRecursive(Node root)
    {
        foreach (Node child in root.GetChildren())
        {
            if (child is AnimationPlayer animationPlayer)
            {
                return animationPlayer;
            }
            AnimationPlayer? nested = FindAnimationPlayerRecursive(child);
            if (nested != null)
            {
                return nested;
            }
        }
        return null;
    }
}

