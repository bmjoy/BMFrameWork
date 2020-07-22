using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DamageTextController
{
    [SerializeField] private DamageText floatingTextPrototype = null;
    [SerializeField] private int preinstantiatedCount = 20;

    private readonly List<DamageText> activeTexts = new List<DamageText>();

    public void Initialize()
    {
        GameObjectPool.PreInstantiate(floatingTextPrototype.gameObject, preinstantiatedCount);
    }

    public void Deinitialize()
    {
        for (int i = activeTexts.Count - 1; i >= 0; --i)
        {
            GameObjectPool.Return(activeTexts[i], true);
            GameObject.Destroy(activeTexts[i]);
        }

        activeTexts.Clear();
    }

    public void Create(Vector3 position, int damage, bool friendly, Transform target = null)
    {
        GameObject damageTextObject = GameObject.Instantiate(floatingTextPrototype.gameObject, position, Quaternion.identity);
        DamageText damageText = damageTextObject.GetComponent<DamageText>();
        damageText.target = target;
        damageText.oldPos = position;
        damageText.flyLenth = 0.0f;
        damageText.Setup(damage, friendly);
        activeTexts.Add(damageText);
    }

    public void DoUpdate(float deltaTime)
    {
        for (int i = activeTexts.Count - 1; i >= 0; --i)
        {
            if (activeTexts[i].DoUpdate(deltaTime))
            {
                GameObjectPool.Return(activeTexts[i], false);
                activeTexts.RemoveAt(i);
            }
        }
    }
}