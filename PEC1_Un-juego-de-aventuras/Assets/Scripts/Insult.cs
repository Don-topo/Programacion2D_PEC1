using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Insult
{
    public string insultText;
    public int counterText;

    public Insult(Insult insult)
    {
        insultText = insult.insultText;
        counterText = insult.counterText;
    }
}
