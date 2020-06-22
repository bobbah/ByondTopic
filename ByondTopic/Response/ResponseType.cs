namespace ByondTopic.Response
{
    /// <summary>
    /// Dictates the type of response from a BYOND world topic call,
    /// will typically only be a float or a textual ASCII response.
    /// Determined from first bytes in the response packet.
    /// </summary>
    public enum ResponseType
    {
        Unknown,
        Float,
        ASCII
    }
}
