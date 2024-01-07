using ExtendedEditorPlugins;
using Godot;

namespace Core.Examples;

[RegisterCustomNode(typeof(Example2DPlayer))]
public partial class Example2DPlayer : CharacterBody2D {
    [Export] public float WalkSpeed = 400;
    [Export] public float RunSpeed = 400;
    [Export] public float JumpForce = 800;

    public float Gravity => ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);

        bool isRunning = Input.IsPhysicalKeyPressed(Key.Shift);
        var movementSpeed = (isRunning) ? RunSpeed : WalkSpeed;

        Vector2 velocity = Velocity;

        if (!IsOnFloor()) {
            velocity.Y += Gravity * (float)delta;
        }

        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor()) {
            velocity.Y = -JumpForce;
        }

        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        if (direction != Vector2.Zero) {
            velocity.X = direction.X * movementSpeed;
        } else {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, movementSpeed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}