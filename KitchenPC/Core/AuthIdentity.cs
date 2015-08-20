namespace KitchenPC
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Security.Principal;
    using System.Text;

    /// <summary>Represents a user identity within a KitchenPC context.  This identity can be stored locally or within the KitchenPC network.</summary>
    public class AuthIdentity : IIdentity
    {
        private const int MinimumAuthIdentityBytes = 17;

        public AuthIdentity()
        {
        }

        public AuthIdentity(Guid id, string name)
        {
            this.UserId = id;
            this.Name = name;
        }

        public Guid UserId { get; private set; }

        public string Name { get; private set; }

        public string AuthenticationType
        {
            get
            {
                return "KPCAuth";
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return this.UserId != Guid.Empty;
            }
        }

        public static string CreateHash(string value)
        {
            var hasher = MD5.Create();
            byte[] bytes = hasher.ComputeHash(Encoding.Unicode.GetBytes(value));
            return Convert.ToBase64String(bytes);
        }

        public static byte[] Serialize(AuthIdentity identity)
        {
            byte[] g = identity.UserId.ToByteArray();
            byte[] u = Encoding.UTF8.GetBytes(identity.Name);

            return g.Concat(u).ToArray();
        }

        public static AuthIdentity Deserialize(byte[] bytes)
        {
            if (bytes.Length < MinimumAuthIdentityBytes)
            {
                throw new ArgumentException(string.Format("AuthIdentity must be at least {0} bytes.", MinimumAuthIdentityBytes));
            }

            byte[] g = new byte[16];
            byte[] u = new byte[bytes.Length - 16];

            Buffer.BlockCopy(bytes, 0, g, 0, 16);
            Buffer.BlockCopy(bytes, 16, u, 0, u.Length);

            return new AuthIdentity(
               new Guid(g),
               Encoding.UTF8.GetString(u));
        }

        public override string ToString()
        {
            if (this.IsAuthenticated)
            {
                return string.Format("{0} ({1})", this.Name, this.UserId.ToString());
            }
            else
            {
                return "<Anonymous>";
            }
        }
    }
}