using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class DummySslPolicy {
    public static void Register() {
        System.Net.ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
    }
    public static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
        return true;    //Return True to force the certificate to be accepted.
    }
}