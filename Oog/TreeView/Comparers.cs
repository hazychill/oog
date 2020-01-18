using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;

namespace Oog {
  [TypeConverter(typeof(OogTreeNodeComparerConverter))]
  public interface IOogTreeNodeComparer : IComparer { }

  class OogTreeNodeSeparateNaturalNumberComparer : IOogTreeNodeComparer {
    Regex _regex;

    public OogTreeNodeSeparateNaturalNumberComparer() {
      // (?<num>[0-9]+)|.
      _regex = new Regex("(?<num>[0-9]+)|.");
    }

    public int Compare(object x, object y) {
      OogTreeNode xNode = x as OogTreeNode;
      OogTreeNode yNode = y as OogTreeNode;

      if (xNode == null) {
        if (yNode == null) {
          return 0;
        }
        else {
          return -1;
        }
      }
      else {
        if (yNode == null) {
          return 1;
        }
        else {
          return CompareNodes(xNode, yNode);
        }
      }
    }

    private int CompareNodes(OogTreeNode xNode, OogTreeNode yNode) {
      if (xNode.IsDirectory == true &&
          yNode.IsDirectory == false) {
        return -1;
      }
      
      if (xNode.IsDirectory == false &&
          yNode.IsDirectory == true) {
        return 1;
      }

      string xStr = xNode.Text;
      string yStr = yNode.Text;

      MatchCollection xMatches = _regex.Matches(xStr);
      MatchCollection yMatches = _regex.Matches(yStr);

      for (int i = 0; i < Math.Min(xMatches.Count, yMatches.Count); i++) {
        Match xMatch = xMatches[i];
        Match yMatch = yMatches[i];

        int result = CompareMatches(xMatch, yMatch);
        if (result == 0) {
          continue;
        }

        return result;
      }

      if (xMatches.Count > yMatches.Count) {
        return 1;
      }
      else if (yMatches.Count > xMatches.Count) {
        return -1;
      }
      else {
        return 0;
      }
    }

    private int CompareMatches(Match xMatch, Match yMatch) {
      long xNum = 0;
      long yNum = 0;
      bool xIsNumber;
      bool yIsNumber;

      xIsNumber = MatchIsNumber(xMatch, out xNum);
      yIsNumber = MatchIsNumber(yMatch, out yNum);

      if (xIsNumber) {
        if (yIsNumber) {
          return xNum.CompareTo(yNum);
        }
        else {
          return string.Compare("0", yMatch.Value.ToLower());
        }
      }
      else {
        if (yIsNumber) {
          return string.Compare(xMatch.Value.ToLower(), "0");
        }
        else {
          return string.Compare(xMatch.Value.ToLower(), yMatch.Value.ToLower());
        }
      }
    }

    private bool MatchIsNumber(Match match, out long num) {
      if (!match.Groups["num"].Success) {
        num = default(long);
        return false;
      }

      return long.TryParse(match.Value, out num);
    }
  }

  class OogTreeNodeMixedNaturalNumberComparer : IOogTreeNodeComparer {
    Regex _regex;

    public OogTreeNodeMixedNaturalNumberComparer() {
      // (?<num>[0-9]+)|.
      _regex = new Regex("(?<num>[0-9]+)|.");
    }

    public int Compare(object x, object y) {
      OogTreeNode xNode = x as OogTreeNode;
      OogTreeNode yNode = y as OogTreeNode;

      if (xNode == null) {
        if (yNode == null) {
          return 0;
        }
        else {
          return -1;
        }
      }
      else { 
        if (yNode == null) {
          return 1;
        }
        else {
          return CompareNodes(xNode, yNode);
        }
      }
    }

    private int CompareNodes(OogTreeNode xNode, OogTreeNode yNode) {
      string xStr = xNode.Text;
      string yStr = yNode.Text;

      MatchCollection xMatches = _regex.Matches(xStr);
      MatchCollection yMatches = _regex.Matches(yStr);

      for (int i = 0; i < Math.Min(xMatches.Count, yMatches.Count); i++) {
        Match xMatch = xMatches[i];
        Match yMatch = yMatches[i];

        int result = CompareMatches(xMatch, yMatch);
        if (result == 0) {
          continue;
        }

        return result;
      }

      if (xMatches.Count > yMatches.Count) {
        return 1;
      }
      else if (yMatches.Count > xMatches.Count) {
        return -1;
      }
      else {
        return 0;
      }
    }

    private int CompareMatches(Match xMatch, Match yMatch) {
      long xNum = 0;
      long yNum = 0;
      bool xIsNumber;
      bool yIsNumber;

      xIsNumber = MatchIsNumber(xMatch, out xNum);
      yIsNumber = MatchIsNumber(yMatch, out yNum);

      if (xIsNumber) {
        if (yIsNumber) {
          return xNum.CompareTo(yNum);
        }
        else {
          return string.Compare("0", yMatch.Value.ToLower());
        }
      }
      else {
        if (yIsNumber) {
          return string.Compare(xMatch.Value.ToLower(), "0");
        }
        else {
          return string.Compare(xMatch.Value.ToLower(), yMatch.Value.ToLower());
        }
      }
    }

