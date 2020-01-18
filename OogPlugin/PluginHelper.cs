using System;
using System.IO;

namespace Oog.Plugin {
  public class PluginHelper {
    public static string GetOogTempFolder() {
      return Path.Combine(Path.GetTempPath(), "OogTemp");
    }
  }
}