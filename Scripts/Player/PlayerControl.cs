using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.TextCore.Text;

public class PlayerControl : NetworkBehaviour
{
    [SerializeField, Self] private CharacterController character_controller;
    [SerializeField, Self] private InventoryController inventory_controller;
    [SerializeField, Self] private Transform player_transform;

    private Transform camera_transform;
    private Transform snow_particles;
    private GameObject minimap;
    private MinimapLighting minimap_script;
    [SerializeField, Self] private PlayerHealth health;
    [SerializeField, Self] private SpiritMeter spirit_meter;

    [SerializeField, Self] private AudioSource footstep_audio_source;
    [SerializeField, Anywhere] private AudioSource flashlight_audio_source;
    [SerializeField, Self] private ImportSounds footstep_sounds;
    [SerializeField, Child] private SpriteRenderer minimap_cursor;
    [SerializeField, Child] private Flashlight_Handler flashlight_controller;

    [SerializeField, Anywhere] private AudioClip flashlight_on_sound;
    [SerializeField, Anywhere] private AudioClip flashlight_off_sound;

    private PlayerInput controls_manager;

    private bool jump_pressed = false;
    private NetworkVariable<bool> sprint_pressed = new NetworkVariable<bool>(false, writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> on_ground = new NetworkVariable<bool>(true, writePerm: NetworkVariableWritePermission.Owner);
    private float smoothing = 1.5f;
    private float flashlight_smoothing = 6.5f;
    private NetworkVariable<float> walking_timer = new NetworkVariable<float>(0f, writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> toggle_flashlight = new NetworkVariable<bool>(false, writePerm:NetworkVariableWritePermission.Owner);
    private bool toggled_flashlight = true;
    private int last_step = 0;
    private float groundedTimer = 0;
    private float fall_timer = 0;

    private Vector3 movement_velocity = Vector3.zero;
    private Vector2 look_velocity = Vector2.zero;
    private Vector2 frame_velocity = Vector2.zero;
    private Vector2 flash_look_velocity = Vector2.zero;
    private Vector2 flash_frame_velocity = Vector2.zero;
    private const float gravity_value = -9.81f;
    private const float jump_height = 1.0f;
    private const float damage_threshold = 1.5f;

    private const float player_speed = 5f;
    private const float player_speed_sprint = 9f;
    private const float player_camera_control_adjustment = 0.075f;

    private const int footstep_step_divisor = 12;
    private const int sprint_speed_divisor = 2;
    private const int walk_speed_divisor = 4;
    private Color Other_Player_Cursor;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    // NetworkBehaviours do not have the IsOwner Property in Awake() or in OnEnable()
    void Start()
    {
        toggle_flashlight.OnValueChanged += Flashlight_Changed;

        Other_Player_Cursor = new Color(255f, 255f, 0f);
        if (!IsOwner)
        {
            minimap_cursor.color = Other_Player_Cursor;
            return;
        }

        // Spatial blend = 0 (full 2d) for our own flashlight because it sounds weird otherwise.
        flashlight_audio_source.spatialBlend = 0;

        camera_transform = Camera.main.transform;
        snow_particles = camera_transform.GetComponentInChildren<ParticleSystem>().gameObject.transform;

        minimap = GameObject.FindGameObjectWithTag("Minimap Camera");
        if (minimap != null)
        {
            minimap_script = minimap.GetComponentInChildren<MinimapLighting>();

            minimap_script.player_object = this.gameObject.transform;
        } else
        {
            Debug.LogWarning("No Minimap Found In Scene!");
        }

        controls_manager = InputSettingsManager.input_management;

        InputSettingsManager.register_input_action(Jump_Pressed, "Jump");
        InputSettingsManager.register_input_action(Sprint_Pressed, "Sprint");
        InputSettingsManager.register_input_action(Movement_Handler, "Movement");
        InputSettingsManager.register_input_action(Flashlight_Pressed, "Flashlight Toggle");
        InputSettingsManager.register_input_action(inventory_controller.Pickup_Pressed, "Pick Up");
        InputSettingsManager.register_input_action(inventory_controller.Throw_Pressed, "Drop");

        Cursor.lockState = CursorLockMode.Locked;
    }

    void handle_grounded()
    {
        on_ground.Value = character_controller.isGrounded;
        if (on_ground.Value)
        {
            // cooldown interval to allow reliable jumping even whem coming down ramps
            groundedTimer = 0.2f;
        }
        else
        {
            fall_timer += Time.deltaTime;
        }

        if (groundedTimer > 0)
        {
            groundedTimer -= Time.deltaTime;
        }

        // slam into the ground
        if (on_ground.Value && movement_velocity.y < 0)
        {
            movement_velocity.y = 0;
        }
    }

    void mouse_look()
    {
        Vector2 mouse_delta = Mouse.current.delta.ReadValue();
        Vector2 raw_frame_velocity = Vector2.Scale(mouse_delta, Vector2.one * GameSettingsManager.look_sensitivity * player_camera_control_adjustment);

        frame_velocity = Vector2.Lerp(frame_velocity, raw_frame_velocity, 1 / smoothing);
        flash_frame_velocity = Vector2.Lerp(flash_frame_velocity, raw_frame_velocity, 1 / flashlight_smoothing);
        look_velocity += frame_velocity;
        flash_look_velocity += flash_frame_velocity;
        look_velocity.y = Mathf.Clamp(look_velocity.y, -90, 90);
        flash_look_velocity.y = Mathf.Clamp(flash_look_velocity.y, -90, 90);

        player_transform.rotation = Quaternion.AngleAxis(look_velocity.x, Vector3.up);
        camera_transform.rotation = Quaternion.AngleAxis(look_velocity.x, Vector3.up) * Quaternion.AngleAxis(-look_velocity.y, Vector3.right);
        flashlight_controller.Move_Flashlight(Quaternion.AngleAxis(flash_look_velocity.x, Vector3.up) * Quaternion.AngleAxis(flash_look_velocity.y, Vector3.right));
    }

    void move_and_jump(bool sprint_pressed)
    {
        movement_velocity.x *= sprint_pressed ? player_speed_sprint : player_speed;
        movement_velocity.z *= sprint_pressed ? player_speed_sprint : player_speed;

        if (movement_velocity.x != 0 || movement_velocity.z != 0)
        {
            walking_timer.Value += Time.deltaTime;
        }

        if (jump_pressed)
        {
            if (groundedTimer > 0)
            {
                groundedTimer = 0;

                movement_velocity.y += Mathf.Sqrt(jump_height * 2.0f * -gravity_value);
            }
        }

        character_controller.Move(player_transform.rotation * movement_velocity * Time.deltaTime);

        // This should be undone to prevent exponential speed
        movement_velocity.x /= sprint_pressed ? player_speed_sprint : player_speed;
        movement_velocity.z /= sprint_pressed ? player_speed_sprint : player_speed;
    }

    void play_sounds(bool sprint_pressed)
    {
        // multiply by 4 for sprinting speed, and divide by 2 if walking. Doing it this way so that I can compare to last_step more granually
        int current_step = Mathf.FloorToInt(walking_timer.Value * footstep_step_divisor);
        int sprint_step_divisor = (sprint_pressed ? sprint_speed_divisor : walk_speed_divisor);

        if (Mathf.FloorToInt(current_step / sprint_step_divisor) % 2 == 0 
         && on_ground.Value 
         && (current_step / sprint_step_divisor) != last_step / sprint_step_divisor)
        {
            footstep_audio_source.PlayOneShot(footstep_sounds.get_random());
            last_step = current_step;
        }
    }

    void handle_flashlight()
    {
        if (toggle_flashlight.Value && !toggled_flashlight)
        {
            toggled_flashlight = true;
            flashlight_audio_source.PlayOneShot(flashlight_controller.flashlight_enabled.Value ?
                flashlight_on_sound : flashlight_off_sound);

            if (!IsOwner) return; // Only owner controls the Toggle_Flashlight function.
            flashlight_controller.Toggle_Flashlight();
        }
    }

    void camera_follow()
    {
        camera_transform.position = new Vector3(player_transform.position.x, player_transform.position.y + 1f, player_transform.position.z);
        snow_particles.transform.eulerAngles = new Vector3(90f, 0f, 0f);
        if (minimap != null)
        {
            minimap.transform.position = new Vector3(player_transform.position.x, player_transform.position.y + 288f, player_transform.position.z);
            minimap.transform.localEulerAngles = new Vector3(90f, 0f, -player_transform.eulerAngles.y);
        }
    }

    void fall_damage()
    {
        if (on_ground.Value && fall_timer > 0f)
        {
            float painful_fall_time = fall_timer - damage_threshold;
            fall_timer = 0f;
            if (painful_fall_time > 0f)
            {
                float damage = painful_fall_time * 24;
                health.Update_Health(health.health - Mathf.FloorToInt(damage));
            }
        }
    }

    /* Events and Update() */

    void Update()
    {
        bool local_pressed_sprint = sprint_pressed.Value;
        play_sounds(local_pressed_sprint); // Play sounds for all clients, instead of just our own.
        handle_flashlight();

        if (!IsOwner) return; // Only move, look around, and handle fall damage for our own client.

        handle_grounded();

        if (!MenuState.is_paused)
        {
            mouse_look();
        }

        movement_velocity.y += gravity_value * Time.deltaTime;


        move_and_jump(local_pressed_sprint);

        camera_follow();
        fall_damage();
    }

    public void Jump_Pressed(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;
        jump_pressed = ctx.performed; // Performed is wether or not this was a key down event
    }

    public void Flashlight_Pressed(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;
        toggle_flashlight.Value = ctx.performed;
    }

    public void Flashlight_Changed(bool old_value, bool new_value)
    {
        if (old_value != new_value) toggled_flashlight = false;
    }

    public void Sprint_Pressed(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;
        sprint_pressed.Value = ctx.performed;
    }

    public void Movement_Handler(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;
        Vector2 movement = ctx.ReadValue<Vector2>();
        movement.Normalize();
        movement_velocity.x = movement.x;
        movement_velocity.z = movement.y;
    }
}