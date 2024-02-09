using Godot;



public partial class player : CharacterBody2D
{
    private AnimatedSprite2D anim;

    public override void _Ready()
    {
        anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    [Export]
    int Speed = 100;    // Hastighed, redigerbar fra Inspector (ved Export)
    [Export]
    float Jump = 200;

    float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();


    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");
        if (inputDirection != Vector2.Zero)
        {
            velocity.X = inputDirection.X * Speed;
            PlayAnimation(inputDirection);
        }
        else
        {
            anim.Play("idle");
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
        }

        // Add the gravity.
        if (!IsOnFloor())
            velocity.Y += gravity * (float)delta;

        // Handle Jump.
        if (Input.IsActionJustPressed("up") && IsOnFloor())
            velocity.Y = -Jump;

        Velocity = velocity;
        MoveAndSlide(); // Flytter sig i henhold fysikkens kræfter og glider af kolliderende Objekter

        Move_Rigidbody(10);

    }



    public void PlayAnimation(Vector2 dir)
    {
        if (dir == Vector2.Right || dir == Vector2.Left)
        {
            anim.FlipH = dir == Vector2.Left;
            anim.Play("walk");
        }
    }

    public void Move_Rigidbody(float push_force)
    {
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            var c = GetSlideCollision(i);
            if (c.GetCollider() is RigidBody2D)
            {
                (c.GetCollider() as RigidBody2D).ApplyCentralImpulse(-c.GetNormal() * push_force);
            }
        }
    }
}
