using System.Collections.Generic;

namespace glsl_babylon.classes
{
    // Interface so the converter can be mocked
    public interface IConverter
    {
        string CheckAndCleanArguments(string a_arguments);
        void Convert(string a_arguments);
        bool ConvertFile(string a_filename);
        string GetOutput(List<string> a_values);
        bool TryConvertFolder(string a_foldername, int a_depth);
    }
}