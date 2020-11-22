using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCShop : NPC
{
    public GameObject panelShop;

    public override void TriggerDialogue()
    {
        panelShop.SetActive(!panelShop.activeInHierarchy);
    }
}
