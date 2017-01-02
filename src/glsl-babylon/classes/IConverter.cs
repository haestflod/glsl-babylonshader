using System.Collections.Generic;

namespace glsl_babylon.classes
{
    // Interface so the converter can be mocked
    public interface IConverter
    {
        void ParseSettings(ConverterSettings a_settings);
        void Convert(string a_arguments);
        bool ConvertFile(string a_filename);       
        bool TryConvertFolder(string a_foldername, int a_depth);
    }
}