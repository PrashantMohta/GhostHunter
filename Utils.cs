using System.Globalization;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace GhostHunter
{
    public static class Utils{

        internal static string Str(float f, string format="0.00")
        {
            return f.ToString(format, CultureInfo.InvariantCulture);
        }
        internal static string newOrEmpty(string oldVal, string newVal)
        {
            if (oldVal == newVal) return "";
            return newVal;
        }
        internal static float ParseFloat(string f,float defaultValue = 0.0f)
        {
            if(f.Length == 0) return defaultValue;
            return float.Parse(f, CultureInfo.InvariantCulture);
        }

        internal static Vector3 ZeroVector = new Vector3(0,0,0);

        internal static void ExtractFile(string path,string file){
            Assembly asm = Assembly.GetExecutingAssembly();
            foreach (string res in asm.GetManifestResourceNames())
            {   
                if(res.EndsWith(file)) {
                    using (Stream s = asm.GetManifestResourceStream(res))
                    {
                            if (s == null) continue;
                            var buffer = new byte[s.Length];
                            s.Read(buffer, 0, buffer.Length);
                            File.WriteAllBytes(Path.Combine(path,file),buffer);
                            s.Dispose();
                    }
                } 
            }
        }
        internal static Vector3 getColliderCenter(this GameObject Parent){
            var collider = Parent.GetComponent<Collider2D>();
            if(collider != null){
                return collider.bounds.center;
            } 
            return ZeroVector;
        }

    }
}