using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Windows.Forms;

namespace Oog {
  class OogTreeNodeComparer : IComparer {
    Regex _regex;

    public OogTreeNodeComparer() {
      // (?<num>[0-9]+)|.
      _regex = new Regex("(?<num>[0-9]+)|.");
    }

    public int Compare(object x, object y) {
      OogTreeNode xNode = x as OogTreeNode;
      OogTreeNode yNode = y as OogTreeNode;

      if (x == null) {
        if (y == null) {
          return 0;
        }
        else {
          return -1;
        }
      }
      else {
        if (y == null) {
          return 1;
        }
        else {
          if (xNode.IsDirectory == true &&
              yNode.IsDirectory == false) {
            return -1;
          }
          else if (xNode.IsDirectory == false &&
                   yNode.IsDirectory == true) {
            return 1;
          }

          string xStr = xNode.Text;
          string yStr = yNode.Text;

          MatchCollection xMatches = _regex.Matches(xStr);
          MatchCollection yMatches = _regex.Matches(yStr);

          Match xMatch;
          Match yMatch;
          long xNum = 0;
          long yNum = 0;
          bool xIsNumber;
          bool yIsNumber;

          for (int i = 0; ; i++) {
            if (xMatches.Count > i) {
              if (yMatches.Count > i) {
                xMatch = xMatches[i];
                yMatch = yMatches[i];

                if (xMatch.Groups["num"].Success &&
                    long.TryParse(xMatch.Value, out xNum)) {
                  xIsNumber = true;
                }
                else {
                  xIsNumber = false;
                }

                if (yMatch.Groups["num"].Success &&
                    long.TryParse(yMatch.Value, out yNum)) {
                  yIsNumber = true;
                }
                else {
                  yIsNumber = false;
                }

                if (xIsNumber) {
                  if (yIsNumber) {
                    int result =  xNum.CompareTo(yNum);
                    if (result == 0) {
                      continue;
                    }
                    return result;
                  }
                  else {
                    int result = string.Compare("0", yMatch.Value.ToLower());
                    if (result == 0) {
                      continue;
                    }
                    return result;
                  }
                }
                else {
                  if (yIsNumber) {
                    int result = string.Compare(xMatch.Value.ToLower(), "0");
                    if (result == 0) {
                      continue;
                    }
                    return result;
                  }
                  else {
                    int result = string.Compare(xMatch.Value.ToLower(), yMatch.Value.ToLower());
                    if (result == 0) {
                      continue;
                    }
                    return result;
                  }
                }
              }
              else {
                return 1;
              }
            }
            else {
              if (yMatches.Count > i) {
                return -1;
              }
              else {
                return 0;
              }
            }
          }
        }
      }
    }
  }
}
