using UnityEngine;

[CreateAssetMenu(fileName = "NewSongData", menuName = "Rhythm Game/Song Data")]
public class SongData : ScriptableObject
{
    [Header("Información Básica")]
    public string songName;
    public AudioClip audioClip;
    
    [Header("Configuración Rítmica")]
    public float bpm;             // BPM específico
    public float firstBeatOffset; // (Opcional) Por si la canción no empieza en el segundo 0
}