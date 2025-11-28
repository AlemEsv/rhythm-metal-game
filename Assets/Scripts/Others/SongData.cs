using UnityEngine;

[CreateAssetMenu(fileName = "NewSongData", menuName = "Rhythm Game/Song Data")]
public class SongData : ScriptableObject
{
    [Header("Info")]
    public string songName;
    public AudioClip audioClip;
    
    [Header("BPM Settings")]
    public float bpm;             // BPM específico
    public float firstBeatOffset; // (Opcional) Por si la canción no empieza en el segundo 0
}