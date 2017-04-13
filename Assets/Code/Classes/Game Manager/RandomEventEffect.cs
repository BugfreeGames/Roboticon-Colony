using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a non-permanent effect on the resources produced by a tile.
/// </summary>
public class RandomEventEffect
{
    private ResourceGroup resourceEffects;
    private int turnsRemaining;
    private GameObject perpetualVisualEffect;   //Optional effect to play while this RandomEventEffect is active.

    private GameObject visualEffectInWorld;

    public RandomEventEffect(ResourceGroup effect, int turns)
    {
        resourceEffects = effect;
        turnsRemaining = turns;
    }

    public bool IsFinished()
    {
        return turnsRemaining == 0;
    }

    public ResourceGroup GetEffects()
    {
        return resourceEffects;
    }

    public void SetVisualEffect(GameObject effect)
    {
        perpetualVisualEffect = effect;
    }

    public void InstantiateVisualEffect(Vector3 position)
    {
        if(perpetualVisualEffect != null)
        {
            visualEffectInWorld = (GameObject)GameObject.Instantiate(perpetualVisualEffect, position, Quaternion.identity);
            visualEffectInWorld.name = Random.Range(1, 1000).ToString();
        }
    }

    public void EndEffect()
    {
        if(visualEffectInWorld != null)
        {
                MonoBehaviour.Destroy(visualEffectInWorld);
        }
    }

    public void Tick()
    {
        turnsRemaining--;
        if(IsFinished())
        {
            EndEffect();
        }
    }

    /// <summary>
    /// Returns a copy of this RandomEventEffect
    /// </summary>
    /// <returns></returns>
    public RandomEventEffect Copy()
    {
        RandomEventEffect copy = new RandomEventEffect(resourceEffects, turnsRemaining);
        copy.SetVisualEffect(perpetualVisualEffect);

        return copy;
    }
}
