namespace AOS.DataAccessLayer
{
    public class AOSServiceLayerFactory
    {
        public static IAOSServiceLayer GetAOSServiceLayer(string endpointURL, string soVersion)
        {
            endpointURL = endpointURL.TrimEnd('/');

            if (soVersion == "so75")
                return new AOSServiceLayer75(endpointURL, false);
            else if (soVersion == "so75ssl")
                return new AOSServiceLayer75(endpointURL, true);
            else if (soVersion == "so81")
                return new AOSServiceLayer82(endpointURL, false);
            else if (soVersion == "so81ssl")
                return new AOSServiceLayer82(endpointURL, true);
            else if (soVersion == "so82")
                return new AOSServiceLayer82(endpointURL, false);
            else if (soVersion == "so82ssl")
                return new AOSServiceLayer82(endpointURL, true);

            return null;
        }
    }
}
