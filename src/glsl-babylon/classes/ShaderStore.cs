using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace glsl_babylon.classes
{
    public class ShaderStore
    {
        public static string ShaderStoreSuffix = "Shader";

        // Replace multiple spaces with 1
        // For example the twoBlockComments regex adds spaces 
        // Source: http://stackoverflow.com/questions/206717/how-do-i-replace-multiple-spaces-with-a-single-space-in-c
        private Regex m_multipleSpaces = new Regex("[ ]{2,}");

        public bool AddShaderStoreName { get; set; } = true;
        /// <summary>
        /// If GetOutput should add Environment.NewLine or not
        /// </summary>
        public bool DoMinify { get; set; } = false;

        /// <summary>
        /// Get the babylon shaderstore output
        /// </summary>
        /// <param name="a_values"></param>
        /// <returns></returns>
        public string GetOutput(List<string> a_values, string a_storeName)
        {
            StringBuilder sb = new StringBuilder();

            if (a_storeName != "")
            {
                sb.AppendFormat("BABYLON.Effect.ShadersStore[\"{0}\"]=", a_storeName);
                if (!DoMinify)
                {
                    sb.Append(Environment.NewLine);
                }
            }

            for (int i = 0; i < a_values.Count; i++)
            {
                string value = m_multipleSpaces.Replace(a_values[i], " ");
                // Reason for adding Environment.NewLine is so that when you copy-paste into your editor it'll be on a new line!
                if (i < a_values.Count - 1)
                {
                    if (!DoMinify)
                    {
                        // "glsl"+\n
                        sb.AppendFormat("\"{0}\"+{1}", value, Environment.NewLine);
                    }
                    else
                    {
                        // "glsl"+
                        sb.AppendFormat("\"{0}\"+", value);
                    }
                }
                else
                {
                    // "glsl";
                    sb.AppendFormat("\"{0}\";", value);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the shaderStore name for an input file.
        /// Input: exampleName.fragment.fx
        /// Output: ExampleNameFragmentShader
        /// </summary>
        /// <param name="a_filename"></param>
        /// <returns></returns>
        public string GetShaderStoreName(string a_filename)
        {
            string name = "";

            if (AddShaderStoreName)
            {
                a_filename = a_filename.Split(new char[] { '/', '\\' }).Last();
                string[] parts = a_filename.Split('.');
                // Length - 1 to discard fx since it's replaced by Shader
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    string part = parts[i];

                    // Capitalize the first character
                    name += char.ToUpper(part[0]);

                    if (part.Length > 1)
                    {
                        // Add the rest
                        name += part.Substring(1);
                    }
                }
                name += ShaderStoreSuffix;
            }

            return name;
        }

    }
}