    private bool MatchIsNumber(Match match, out long num) {
      if (!match.Groups["num"].Success) {
        num = default(long);
        return false;
      }

      return long.TryParse(match.Value, out num);
    }
  }

  class OogTreeNodeSepareteComparer : IOogTreeNodeComparer {
    public OogTreeNodeSepareteComparer() { }

    public int Compare(object x, object y) {
      OogTreeNode xNode = x as OogTreeNode;
      OogTreeNode yNode = y as OogTreeNode;

      if (xNode == null) {
        if (yNode == null) {
          return 0;
        }
        else {
          return -1;
        }
      }
      else {
        if (yNode == null) {
          return 1;
        }
        else {
          return CompareNodes(xNode, yNode);
        }
      }
    }

    private int CompareNodes(OogTreeNode xNode, OogTreeNode yNode) {
      if (xNode.IsDirectory == true &&
          yNode.IsDirectory == false) {
        return -1;
      }
      
      if (xNode.IsDirectory == false &&
          yNode.IsDirectory == true) {
        return 1;
      }

      string xStr = xNode.Text;
      string yStr = yNode.Text;

      if (xStr == null) {
        if (yStr == null) {
          return 0;
        }
        else {
          return -1;
        }
      }
      else {
        if (yStr == null) {
          return 1;
        }
        else {
          return string.Compare(xStr, yStr, StringComparison.OrdinalIgnoreCase);
        }
      }
    }
  }

  class OogTreeNodeMixedComparer : IOogTreeNodeComparer {
    public OogTreeNodeMixedComparer () { }

    public int Compare(object x, object y) {
      OogTreeNode xNode = x as OogTreeNode;
      OogTreeNode yNode = y as OogTreeNode;

      if (xNode == null) {
        if (yNode == null) {
          return 0;
        }
        else {
          return -1;
        }
      }
      else {
        if (yNode == null) {
          return 1;
        }
        else {
          return CompareNodes(xNode, yNode);
        }
      }
    }

    private int CompareNodes(OogTreeNode xNode, OogTreeNode yNode) {
      string xStr = xNode.Text;
      string yStr = yNode.Text;

      if (xStr == null) {
        if (yStr == null) {
          return 0;
        }
        else {
          return -1;
        }
      }
      else {
        if (yStr == null) {
          return 1;
        }
        else {
          return string.Compare(xStr, yStr, StringComparison.OrdinalIgnoreCase);
        }
      }
    }
  }

  class OogTreeNodeComparerConverter : TypeConverter {
    private static IDictionary<string, IOogTreeNodeComparer> comparerMap;

    static OogTreeNodeComparerConverter() {
      comparerMap = new Dictionary<string, IOogTreeNodeComparer>();

      comparerMap.Add("OogTreeNodeSeparateNaturalNumberComparer", new OogTreeNodeSeparateNaturalNumberComparer());
      comparerMap.Add("OogTreeNodeMixedNaturalNumberComparer", new OogTreeNodeMixedNaturalNumberComparer());
      comparerMap.Add("OogTreeNodeSepareteComparer", new OogTreeNodeSepareteComparer());
      comparerMap.Add("OogTreeNodeMixedComparer", new OogTreeNodeMixedComparer());
    }

    public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
      return true;
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
      if (sourceType == typeof(string)) {
        return true;
      }
      else {
        return base.CanConvertFrom(context, sourceType);
      }
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
      string name = value as string;
      if (name == null) {
        throw new NotSupportedException();
      }

      IOogTreeNodeComparer comparer;
      if (comparerMap.TryGetValue(name, out comparer)) {
        return comparer;
      }
      else {
        throw new NotSupportedException();
      }
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
      if (destinationType == typeof(string)) {
        return true;
      }
      else {
        return base.CanConvertTo(context, destinationType);
      }
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
      if (!CanConvertTo(context, destinationType)) {
        throw new NotSupportedException();
      }

      IOogTreeNodeComparer comparer = value as IOogTreeNodeComparer;
      if (comparer == null) {
        throw new NotSupportedException();
      }

      return comparer.GetType().Name;
    }

    public override bool IsValid(ITypeDescriptorContext context, object value) {
      string name = value as string;
      if (name == null) {
        return false;
      }

      if (comparerMap.ContainsKey(name)) {
        return true;
      }
      else {
        return false;
      }
    }

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
      return new StandardValuesCollection(comparerMap.Values.ToList());
    }

    public override bool GetStandardValuesExclusive (ITypeDescriptorContext context) {
      return true;
    }
  }
}
