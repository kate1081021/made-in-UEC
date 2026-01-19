using UnityEngine;

public interface IMiniGame
{
    // bool IsClear { get; }
    AudioClip GameBGM { get; }
    void OnGameStart();
    void OnGameEnd();
    void StopAllSounds();
}