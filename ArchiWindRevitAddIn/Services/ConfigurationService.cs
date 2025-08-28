using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace ArchiWindRevitAddIn.Services
{
    public static class ConfigurationService
    {
        private const string REGISTRY_KEY = @"SOFTWARE\NablaFlow\ArchiWindForRevit";
        private const string REGISTRY_KEY_PAT = "PAT";

        private static readonly byte[] pepper = Encoding.UTF8.GetBytes("CF16D875-BCBA-4B1B-80A5-1C85E6CD122F");

        public static void StorePAT(SecureString pat)
        {
            SecureStore(REGISTRY_KEY, REGISTRY_KEY_PAT, pat);
        }

        public static SecureString? RetrievePAT()
        {
            return SecureRetrieve(REGISTRY_KEY, REGISTRY_KEY_PAT);
        }

        public static void DeletePAT()
        {
            using var key = Registry.CurrentUser.CreateSubKey(REGISTRY_KEY);

            if (key is null)
            {
                return;
            }

            key.DeleteValue(REGISTRY_KEY_PAT);
        }

        private static void SecureStore(string keyPath, string value, SecureString token)
        {
            var encryptedToken = ProtectSecureString(token, pepper);

            using var key = Registry.CurrentUser.CreateSubKey(keyPath)
                ?? throw new Exception("failed to write in registry");

            key.SetValue(value, encryptedToken, RegistryValueKind.Binary);
        }

        private static SecureString? SecureRetrieve(string keyPath, string value)
        {
            using var key = Registry.CurrentUser.OpenSubKey(keyPath);

            if (key == null || key.GetValue(value) is not byte[] encryptedToken)
            {
                return null;
            }

            return UnprotectToSecureString(encryptedToken, pepper);
        }

        private static byte[] ProtectSecureString(SecureString secureString, byte[] entrypy)
        {
            if (secureString == null || secureString.Length == 0)
            {
                throw new ArgumentException("SecureString cannot be null or empty");
            }

            IntPtr unmanagedString = IntPtr.Zero;
            byte[]? plaintextBytes = null;

            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);

                if (unmanagedString == IntPtr.Zero)
                {
                    throw new Exception("SecureStringToGlobalAllocUnicode failed");
                }

                int length = secureString.Length * 2;

                plaintextBytes = new byte[length];
                Marshal.Copy(unmanagedString, plaintextBytes, 0, length);

                return ProtectedData.Protect(
                    plaintextBytes,
                    entrypy,
                    DataProtectionScope.CurrentUser
                );
            }
            finally
            {
                if (unmanagedString != IntPtr.Zero)
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
                }

                if (plaintextBytes != null)
                {
                    Array.Clear(plaintextBytes, 0, plaintextBytes.Length);
                }
            }
        }

        private static SecureString UnprotectToSecureString(byte[] encryptedData, byte[] entropy)
        {
            if (encryptedData == null || encryptedData.Length == 0)
            {
                throw new ArgumentException("Encrypted data cannot be null or empty");
            }

            byte[]? decryptedBytes = null;

            try
            {
                decryptedBytes = ProtectedData.Unprotect(
                    encryptedData,
                    entropy,
                    DataProtectionScope.CurrentUser
                );

                if (decryptedBytes.Length % 2 != 0)
                {
                    throw new InvalidDataException("Decrypted data length must be even for UTF-16");
                }

                var secureString = new SecureString();

                for (int i = 0; i < decryptedBytes.Length; i += 2)
                {
                    if (i + 1 < decryptedBytes.Length)
                    {
                        char c = (char)(decryptedBytes[i] | (decryptedBytes[i + 1] << 8));
                        secureString.AppendChar(c);
                    }
                }

                secureString.MakeReadOnly();

                return secureString;
            }
            finally
            {
                if (decryptedBytes != null)
                {
                    Array.Clear(decryptedBytes, 0, decryptedBytes.Length);
                }
            }
        }
    }
}
