namespace RAGENativeUI.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        Game.LogTrivialDebug("UnitTest1.Test1  -" + Thread.CurrentThread.ManagedThreadId);

        var ped = new Ped(Game.LocalPlayer.Character.GetOffsetPositionFront(5.0f));
        GameFiber.Yield();

        ped.Tasks.FightAgainst(Game.LocalPlayer.Character);

        GameFiber.Sleep(5000);

        if (ped) ped.Delete();

        Assert.False(false);
    }

    [Fact]
    public void Test2()
    {
        Game.LogTrivialDebug("UnitTest1.Test2  -" + Thread.CurrentThread.ManagedThreadId);

        var ped = new Ped(Game.LocalPlayer.Character.GetOffsetPositionFront(5.0f));
        ped.GiveHelmet(false, HelmetTypes.RegularMotorcycleHelmet, 0);
        GameFiber.Yield();

        ped.Tasks.FightAgainst(Game.LocalPlayer.Character);

        GameFiber.Sleep(5000);

        if (ped) ped.Delete();

        Assert.False(true);
    }
}
