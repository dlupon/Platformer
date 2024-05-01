using System.Drawing.Text;
using Godot;

namespace Com.Unbocal.Platformer
{
    public partial class InputManager : Node
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Variables

        private const float JOY_SENSITIVITY = .1f; 

        public static class  Player
        {
            // INPUT NAMES
            private const string INPUT_NAME_GO_FORWARD = "go_forward";
            private const string INPUT_NAME_GO_BACKWARD = "go_backward";
            private const string INPUT_NAME_GO_LEFT = "go_left";
            private const string INPUT_NAME_GO_RIGHT = "go_right";
            private const string INPUT_NAME_TURN_LEFT = "turn_left";
            private const string INPUT_NAME_TURN_RIGHT = "turn_right";
            private const string INPUT_NAME_JUMP = "jump";

            // IS INPUT PRESSED
            public static bool goForward => Input.IsActionPressed(INPUT_NAME_GO_FORWARD);
            public static bool goBackward => Input.IsActionPressed(INPUT_NAME_GO_BACKWARD);
            public static bool goLeft => Input.IsActionPressed(INPUT_NAME_GO_LEFT);
            public static bool goRight => Input.IsActionPressed(INPUT_NAME_GO_RIGHT);
            public static bool jump => Input.IsActionJustPressed(INPUT_NAME_JUMP);

            // INPUT STRENGTH
            public static float goForwardStrength => GetActionRawStrengthCorrected(INPUT_NAME_GO_FORWARD);
            public static float goBackwardStrength => GetActionRawStrengthCorrected(INPUT_NAME_GO_BACKWARD);
            public static float goLeftStrength => GetActionRawStrengthCorrected(INPUT_NAME_GO_LEFT);
            public static float goRightStrength => GetActionRawStrengthCorrected(INPUT_NAME_GO_RIGHT);
            public static float turnLeftStrength => GetActionRawStrengthCorrected(INPUT_NAME_TURN_LEFT);
            public static float turnRightStrength => GetActionRawStrengthCorrected(INPUT_NAME_TURN_RIGHT);
        }

        private static float GetActionRawStrengthCorrected(string pInputName) => Mathf.Clamp(Input.GetActionRawStrength(pInputName), JOY_SENSITIVITY, 1f);
    }
}
