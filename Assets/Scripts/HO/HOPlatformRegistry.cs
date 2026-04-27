using System.Collections.Generic;
using UnityEngine;

public class HOPlatformRegistry : MonoBehaviour
{
    private static readonly List<HOErasablePlatform> platforms = new List<HOErasablePlatform>();

    public static void Register(HOErasablePlatform platform)
    {
        if (!platforms.Contains(platform))
        {
            platforms.Add(platform);
        }
    }

    public static void Unregister(HOErasablePlatform platform)
    {
        platforms.Remove(platform);
    }

    // solo las que no han sido borradas
    public static List<HOErasablePlatform> getPlatformsNear(Vector3 playerPos, float radio)
    {
        List<HOErasablePlatform> result = new List<HOErasablePlatform>();
        float sqrRadio = radio * radio;

        foreach (var p in platforms)
        {
            if (p == null || p.IsErased) 
            {
                continue;
            }
            if ((p.transform.position - playerPos).sqrMagnitude <= sqrRadio)
            {
                result.Add(p);
            }
        }

        return result;
    }
}