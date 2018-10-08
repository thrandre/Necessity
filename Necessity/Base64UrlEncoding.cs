using System;
using System.Text;

namespace Necessity
{
    public static class Base64UrlEncoding
    {
        public static string Encode(byte[] arg)
        {
            string s = Convert.ToBase64String(arg); // Standard base64 encoder

            s = s.Split('=')[0]; // Remove any trailing '='s
            s = s.Replace('+', '-'); // 62nd char of encoding
            s = s.Replace('/', '_'); // 63rd char of encoding

            return s;
        }

        public static byte[] Decode(string arg)
        {
            string s = arg;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding

            switch (s.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: s += "=="; break; // Two pad chars
                case 3: s += "="; break; // One pad char
                default: throw new Exception("Illegal base64url string!");
            }

            return Convert.FromBase64String(s); // Standard base64 decoder
        }
    }

    public static class JsonB64UrlEncoding
    {
        public static string Pack<T>(T model, Func<T, string> serializerFunc)
        {
            return serializerFunc(model)
                .Pipe(Encoding.UTF8.GetBytes)
                .Pipe(Base64UrlEncoding.Encode);
        }

        public static T Unpack<T>(string model, Func<string, T> deserializerFunc)
        {
            return model
                .Pipe(Base64UrlEncoding.Decode)
                .Pipe(b => Encoding.UTF8.GetString(b, 0, b.Length))
                .Pipe(deserializerFunc);
        }
    }
}