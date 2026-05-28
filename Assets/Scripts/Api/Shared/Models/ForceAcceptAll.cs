using UnityEngine.Networking;


/// Clase para aceptar forzosamente todos los certificados de red.

public class ForceAcceptAll : CertificateHandler
{
    
    /// Valida el certificado de red devolviendo siempre verdadero.
    
    /// <param name="certificateData">Los datos del certificado.</param>
    /// <returns>Siempre devuelve true para aceptar cualquier certificado.</returns>
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}
