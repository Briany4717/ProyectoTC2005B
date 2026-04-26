using System.Collections.Generic;
using UnityEngine;

public class HOPlatformRegistry : MonoBehaviour
{
    private static readonly List<HOErasablePlatform> platforms = new List<HOErasablePlatform>();

    public static void Register(HOErasablePlatform platform)
    {
        if (!platforms.Contains(platform)) platforms.Add(platform);
    }

    public static void Unregister(HOErasablePlatform platform)
    {
        platforms.Remove(platform);
    }

    /// <summary>
    /// Devuelve plataformas activas (no borradas) dentro de un radio del jugador.
    /// </summary>
    public static List<HOErasablePlatform> GetPlatformsNearPlayer(Vector3 playerPos, float radius)
    {
        List<HOErasablePlatform> result = new List<HOErasablePlatform>();
        float sqrRadius = radius * radius;

        foreach (var p in platforms)
        {
            if (p == null || p.IsErased) continue;
            if ((p.transform.position - playerPos).sqrMagnitude <= sqrRadius)
            {
                result.Add(p);
            }
        }

        return result;
    }
}