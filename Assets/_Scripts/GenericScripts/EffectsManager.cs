using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;


public enum EffectTrigger
{
    ExplodeBomb,
}

[System.Serializable]
public class Effects : SerializableDictionaryBase<EffectTrigger, GameObject> { }

public class EffectsManager : MonoBehaviour
{
    public Effects effects;

    public static EffectsManager Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

    }

    public void PlayEffect(EffectTrigger effectTrigger, Vector3 position, Vector3 euler, Vector3 scale, Transform parent, Color? color = null)
    {
        var effect = Instantiate(effects[effectTrigger], position, Quaternion.Euler(euler), null);
        effect.transform.localScale = scale;
        effect.transform.parent = parent;

        effect.AddComponent<SelfDestruct>().lifetime = 1.5f;


        if(color != null)
        {
            ParticleSystem.MainModule settings1 = effect.GetComponent<ParticleSystem>().main;
            settings1.startColor = color ?? Color.white;
            foreach (ParticleSystem item in effect.GetComponentsInChildren<ParticleSystem>())
            {
                ParticleSystem.MainModule settings2 = item.main;
                settings2.startColor = color ?? Color.white;
            }
        }


        // Add Special Behavior Here
        switch (effectTrigger)
        {
            case EffectTrigger.ExplodeBomb:

                break;
            default:
                break;

        }
    }
}