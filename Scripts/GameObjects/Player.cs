using System;
using Godot;

// Author	: Dylan Lupon
// Date		: 05/01/2024
namespace Com.Unbocal.Platformer.GameObjects
{
	
	public partial class Player : CharacterBody3D
	{
		// ----------------~~~~~~~~~~~~~~~~~~~==========================# // Variables

		// UTILITIES
		private float deltaTime;

		// STATE MACHIN
		private Action MovementsAction;
		private Action CameraAction;

		// MOVEMENTS
        // Displacement
		[Export] private float accelerationRunMax = 100f;
		[Export] private float accelerationDurationIn = .1f;
		[Export] private float accelerationDurationOut = .25f;
		[Export] private float jumpStregth = 100f;
		[Export] private float gravityStregth = 3f;
		private const float VELOCITY_SMOOTHNESS = 10f;
        private float accelerationSpeed;
		private float decelerationSpeed;
		private Vector3 acceleration = Vector3.Zero;
		private Vector3 floorNormal = Vector3.Up;
		private Vector3 inputDirection;
		// Dash
		private const float DASH_STRENGTH = 100f;
		// Rotation
		private const float ROTATION_SMOOTHNESS = 5f;
		private Vector3 rotationVelocity = Vector3.Zero;
		private Vector2 rotationAngleVector = Vector2.Zero;

		// JUMP
		[Export] private int jumpAmount = 2;
		private int jumpAvalable = 0;

		// CAMERA
		[Export] private NodePath cameraArmPath;
		[Export] private NodePath cameraPath;
		private SpringArm3D cameraArm;
		private Camera3D camera;
		private float cameraRotationSpeed = Mathf.DegToRad(180f);

		// RENDERER
		[Export] private NodePath rendererPath;
		private Node3D renderer;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization

        public override void _Ready()
        {
			// Init
			SetNodes();

			// Set Properties
            SetMouvementsFalling();
			SetCameraRotation();
        }

		private void SetNodes()
		{
			renderer = (Node3D)GetNode(rendererPath);
            cameraArm = (SpringArm3D)GetNode(cameraArmPath);
            camera = (Camera3D)GetNode(cameraPath);
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Process

        public override void _PhysicsProcess(double pDelta)
        {
			// Update
			InputDirectionUpDate();
			deltaTime = (float)pDelta;

			// Movements
			CameraAction();
            MovementsAction();
        }

		// ----------------~~~~~~~~~~~~~~~~~~~==========================# // Inputs

		private void InputDirectionUpDate()
		{
			// Reset Direction
			inputDirection = Vector3.Zero;

			// Apply Direction From Input
			inputDirection.Z = InputManager.Player.goBackwardStrength - InputManager.Player.goForwardStrength;
			inputDirection.X = InputManager.Player.goRightStrength - InputManager.Player.goLeftStrength;

			// Clamp To Not Get Over A Length Of One
			inputDirection = inputDirection.Length() > 1f ? inputDirection.Normalized() : inputDirection;
		}

		// ----------------~~~~~~~~~~~~~~~~~~~==========================# // State Machine

		// CAMERA
		private void SetCameraRotation() => CameraAction = CameraRotation;

		// MOVEMENTS
		private void SetMouvementsGround()
		{
			Velocity *= Vector3.One - floorNormal;
			jumpAvalable = jumpAmount;
			MovementsAction = MovementsOnGround;
        }

		private void SetMouvementsFalling()
		{
			MovementsAction = MovementFalling;
		}

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Camera Actions
        
		private void CameraStatic() { }

        private void CameraRotation()
        {
			camera.LookAt(GlobalPosition);
            cameraArm.Rotation += (InputManager.Player.turnLeftStrength - InputManager.Player.turnRightStrength) * Vector3.Up * cameraRotationSpeed * deltaTime;
            cameraArm.Rotation += (InputManager.Player.turnUpStrength - InputManager.Player.turnDownStrength) * Vector3.Right * cameraRotationSpeed * deltaTime;
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Movement Actions


		// MOVEMENTS ON GROUND
        private void MovementsOnGround()
		{
            MustJump();
            VelocityApply();
			RotationVelocityApply();
            MoveAndSlide();
            ActionOnFallingCheck(SetMouvementsFalling);
		}

		// MOVEMENTS FALLING
		private void MovementFalling()
		{
            MustJump();
			GravityApply();
            VelocityApply();
			RotationVelocityApply();
			MustDash();
            MoveAndSlide();
			ActionOnGroundCheck(SetMouvementsGround);
        }

		// MOVEMENTS COMMON ACTIONS
		private void GravityApply() => Velocity -= floorNormal * gravityStregth *deltaTime;
		
		private void VelocityApply()
		{
			acceleration = (inputDirection * accelerationRunMax).Rotated(floorNormal, camera.GlobalRotation.Y);
			Velocity = Velocity.Lerp(acceleration,deltaTime * VELOCITY_SMOOTHNESS);
		}

		private void RotationVelocityApply()
		{
			// Rotate Only If It's Needed
            if (acceleration.Length() <= 0f) return;

			// Get A Smooth Rotation
            rotationVelocity = rotationVelocity.Lerp(Velocity,deltaTime * ROTATION_SMOOTHNESS);

			// Update The Angle
            rotationAngleVector.X = rotationVelocity.Z;
            rotationAngleVector.Y = rotationVelocity.X;
            renderer.GlobalRotation = Vector3.Up * rotationAngleVector.Angle();
        }

        private void MustDash()
		{
			if (!InputManager.Player.dash) return;
			Velocity = (inputDirection.Rotated(floorNormal, camera.GlobalRotation.Y) + floorNormal) * DASH_STRENGTH;
        }

        private void ActionOnGroundCheck(Action pSwitchAction) { if (IsOnFloor()) pSwitchAction(); }
		
		private void ActionOnFallingCheck(Action pSwitchAction) { if (!IsOnFloor()) pSwitchAction(); }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Jump

        private void MustJump()
		{
			// If The Player Is Falling From a Platform
            if (!IsOnFloor() && jumpAvalable >= jumpAmount) jumpAvalable--;

			// Player Want To Jump And Can Do It
            if (jumpAvalable > 0 && InputManager.Player.jump)
            {
                Velocity *= Vector3.One - floorNormal;
                Velocity += floorNormal * jumpStregth;
				jumpAvalable--;
            }
        }
	}
}