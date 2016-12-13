
namespace DropshipBusiness.Security 
{
    public interface IEncryptionService 
    {
        /// <summary>
        /// Create salt key
        /// </summary>
        /// <param name="size">Key size</param>
        /// <returns>Salt key</returns>
        string CreateSaltKey(int size);

        /// <summary>
        /// Create a password hash
        /// </summary>
        /// <param name="password">{assword</param>
        /// <param name="saltkey">Salk key</param>
        /// <param name="passwordFormat">Password format (hash algorithm)</param>
        /// <returns>Password hash</returns>
        string CreatePasswordHash(string password, string saltkey, string passwordFormat = "SHA1");

    }
}
