using System;
using System.IO;

namespace Oog.Plugin {
    public interface IExtractor2 : IExtractor {
      bool SynchronizationRequired { get; }
    }
}
