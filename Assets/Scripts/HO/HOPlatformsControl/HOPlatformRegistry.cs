using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mantiene un registro centralizado de las plataformas borrables activas en la escena.
/// </summary>
public class HOPlatformRegistry : MonoBehaviour
{
    private static readonly List<HOErasablePlatform> platforms = new List<HOErasablePlatform>();

    /// <summary>
    /// Añade una plataforma al registro.
    /// </summary>
    public static void Register(HOErasablePlatform platform)
    {
        if (!platforms.Contains(platform))
        {
            platforms.Add(platform);
        }
    }

    /// <summary>
    /// Elimina una plataforma del registro.
    /// </summary>
    public static void Unregister(HOErasablePlatform platform)
    {
        platforms.Remove(platform);
    }

    /// <summary>
    /// Obtiene una lista de plataformas cercanas a una posición dada dentro de un radio.
    /// </summary>
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