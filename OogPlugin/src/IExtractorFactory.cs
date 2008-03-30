using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Oog.Plugin {
    public interface IExtractorFactory {
        IExtractor Create(string path);
        string[] SupportedExtensions { get; }
        Image Icon { get; }
    }
}
