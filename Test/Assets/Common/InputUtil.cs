using UnityEngine;
using UnityEditor;

public class InputUtil
{
    public enum INPUT_MOVE_COMMAND
    {
        NONE,
        F_L,
        F,
        F_R,
        R,
        B_R,
        B,
        B_L,
        L,
    }

    public class MoveCommand
    {
        public INPUT_MOVE_COMMAND command = INPUT_MOVE_COMMAND.NONE;
        public float rotation = 0;
        public Vector2 xy = new Vector2(0, 0);
        public bool isShift = false;

        public bool IsDash()
        {
            return isShift;
        }

        public bool IsMove()
        {
            return !INPUT_MOVE_COMMAND.NONE.Equals(command);
        }
    }

    public static MoveCommand GetMoveCommand()
    {
        MoveCommand ret = new MoveCommand();

        float x = 0;
        float y = 0;

        switch ((ret.command = InputMoveCommand()))
        {
            case INPUT_MOVE_COMMAND.F_L:
                y = 1;
                x = -1;
                ret.rotation = -45f;
                break;

            case INPUT_MOVE_COMMAND.F_R:
                y = 1;
                x = 1;
                ret.rotation = 45f;
                break;

            case INPUT_MOVE_COMMAND.B_R:
                y = -1;
                x = 1;
                ret.rotation = 135f;
                break;

            case INPUT_MOVE_COMMAND.B_L:
                y = -1;
                x = -1;
                ret.rotation = 225f;
                break;

            case INPUT_MOVE_COMMAND.R:
                ret.rotation = 90f;
                x = 1;
                break;

            case INPUT_MOVE_COMMAND.B:
                ret.rotation = 180f;
                y = -1;
                break;

            case INPUT_MOVE_COMMAND.L:
                ret.rotation = 270;
                x = -1;
                break;

            case INPUT_MOVE_COMMAND.F:
                y = 1;
                break;

            default:
                break;
        }

        ret.isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        ret.xy = new Vector2(x, y);

        return ret;
    }

    public static INPUT_MOVE_COMMAND InputMoveCommand()
    {

        if (Input.GetKey("up") && Input.GetKey("left"))
        {
            return INPUT_MOVE_COMMAND.F_L;
        }
        else if (Input.GetKey("up") && Input.GetKey("right"))
        {
            return INPUT_MOVE_COMMAND.F_R;
        }
        else if (Input.GetKey("down") && Input.GetKey("right"))
        {
            return INPUT_MOVE_COMMAND.B_R;
        }
        else if (Input.GetKey("down") && Input.GetKey("left"))
        {
            return INPUT_MOVE_COMMAND.B_L;
        }
        else if (Input.GetKey("right"))
        {
            return INPUT_MOVE_COMMAND.R;
        }
        else if (Input.GetKey("down"))
        {
            return INPUT_MOVE_COMMAND.B;
        }
        else if (Input.GetKey("left"))
        {
            return INPUT_MOVE_COMMAND.L;
        }
        else if (Input.GetKey("up"))
        {
            return INPUT_MOVE_COMMAND.F;
        }


        return INPUT_MOVE_COMMAND.NONE;
    }
}