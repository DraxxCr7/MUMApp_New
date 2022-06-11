using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUMApp.Models
{
    public class csEncrypt
    {
        public string m_Texto;
        public byte m_Accion;
        public double m_Clave;

        public string Texto
        {
            get
            {
                return m_Texto;
            }
            set
            {
                m_Texto = value;
            }
        }

        public string Accion
        {
            get
            {
                return m_Accion.ToString();
            }
            set
            {
                m_Accion = Convert.ToByte(value);
            }
        }

        public string Clave
        {
            get
            {
                return m_Clave.ToString();
            }
            set
            {
                m_Clave = Convert.ToDouble(value);
            }
        }

        public csEncrypt Encrypt
        {
            get
            {
                  return this;
            }
            set 
            {
                this.Encrypt = value;
            }
        }

        public string f_Encrypt()
        {
            long key;
            bool salt;
            string s, ss;

            key = 1234567890; //Or any other positive integer
            key = (long)m_Clave;
            salt = false;

            s = m_Texto;
            ss = StrEncode(s, key, salt);

            return ss;
        }

        public string f_Decrypt()
        {
            long key;
            bool salt;
            string s, ss;

            key = 1234567890; //Or any other positive integer
            key = (long)m_Clave;
            salt = false;

            s = m_Texto;
            ss = StrDecode(s, key, salt);

            return ss;
        }

        public string StrEncode(string s, long key, bool salt)
        {
            long n, k1, k2, k3, k4;
            string ss = "";
            int i;
            string saltvalue = 4.ToString();

            n = s.Length;
            for (int j = 0; j < n; j++)
            {
                var x = " ";
                ss = ss + x;
            }

            long[] sn = new long[n + 1];

            k1 = 11 + (key % 233);
            k2 = 7 + (key % 239);
            k3 = 5 + (key % 241);
            k4 = 3 + (key % 251);

            for (i = 1; i <= n; i++)
            {
                string y = s.Substring(i-1, 1);
                for (int j = 0; j < y.Length; j++)
                {
                    var ansi = Encoding.GetEncoding(1252); // ANSI
                    var unicode = Encoding.Unicode;
                    var unicodeBytes = unicode.GetBytes(y);
                    var bytes = Encoding.Convert(unicode, ansi, unicodeBytes);
                    sn[i] = (long)bytes[j];
                }
            }

            for (i = 2; i <= n ; i++)
            {
                sn[i] = sn[i] ^ sn[i - 1] ^ ((k1 * sn[i - 1]) % 256);
            }
            for (i = (int)n - 1; i >= 1; i += -1)
            {
                sn[i] = sn[i] ^ sn[i + 1] ^ (k2 * sn[i + 1]) % 256;
            }
            for (i = 3; i <= n; i++)
            {
                sn[i] = sn[i] ^ sn[i - 2] ^ (k3 * sn[i - 1]) % 256;
            }
            for (i = (int)n - 2; i >= 1; i += -1)
            {
                sn[i] = sn[i] ^ sn[i + 2] ^ (k4 * sn[i + 1]) % 256;
            }

            StringBuilder sb = new StringBuilder(ss);

            for (i = 1; i <= n; i++)
            {
                ss.Substring(i-1, 1);
                sb[i-1] = Convert.ToChar(sn[i]);
            }
            ss = sb.ToString().Trim();

            return ss;

        }

        public string StrDecode(string s, long key, bool salt)
        {
            long n, k1, k2, k3, k4;
            string ss = "";
            int i;

            n = s.Length;
            for (int j = 0; j < n; j++)
            {
                var x = " ";
                ss = ss + x;
            }
            
            long[] sn = new long[n + 1];

            k1 = 11 + (key % 233);
            k2 = 7 + (key % 239);
            k3 = 5 + (key % 241);
            k4 = 3 + (key % 251);

            for (i = 1; i <= n; i++)
            {
                string v = s.Substring(i - 1, 1);

                for (int j = 0; j < v.Length; j++)
                {
                    var ansi = Encoding.GetEncoding(1252); // ANSI
                    var unicode = Encoding.Unicode;
                    var unicodeBytes = unicode.GetBytes(v);
                    var bytes = Encoding.Convert(unicode, ansi, unicodeBytes);
                    sn[i] = (long)bytes[j];

                }
            }

            for (i = 1; i <= n-2; i++)
            {
                sn[i] = sn[i] ^ sn[i + 2] ^ ((k4 * sn[i + 1]) % 256);
            }

            for (i = (int)n; i >= 3; i += -1)
            {
                sn[i] = sn[i] ^ sn[i - 2] ^ (k3 * sn[i - 1]) % 256;
            }
            for (i = 1; i <= n - 1; i++)
            {
                sn[i] = sn[i] ^ sn[i + 1] ^ (k2 * sn[i + 1]) % 256;
            }
            for (i = (int)n; i >= 2; i += -1)
            {
                sn[i] = sn[i] ^ sn[i - 1] ^ (k1 * sn[i - 1]) % 256;
            }

            StringBuilder sb = new StringBuilder(ss);

            for (i = 1; i <= n; i++)
            {
                ss.Substring(i - 1, 1);
                sb[i - 1] = Convert.ToChar(sn[i]);
            }
            ss = sb.ToString();

            if (salt)
            {
                return ss.Substring(3, ss.Length - 4);
            }
            else
            {
                return ss;
            }
        }
    }
}
