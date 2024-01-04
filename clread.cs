  using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


  namespace CreoPost
{
   
  
  public NclReader()
        {
        }

        public NclReader(string fn)
        {
            ReadNclFromFile(fn);
        }

        public static List<double>? ParseDoubles(string ln)
        {
            // res?
            var res = new List<double>();

            // into items
            var items = ln.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            // loop
            foreach (var it in items)
                if (double.TryParse(it, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out var f))
                    res.Add(f);
                else
                    return null;

            // ok!
            return res;
        }

        public static NclUnit? ParseUnit(string st)
        {
            foreach (var x in (NclUnit[])Enum.GetValues(typeof(NclUnit)))
                if (x.ToString().Equals(st, StringComparison.InvariantCultureIgnoreCase))
                    return x;
            return null;
        }

        public bool ParseLine(int lineno, string raw)
        {
            // cmt?
            var ln = raw;
            var cmt = "";
            var i = ln.IndexOf("$$");
            if (i >= 0)
            {
                cmt = ln.Substring(i + 2);
                ln = ln.Substring(0, i);
            }

            // trimmed
            ln = ln.Trim();

            // error?
            var error = false;
            var found = false;

            // recognize
            NclItemBase item = null;

            // some globals
            var m = Regex.Match(ln, @"^PARTNO\s*/\s*(\w.*)");
            if (m.Success)
            {
                item = new NclItemGlobal();
                Globals.PartNo = m.Groups[1].ToString().Trim();
                found = true;
            }

            m = Regex.Match(ln, @"^MACHIN\s*/\s*([^,]+),\s*(\w.*)");
            if (!found && m.Success)
            {
                item = new NclItemGlobal();
                Globals.PostProc = m.Groups[1].ToString().Trim();
                Globals.MachineNo = m.Groups[2].ToString().Trim();
                found = true;
            }

            m = Regex.Match(ln, @"^UNITS\s*/\s*(\w.*)");
            if (!found && m.Success)
            {
                item = new NclItemGlobal();
                var un = m.Groups[1].ToString().Trim().ToLower();
                if (un == "mm")
                    Globals.Units = NclUnit.MM;
                else if (un == "inch")
                    Globals.Units = NclUnit.Inch;
                else
                {
                    error = true;
                }
                found = true;
            }

            m = Regex.Match(ln, @"^LOADTL\s*/\s*(\d+)$");
            if (!found && m.Success)
            {
                item = new NclItemLoadTool()
                {
                    ToolNo = Convert.ToInt32(m.Groups[1].ToString().Trim())
                };
                found = true;
            }

            m = Regex.Match(ln, @"^SPINDL\s*/\s*RPM,\s*([-0-9+.]+)");
            if (!found && m.Success)
                if (double.TryParse(m.Groups[1].ToString().Trim(), NumberStyles.Float, 
                    CultureInfo.InvariantCulture, out var f))
                {
                    item = new NclItemSpindleRpm()
                    {
                        Rpm = f
                    };
                    found = true;
                }

            m = Regex.Match(ln, @"^SPINDL\s*/\s*OFF$");
            if (!found && m.Success)
            {
                item = new NclItemSpindleOff()
                {
                };
                found = true;
            }

            m = Regex.Match(ln, @"^RAPID$");
            if (!found && m.Success)
            {
                item = new NclItemRapid()
                {
                };
                found = true;
            }

            m = Regex.Match(ln, @"^FINI");
            if (!found && m.Success)
            {
                item = new NclItemFini()
                {
                };
                found = true;
            }

            m = Regex.Match(ln, @"^GOTO\s*/\s*([-0-9+., ]+)");
            if (!found && m.Success)
                if (ParseDoubles(m.Groups[1].ToString().Trim()) is List<double> dbls
                    && dbls.Count == 3)
                {
                    item = new NclItemGoto()
                    {
                        X = dbls[0],
                        Y = dbls[1],
                        Z = dbls[2]
                    };
                    found = true;
                }

            m = Regex.Match(ln, @"^CIRCLE\s*/\s*([-0-9+., ]+)");
            if (!found && m.Success)
                if (ParseDoubles(m.Groups[1].ToString().Trim()) is List<double> dbls
                    && dbls.Count == 7)
                {
                    item = new NclItemCircle()
                    {
                        X = dbls[0],
                        Y = dbls[1],
                        Z = dbls[2],
                        I = dbls[3],
                        J = dbls[4],
                        K = dbls[5],
                        R = dbls[6]
                    };
                    found = true;
                }

            m = Regex.Match(ln, @"^FEDRAT\s*/\s*([-0-9+.]+),\s*(\w+)");
            if (!found && m.Success)
                if (double.TryParse(m.Groups[1].ToString().Trim(), System.Globalization.NumberStyles.Float,
                    CultureInfo.InvariantCulture, out var f))
                {
                    item = new NclItemFeedRate()
                    {
                        FeedRate = f,
                        Unit = ParseUnit(m.Groups[2].ToString().Trim())
                    };
                    found = true;
                }

            m = Regex.Match(ln, @"^CYCLE\s*/\s*DRILL\s*,\s*(.+)");
            if (!found && m.Success)
                if ((new NclArgValueList().Parse(m.Groups[1].ToString())) is NclArgValueList alist)
                {
                    item = new NclItemCycleDrill()
                    {
                        Args = alist
                    };
                    found = true;
                }

            m = Regex.Match(ln, @"^CYCLE\s*/\s*DEEP\s*,\s*(.+)");
            if (!found && m.Success)
                if ((new NclArgValueList().Parse(m.Groups[1].ToString())) is NclArgValueList alist)
                {
                    item = new NclItemCycleDeep()
                    {
                        Args = alist
                    };
                    found = true;
                }

            m = Regex.Match(ln, @"^CYCLE\s*/\s*OFF\s*$");
            if (!found && m.Success)
            {
                item = new NclItemCycleOff();
                found = true;
            }

            // empty
            if (!found && ln.Trim() == "")
            {
                item = new NclItemBlank()
                {
                };
                found = true;
            }

            // finalize item
            if (error || !found)
            {
                item = new NclItemUnknown()
                {
                    RawLine = raw,
                    LineNo = lineno
                };
                Lines.Add(item);
                return false;
            }

            if (item != null)
            {
                // enrich
                item.RawLine = raw;
                item.LineNo = lineno;
                item.Comment = cmt;
                Lines.Add(item);
                return true;
            }

            // hmm?
            return false;
        }

