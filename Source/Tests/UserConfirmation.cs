namespace RAGENativeUI.Tests;

using System.Windows.Forms;

internal static class UserConfirmation
{
    public static bool Confirm(string message, float timeout = 15.0f)
    {
        while (timeout > 0.0f)
        {
            Game.DisplayHelp($"{message}~n~Looks good? ({(int)timeout} s)~n~~{InstructionalKey.Y.GetId()}~ ~g~Yes~n~~{InstructionalKey.N.GetId()}~ ~r~No");

            if (Game.IsKeyDownRightNow(Keys.Y)) return true;
            if (Game.IsKeyDownRightNow(Keys.N)) return false;

            GameFiber.Yield();
            timeout -= Game.FrameTime;
        }

        return false;
    }
}
