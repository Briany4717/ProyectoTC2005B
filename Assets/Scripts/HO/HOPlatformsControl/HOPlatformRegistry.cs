using System.Collections.Generic;
using UnityEngine;


/// Mantiene un registro centralizado de las plataformas borrables activas en la escena.

public class HOPlatformRegistry : MonoBehaviour
{
    private static readonly List<HOErasablePlatform> platforms = new List<HOErasablePlatform>();

    
    /// Añade una plataforma al registro.
    
    public static void Register(HOErasablePlatform platform)
    {
        if (!platforms.Contains(platform))
        {
            platforms.Add(platform);
        }
    }

    
    /// Elimina una plataforma del registro.
    
    public static void Unregister(HOErasablePlatform platform)
    {
        platforms.Remove(platform);
    }

    
    /// Obtiene una lista de plataformas cercanas a una posición dada dentro de un radio.
    
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