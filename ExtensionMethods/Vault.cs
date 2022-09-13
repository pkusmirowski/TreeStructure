using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace TreeStructure.ExtensionMethods
{
    public static class Vault
    {
        public static string GetSecretPhrase(string key)
        {
            const string keyVaultUrl = "https://bigsecrets.vault.azure.net/";
            var credential = new DefaultAzureCredential();
            var client = new SecretClient(vaultUri: new Uri(keyVaultUrl), credential);
            KeyVaultSecret secret = client.GetSecret(key);
            return secret.Value;
        }
    }
}
