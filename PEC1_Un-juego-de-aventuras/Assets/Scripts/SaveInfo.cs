using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveInfo
{
    public int level;
    public Time time;
    public int currentHealth;

    public SaveInfo(int level, Time time, int currentHealth)
    {
        this.level = level;
        this.time = time;
        this.currentHealth = currentHealth;
    }
}
