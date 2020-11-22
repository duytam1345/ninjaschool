using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCThayHiroyaki : NPC
{
    public override void TriggerDialogue()
    {
        string s = Manager.manager.mission.ToString();
        string a = s.Substring(1, s.Length - 1);

        Manager.manager.StartDialogue(dialogue[Convert.ToInt32(a) - 1], this);
    }
}
