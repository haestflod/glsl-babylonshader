using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using glsl_babylon.classes;

namespace tests.testhelpers.Interfaces
{
    // Mocks the IConverter for testing since Moq didn't want to work!
    public class MockConverter : IConverter
    {
        // I'm sure there is a more effecient way to do this but it was easy & fast to implement!
        public string CheckAndCleanArgumentsResult { get; set; } = "";
        public bool ConvertFileResult { get; set; } = false;
        public string GetOutputResult { get; set; } = "";
        public string GetShaderStoreNameResult { get; set; } = "";
        public bool TryConvertFolderResult { get; set; } = false;

        // Number of times functions were called!
        public int ConvertCalls { get; set; } = 0;
        public int ConvertFileCalls { get; set; } = 0;


        public string CheckAndCleanArguments(string a_arguments)
        {
            return CheckAndCleanArgumentsResult;
        }
        public void Convert(string a_arguments)
        {
            ConvertCalls++;
        }
        public bool ConvertFile(string a_filename)
        {
            ConvertFileCalls++;
            return ConvertFileResult;
        }
        public string GetOutput(List<string> a_values, string a_storeName)
        {
            return GetOutputResult;
        }
        public string GetShaderStoreName(string a_filename)
        {
            return GetShaderStoreNameResult;
        }
        public bool TryConvertFolder(string a_foldername, int a_depth)
        {
            return TryConvertFolderResult;
        }
    }
}
