using System.Collections;

public interface IGamePhase
{
    GameInstance Game { set; }
    IEnumerator OnEnter();
    IEnumerator Loop() { while (true) yield return null; }
    IEnumerator OnExit();
}