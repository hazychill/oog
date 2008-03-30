using System;
using System.IO;

namespace Oog.Plugin {
  public class EmptyExtractor : IExtractor {
    public string[] GetNames() {
      return new string[0];
    }
    public Stream GetData(string name) {
      return null;
    }
    
    public void Close() {}

    public void Dispose() { Close(); }
  }
}
