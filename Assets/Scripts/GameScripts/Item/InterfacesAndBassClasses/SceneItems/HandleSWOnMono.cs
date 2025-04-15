using System;
using UnityEngine;

public class HandleSWOnMono : BaseSWMono
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
        GameItemTool.GenerateItemAtTransform(220013, gameObject.transform.position, false, itembase =>
        {
            gameObject.SetActive(false);
            itembase.transform.SetParent(ItemNode);
            itembase.transform.localScale = gameObject.transform.localScale;
            itembase.transform.eulerAngles = gameObject.transform.eulerAngles;
            Destroy(gameObject);
        });
    }

    public override void OnPlayerInteract()
    {
        base.OnPlayerInteract();
        if (!IsTrue)
        {
            GameRunTimeData.Instance.CharacterBasicStat.GetStat().CurrentHp -= 9999;
        }
    }

    public override MonoBehaviour getMonoBehaviour()
    {
        return this;
    }
}