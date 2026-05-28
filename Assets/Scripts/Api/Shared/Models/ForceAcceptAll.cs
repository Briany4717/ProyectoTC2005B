using UnityEngine.Networking;

/// <summary>
/// Clase para aceptar forzosamente todos los certificados de red.
/// </summary>
public class ForceAcceptAll : CertificateHandler
{
    /// <summary>
    /// Valida el certificado de red devolviendo siempre verdadero.
    /// </summary>
    /// <param name="certificateData">Los datos del certificado.</param>
    /// <returns>Siempre devuelve true para aceptar cualquier certificado.</returns>
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}
