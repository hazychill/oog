using System;
using System.IO;

namespace Oog.Plugin {
    public interface IExtractor : IDisposable {
        string[] GetNames();
        Stream GetData(string name);
        void Close();
    }
}
