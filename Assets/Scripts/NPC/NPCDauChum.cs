using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDauChum : NPC
{
    private void Update()
    {
        Move();
    }

    public override void TriggerDialogue()
    {
        if (!moving)
        {
            Manager.manager.StartDialogue(dialogue[0], this);
        }
    } 
}
