﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Calamari.Integration.Certificates;
using NUnit.Framework;

namespace Calamari.Tests.Helpers.Certificates
{
    public class SampleCertificate
    {
        private readonly string fileName;

        public const string CngPrivateKeyId = "CngWithPrivateKey";
        public static readonly SampleCertificate CngWithPrivateKey = new SampleCertificate("cng_privatekey_password.pfx", "password", "CFED1F749A4D441F74F8D7345BA77C640E2449F0", true);

        public const string CapiWithPrivateKeyId = "CapiWithPrivateKey";
        public static readonly SampleCertificate CapiWithPrivateKey = new SampleCertificate("capi_self_signed_privatekey_password.pfx", "Password01!", "D8BA5D8760D74AE871C018B1B276602E8B4CFD76", true);

        public const string CapiWithPrivateKeyNoPasswordId = "CngWithPrivateKeyNoPassword";
        public static readonly SampleCertificate CapiWithPrivateKeyNoPassword = new SampleCertificate("capi_self_signed_privatekey_no_password.pfx", null, "E7D0BF9F1A62AED35BA22BED80F9795012A53636", true);

        public const string FabrikamNoPrivateKeyId = "FabrikamNoPrivateKey";
        public static readonly SampleCertificate FabrikamNoPrivateKey = new SampleCertificate("fabrikam_no_private_key_password.pfx", "password", "00567BA9BA571F9F53278067DB8C0871A72A3C51", false);

        public static readonly IDictionary<string, SampleCertificate> SampleCertificates = new Dictionary<string, SampleCertificate>
        {
            {CngPrivateKeyId, CngWithPrivateKey},
            {CapiWithPrivateKeyId, CapiWithPrivateKey},
            {CapiWithPrivateKeyNoPasswordId, CapiWithPrivateKeyNoPassword},
            {FabrikamNoPrivateKeyId, FabrikamNoPrivateKey}
        };

        public SampleCertificate(string fileName, string password, string thumbprint, bool hasPrivateKey)
        {
            Password = password;
            Thumbprint = thumbprint;
            HasPrivateKey = hasPrivateKey;
            this.fileName = fileName;
        }

        public string Password { get; set; }
        public string Thumbprint { get; }
        public bool HasPrivateKey { get; }

        public string Base64Bytes()
        {
            return Convert.ToBase64String(File.ReadAllBytes(FilePath));
        }

        public void EnsureCertificateIsInStore(StoreName storeName, StoreLocation storeLocation)
        {
            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadWrite);
            store.Add(LoadAsX509Certificate2());
            store.Close();
        }

        public void EnsureCertificateIsInStore(string storeName, StoreLocation storeLocation)
        {
            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadWrite);
            store.Add(LoadAsX509Certificate2());
            store.Close();
        }

        public void EnsureCertificateNotInStore(StoreName storeName, StoreLocation storeLocation)
        {
            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadWrite);

            EnsureCertificateNotInStore(store);
            store.Close();
        }

        public void EnsureCertificateNotInStore(string storeName, StoreLocation storeLocation)
        {
            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadWrite);

            EnsureCertificateNotInStore(store);
            store.Close();
        }

        private void EnsureCertificateNotInStore(X509Store store)
        {
            var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, Thumbprint, false);

            if (certificates.Count == 0)
                return;

            WindowsX509CertificateStore.RemoveCertificateFromStore(Thumbprint, store.Location, store.Name);
        }

        public void AssertCertificateIsInStore(string storeName, StoreLocation storeLocation)
        {
            Assert.NotNull(GetCertificateFromStore(storeName, storeLocation),
                $"Could not find certificate with thumbprint {Thumbprint} in store {storeLocation}\\{storeName}");
        }

        public X509Certificate2 GetCertificateFromStore(string storeName, StoreLocation storeLocation)
        {
            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadWrite);

            var foundCertificates = store.Certificates.Find(X509FindType.FindByThumbprint, Thumbprint, false);

            return foundCertificates.Count > 0
                ? foundCertificates[0]
                : null;
        }

        X509Certificate2 LoadAsX509Certificate2()
        {
            return new X509Certificate2(FilePath, Password,
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
        }

        string FilePath => TestEnvironment.GetTestPath("Helpers", "Certificates", "SampleCertificateFiles", fileName);

    }
}