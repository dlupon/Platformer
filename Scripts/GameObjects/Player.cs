using Godot;
using Com.Unbocal.Platformer.Utilities;
using System.Drawing.Text;

// Author	: Dylan Lupon
// Date		: 00/00/0000
namespace Com.Unbocal.Platformer.GameObjects
{
	
	public partial class Player : KinematicBody
	{
		// ----------------~~~~~~~~~~~~~~~~~~~==========================# // Variables

		// MOVEMENTS
		[Export] private float accelerationSpeed = 100f;
		[Export] private float jumpStregth = 300f;
		[Export] private float gravityStregth = 2.5f;
		[Export] private Vector3 frictionOnGround = new Vector3(10f, 2.5f, 10f);
		[Export] private Vector3 frictionInAir = new Vector3(9f, 5f, 9f);
		private Vector3 floorNormal = Vector3.Up;
		private Vector3 acceleration = Vector3.Zero;
		private Vector3 velocity = Vector3.Zero;
		private Vector3 currentFriction;


		// ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization

		// ----------------~~~~~~~~~~~~~~~~~~~==========================# // Process

        public override void _PhysicsProcess(float pDelta)
        {
            Move(pDelta);
			GD.Print(velocity.y);
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Action

        private void Move(float pDelta)
		{
			// HANDLE ROTATIONS
			Rotation += (InputManager.Player.turnLeftStrength - InputManager.Player.turnRightStrength) * Vector3.Up * 2.5f * pDelta;


			// HANDLE MOVEMENTS

			// Init Movement Properties
			acceleration = Vector3.Zero;
			currentFriction = IsOnFloor() ? frictionOnGround : frictionInAir;
			GD.Print(currentFriction.y);

			// Handle Gravity
			if (IsOnFloor()) velocity.y = 0f;
			acceleration -= floorNormal * gravityStregth;

			// Set Acceleration Based On Input
			// Walk / Run
			acceleration.z = InputManager.Player.goBackwardStrength - InputManager.Player.goForwardStrength;
			acceleration.x = InputManager.Player.goRightStrength - InputManager.Player.goLeftStrength;
			// Junmp
			if (InputManager.Player.jump) {acceleration += floorNormal * jumpStregth; velocity.y = 0f;}

			// Apply Acceleration And Friction
			velocity += acceleration.Rotated(Vector3.Up, Rotation.y) * accelerationSpeed * pDelta;
			velocity -= velocity * currentFriction * pDelta;

			// Move Body
			MoveAndSlide(velocity, floorNormal, true);
		}

	}
}