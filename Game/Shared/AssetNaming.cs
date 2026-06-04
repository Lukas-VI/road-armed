namespace RoadArmed.Game.Shared;

public static class AssetNaming
{
    public static class CharacterBones
    {
        public const string Root = "Root";
        public const string Hips = "Hips";
        public const string Spine = "Spine";
        public const string Chest = "Chest";
        public const string UpperChest = "UpperChest";
        public const string Neck = "Neck";
        public const string Head = "Head";
        public const string LeftShoulder = "LeftShoulder";
        public const string LeftUpperArm = "LeftUpperArm";
        public const string LeftLowerArm = "LeftLowerArm";
        public const string LeftHand = "LeftHand";
        public const string RightShoulder = "RightShoulder";
        public const string RightUpperArm = "RightUpperArm";
        public const string RightLowerArm = "RightLowerArm";
        public const string RightHand = "RightHand";
        public const string LeftUpperLeg = "LeftUpperLeg";
        public const string LeftLowerLeg = "LeftLowerLeg";
        public const string LeftFoot = "LeftFoot";
        public const string LeftToes = "LeftToes";
        public const string RightUpperLeg = "RightUpperLeg";
        public const string RightLowerLeg = "RightLowerLeg";
        public const string RightFoot = "RightFoot";
        public const string RightToes = "RightToes";
    }

    public static class CharacterSockets
    {
        public const string RightHand = "socket_hand_r";
        public const string LeftHand = "socket_hand_l";
        public const string Muzzle = "socket_muzzle";
        public const string Magazine = "socket_magazine";
        public const string Camera = "socket_camera";
        public const string Aim = "socket_aim";
        public const string Interaction = "socket_interact";
        public const string FootIkL = "socket_foot_ik_l";
        public const string FootIkR = "socket_foot_ik_r";
    }

    public static class VehicleSockets
    {
        public const string DriverCamera = "socket_camera_driver";
        public const string GunnerCamera = "socket_camera_gunner";
        public const string CommanderCamera = "socket_camera_commander";
        public const string TurretYaw = "socket_turret_yaw";
        public const string BarrelPitch = "socket_barrel_pitch";
        public const string BarrelMuzzle = "socket_barrel_muzzle";
    }

    public static class AircraftSockets
    {
        public const string CockpitCamera = "socket_camera_cockpit";
        public const string ChaseCamera = "socket_camera_chase";
        public const string WeaponPylonL = "socket_pylon_l";
        public const string WeaponPylonR = "socket_pylon_r";
        public const string GunMuzzle = "socket_gun_muzzle";
    }

    public static class AnimationKeys
    {
        public const string IdleUnarmed = "locomotion_idle_unarmed";
        public const string IdleRifle = "locomotion_idle_rifle";
        public const string WalkForward = "locomotion_walk_fwd";
        public const string WalkBackward = "locomotion_walk_bwd";
        public const string WalkLeft = "locomotion_walk_left";
        public const string WalkRight = "locomotion_walk_right";
        public const string RunForward = "locomotion_run_fwd";
        public const string CrouchIdle = "locomotion_crouch_idle";
        public const string JumpStart = "locomotion_jump_start";
        public const string JumpLoop = "locomotion_jump_loop";
        public const string Land = "locomotion_land";
        public const string TurnLeft90 = "locomotion_turn_l_90";
        public const string TurnRight90 = "locomotion_turn_r_90";
        public const string EquipDefault = "equip_default";
        public const string EquipRifle = "equip_rifle";
        public const string UnequipDefault = "unequip_default";
        public const string UnequipRifle = "unequip_rifle";
        public const string FireRifle = "combat_fire_rifle";
        public const string ReloadRifle = "combat_reload_rifle";
        public const string SocialWave = "override_social_wave";
    }
}