        public void ReadNclFrom(IList<string> lines)
        {
            // now join some lines
            var joined = new List<string>();
            for (int li=0; li < lines.Count; li++)
            {
                var sum = lines[li].TrimEnd('$', ' ');
                while (li < lines.Count && lines[li].TrimEnd().EndsWith("$"))
                {
                    li++;
                    sum += " " + lines[li].TrimStart().TrimEnd('$', ' ');
                }
                joined.Add(sum);
            }

            // parse
            int lineno = 1;
            foreach (var ln in joined)
                ParseLine(lineno++, ln);

            // check for errors
            foreach (var l in Lines)
                if (l is NclItemUnknown niu)
                {
                    ErrorNum++;
                    Log?.Invoke(LogLevel.Error, $"Unknown line ({niu.LineNo}): {niu.RawLine}");
                }

            // ok
            ;
        }

        public void ReadNclFromFile(string fn)
        {
            // first read line by line
               var lines = File.ReadAllLines(fn);
                var sequences = new List<NclSequence>();
                NclSequence currentSequence = null;

                foreach (var line in lines)
                {
                    var parsedLine = ParseLine(line); // Implement ParseLine to return NclItemBase
                    if (parsedLine is NclItemFeatureNo) // Assuming NclItemFeatureNo is a class for FEATNO/
                    {
                        if (currentSequence != null)
                        {
                            sequences.Add(currentSequence);
                        }
                        currentSequence = new NclSequence
                        {
                            FeatureNumber = ((NclItemFeatureNo)parsedLine).FeatureNumber
                        };
                    }
                    currentSequence?.Lines.Add(parsedLine);
                }
                if (currentSequence != null)
                {
                    sequences.Add(currentSequence);
                }
        }

        public void ReadNclFromText(string text)
        {
            // first read line by line
            string? line;
            var lines = new List<string>();
            using (StringReader sr = new StringReader(text))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            // hand over
            ReadNclFrom(lines);
        }

        public void CutSequence(NclSequence sequence, int maxGotoCount)
            {
                int gotoCount = 0;
                for (int i = 0; i < sequence.Lines.Count; i++)
                {
                    if (sequence.Lines[i] is NclItemGoto)
                    {
                        gotoCount++;
                        if (gotoCount > maxGotoCount)
                        {
                            sequence.Lines.RemoveRange(i, sequence.Lines.Count - i);
                            break;
                        }
                    }
                }
            }

    }
}
