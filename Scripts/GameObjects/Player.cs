using System;
using Godot;

// Author	: Dylan Lupon
// Date		: 05/01/2024
namespace Com.Unbocal.Platformer.GameObjects
{
	
	public partial class Player : CharacterBody3D
	{
		// ----------------~~~~~~~~~~~~~~~~~~~==========================# // Variables

		// STATE MACHIN
		private Action<double> MovementsAction;
		private Action<double> CameraAction;

		// MOVEMENTS
		[Export] private float accelerationRunMax = 100f;
		[Export] private float accelerationDurationIn = .1f;
		[Export] private float accelerationDurationOut = .25f;
		[Export] private float jumpStregth = 75f;
		[Export] private float gravityStregth = 2.5f;

		private float accelerationSpeed;
		private float decelerationSpeed;
		private Vector3 acceleration = Vector3.Zero;
		private Vector3 rotationVelocity = Vector3.Zero;
		private Vector3 floorNormal = Vector3.Up;
		private Vector3 inputDirection;

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
			InputDirectionUpDate();
			CameraAction(pDelta);
            MovementsAction(pDelta);
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
        
		private void CameraStatic(double pDelta) { }

        private void CameraRotation(double pDelta)
        {
			camera.LookAt(GlobalPosition);
            cameraArm.Rotation += (InputManager.Player.turnLeftStrength - InputManager.Player.turnRightStrength) * Vector3.Up * cameraRotationSpeed * ((float)pDelta);
            cameraArm.Rotation += (InputManager.Player.turnUpStrength - InputManager.Player.turnDownStrength) * Vector3.Right * cameraRotationSpeed * ((float)pDelta);
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Movement Actions


		// MOVEMENTS ON GROUND
        private void MovementsOnGround(double pDelta)
		{
            MustJump();
            VelocityApply(pDelta);
			MoveAndSlide();
            FallingCheck(SetMouvementsFalling);
		}

		// MOVEMENTS FALLING
		private void MovementFalling(double pDelta)
		{
            MustJump();
			GravityApply(pDelta);
            VelocityApply(pDelta);
            MoveAndSlide();
			GroundCheck(SetMouvementsGround);
        }

		// MOVEMENTS COMMON ACTIONS
		private void VelocityApply(double pDelta)
		{
			acceleration = (inputDirection * accelerationRunMax).Rotated(floorNormal, camera.GlobalRotation.Y);			
			Velocity = Velocity.Lerp(acceleration, ((float)pDelta) * 10f);

			if (acceleration.Length() <= 0f) return;

			Vector2 test = Vector2.Zero;
			rotationVelocity = rotationVelocity.Lerp(Velocity, ((float)pDelta) * 5);
			
			test.X = rotationVelocity.Z;
			test.Y = rotationVelocity.X;


			Vector3 oui = Vector3.Up * test.Angle();


			renderer.GlobalRotation = oui;
		}
	
		private void GravityApply(double pDelta) => Velocity -= floorNormal * gravityStregth * ((float)pDelta);

		private void GroundCheck(Action pSwitchAction) { if (IsOnFloor()) pSwitchAction(); }
		
		private void FallingCheck(Action pSwitchAction) { if (!IsOnFloor()) pSwitchAction(); }

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