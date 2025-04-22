using System;
using UnityEngine;

public class HandleSWOffMono : BaseSWMono
{
    public bool IsTrue;
    private void Start()
    {
        
    }

    public override void OnPlayerFocus()
    {
        base.OnPlayerFocus();
    }

    public override void OnPlayerStartInteract()
    {
        base.OnPlayerStartInteract();
    }

    public override void OnPlayerInteract()
    {
        base.OnPlayerInteract();
        
        GameItemTool.GenerateItemAtPosition(220014, gameObject.transform.position, false, itembase =>
        {
            gameObject.SetActive(false);
            itembase.transform.SetParent(ItemNode);
            itembase.transform.localScale = gameObject.transform.localScale;
            itembase.transform.eulerAngles = gameObject.transform.eulerAngles;
            Destroy(gameObject);
        });
    }

    public override MonoBehaviour getMonoBehaviour()
    {
        return this;
    }
}