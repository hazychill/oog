using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Oog.Plugin {
    public class FolderExtractor : IExtractor2 {
        string directoryPath;
        string[] names;

        internal FolderExtractor(string directoryPath) {
            this.directoryPath = directoryPath;
        }

        #region IExtractor Members

        public string[] GetNames() {
            if (names == null) {
                try {
                    string[] fullNames = Directory.GetFiles(directoryPath);
                    names = Array.ConvertAll<string, string>(fullNames, Path.GetFileName);
                }
                catch {
                    names = new string[0];
                }
            }
            return names;
        }

        public Stream GetData(string name) {
            try {
                string fullName = Path.Combine(directoryPath, name);
                return File.OpenRead(fullName);
            }
            catch {
                return null;
            }
        }

        public void Close() { }

        #endregion

        #region IExtractor2 Members

        public bool SynchronizationRequired {
          get { return false; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
          Dispose(true);
          GC.SuppressFinalize(this);
        }

      protected virtual void Dispose(bool disposing) { }

        #endregion
    }
}
