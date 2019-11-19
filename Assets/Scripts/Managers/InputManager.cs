// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Managers/InputManager.inputactions'

using System;
using UnityEngine;
using UnityEngine.Experimental.Input;


[Serializable]
public class InputManager : InputActionAssetReference
{
    public InputManager()
    {
    }
    public InputManager(InputActionAsset asset)
        : base(asset)
    {
    }
    private bool m_Initialized;
    private void Initialize()
    {
        // Spawn
        m_Spawn = asset.GetActionMap("Spawn");
        m_Spawn_Scout = m_Spawn.GetAction("Scout");
        m_Spawn_Tank = m_Spawn.GetAction("Tank");
        m_Spawn_Warrior = m_Spawn.GetAction("Warrior");
        m_Spawn_Range = m_Spawn.GetAction("Range");
        m_Spawn_SimpleWall = m_Spawn.GetAction("SimpleWall");
        m_Spawn_SlopeLeft = m_Spawn.GetAction("SlopeLeft");
        m_Spawn_SlopeRight = m_Spawn.GetAction("SlopeRight");
        m_Spawn_Pillar = m_Spawn.GetAction("Pillar");
        m_Spawn_Trap = m_Spawn.GetAction("Trap");
        // Camera
        m_Camera = asset.GetActionMap("Camera");
        m_Camera_ChangeView = m_Camera.GetAction("ChangeView");
        m_Camera_FocusSpawn1 = m_Camera.GetAction("FocusSpawn1");
        m_Camera_FocusSpawn2 = m_Camera.GetAction("FocusSpawn2");
        m_Camera_MouseClick = m_Camera.GetAction("MouseClick");
        m_Camera_MouseMove = m_Camera.GetAction("MouseMove");
        // Checkpoint
        m_Checkpoint = asset.GetActionMap("Checkpoint");
        m_Checkpoint_ReleaseSlot1 = m_Checkpoint.GetAction("ReleaseSlot1");
        m_Checkpoint_ReleaseSlot2 = m_Checkpoint.GetAction("ReleaseSlot2");
        m_Checkpoint_ReleaseSlot3 = m_Checkpoint.GetAction("ReleaseSlot3");
        m_Initialized = true;
    }
    private void Uninitialize()
    {
        m_Spawn = null;
        m_Spawn_Scout = null;
        m_Spawn_Tank = null;
        m_Spawn_Warrior = null;
        m_Spawn_Range = null;
        m_Spawn_SimpleWall = null;
        m_Spawn_SlopeLeft = null;
        m_Spawn_SlopeRight = null;
        m_Spawn_Pillar = null;
        m_Spawn_Trap = null;
        m_Camera = null;
        m_Camera_ChangeView = null;
        m_Camera_FocusSpawn1 = null;
        m_Camera_FocusSpawn2 = null;
        m_Camera_MouseClick = null;
        m_Camera_MouseMove = null;
        m_Checkpoint = null;
        m_Checkpoint_ReleaseSlot1 = null;
        m_Checkpoint_ReleaseSlot2 = null;
        m_Checkpoint_ReleaseSlot3 = null;
        m_Initialized = false;
    }
    public void SetAsset(InputActionAsset newAsset)
    {
        if (newAsset == asset) return;
        if (m_Initialized) Uninitialize();
        asset = newAsset;
    }
    public override void MakePrivateCopyOfActions()
    {
        SetAsset(ScriptableObject.Instantiate(asset));
    }
    // Spawn
    private InputActionMap m_Spawn;
    private InputAction m_Spawn_Scout;
    private InputAction m_Spawn_Tank;
    private InputAction m_Spawn_Warrior;
    private InputAction m_Spawn_Range;
    private InputAction m_Spawn_SimpleWall;
    private InputAction m_Spawn_SlopeLeft;
    private InputAction m_Spawn_SlopeRight;
    private InputAction m_Spawn_Pillar;
    private InputAction m_Spawn_Trap;
    public struct SpawnActions
    {
        private InputManager m_Wrapper;
        public SpawnActions(InputManager wrapper) { m_Wrapper = wrapper; }
        public InputAction @Scout { get { return m_Wrapper.m_Spawn_Scout; } }
        public InputAction @Tank { get { return m_Wrapper.m_Spawn_Tank; } }
        public InputAction @Warrior { get { return m_Wrapper.m_Spawn_Warrior; } }
        public InputAction @Range { get { return m_Wrapper.m_Spawn_Range; } }
        public InputAction @SimpleWall { get { return m_Wrapper.m_Spawn_SimpleWall; } }
        public InputAction @SlopeLeft { get { return m_Wrapper.m_Spawn_SlopeLeft; } }
        public InputAction @SlopeRight { get { return m_Wrapper.m_Spawn_SlopeRight; } }
        public InputAction @Pillar { get { return m_Wrapper.m_Spawn_Pillar; } }
        public InputAction @Trap { get { return m_Wrapper.m_Spawn_Trap; } }
        public InputActionMap Get() { return m_Wrapper.m_Spawn; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(SpawnActions set) { return set.Get(); }
    }
    public SpawnActions @Spawn
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new SpawnActions(this);
        }
    }
    // Camera
    private InputActionMap m_Camera;
    private InputAction m_Camera_ChangeView;
    private InputAction m_Camera_FocusSpawn1;
    private InputAction m_Camera_FocusSpawn2;
    private InputAction m_Camera_MouseClick;
    private InputAction m_Camera_MouseMove;
    public struct CameraActions
    {
        private InputManager m_Wrapper;
        public CameraActions(InputManager wrapper) { m_Wrapper = wrapper; }
        public InputAction @ChangeView { get { return m_Wrapper.m_Camera_ChangeView; } }
        public InputAction @FocusSpawn1 { get { return m_Wrapper.m_Camera_FocusSpawn1; } }
        public InputAction @FocusSpawn2 { get { return m_Wrapper.m_Camera_FocusSpawn2; } }
        public InputAction @MouseClick { get { return m_Wrapper.m_Camera_MouseClick; } }
        public InputAction @MouseMove { get { return m_Wrapper.m_Camera_MouseMove; } }
        public InputActionMap Get() { return m_Wrapper.m_Camera; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(CameraActions set) { return set.Get(); }
    }
    public CameraActions @Camera
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new CameraActions(this);
        }
    }
    // Checkpoint
    private InputActionMap m_Checkpoint;
    private InputAction m_Checkpoint_ReleaseSlot1;
    private InputAction m_Checkpoint_ReleaseSlot2;
    private InputAction m_Checkpoint_ReleaseSlot3;
    public struct CheckpointActions
    {
        private InputManager m_Wrapper;
        public CheckpointActions(InputManager wrapper) { m_Wrapper = wrapper; }
        public InputAction @ReleaseSlot1 { get { return m_Wrapper.m_Checkpoint_ReleaseSlot1; } }
        public InputAction @ReleaseSlot2 { get { return m_Wrapper.m_Checkpoint_ReleaseSlot2; } }
        public InputAction @ReleaseSlot3 { get { return m_Wrapper.m_Checkpoint_ReleaseSlot3; } }
        public InputActionMap Get() { return m_Wrapper.m_Checkpoint; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(CheckpointActions set) { return set.Get(); }
    }
    public CheckpointActions @Checkpoint
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new CheckpointActions(this);
        }
    }
    private int m_ControlSchemeSchemeIndex = -1;
    public InputControlScheme ControlSchemeScheme
    {
        get

        {
            if (m_ControlSchemeSchemeIndex == -1) m_ControlSchemeSchemeIndex = asset.GetControlSchemeIndex("ControlScheme");
            return asset.controlSchemes[m_ControlSchemeSchemeIndex];
        }
    }
}
