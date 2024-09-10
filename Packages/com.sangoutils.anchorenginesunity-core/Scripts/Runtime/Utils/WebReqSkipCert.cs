using UnityEngine.Networking;

namespace SangoUtils.AnchorEngines_Unity.Core.Utils
{
    internal class WebReqSkipCert : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}