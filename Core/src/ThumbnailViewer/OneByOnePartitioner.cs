using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using System;

namespace Oog {
  public class OneByOnePartitioner<T> : Partitioner<T> {
    private IEnumerable<T> source;

    public OneByOnePartitioner(IEnumerable<T> source) {
      this.source = source;
    }

    public override bool SupportsDynamicPartitions {
      get { return true; }
    }

    public override IEnumerable<T> GetDynamicPartitions() {
      return new OneByOneEnumerable<T>(source);
    }

    public override IList<IEnumerator<T>> GetPartitions(int partitionCount) {
      throw new NotImplementedException();
    }


    private class OneByOneEnumerable<U> : IEnumerable<U> {
      private IEnumerator<U> source;
      private bool enumerationFinished;

      object enumerateLock;

      public OneByOneEnumerable(IEnumerable<U> source) {
        this.source = source.GetEnumerator();
        enumerationFinished = false;
        enumerateLock = new object();
      }

      public IEnumerator<U> GetEnumerator() {
        while (true) {
          U obj;
          lock (enumerateLock) {
            var b = this.source.MoveNext();
            if (b == false) {
              yield break;
            }
            obj = this.source.Current;
          }
          yield return obj;
        }
      }

      IEnumerator IEnumerable.GetEnumerator() {
        return (this as IEnumerable<T>).GetEnumerator();
      }
    }
  }
}
