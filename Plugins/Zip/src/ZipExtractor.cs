using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace Oog.Plugin {
    public class ZipExtractor : IExtractor {
        ZipFile zipFile;
        string[] names;

        internal ZipExtractor(string path) {
            zipFile = new ZipFile(path);
        }

        #region IExtractor Members

        public void Close() {
            zipFile.Close();
        }

        public System.IO.Stream GetData(string name) {
            ZipEntry entry = zipFile.GetEntry(name);
            try {
                return zipFile.GetInputStream(entry);
            }
            catch {
                return null;
            }
        }

        public string[] GetNames() {
            if (names == null) {
                names = new string[zipFile.Count];
                int i = 0;
                foreach (ZipEntry entry in zipFile) {
                    names[i] = entry.Name;
                    i++;
                }
            }
            return names;
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
            Close();
        }

        #endregion
    }
}
