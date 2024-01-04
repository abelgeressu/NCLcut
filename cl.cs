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
    // Some ACL explanations
    // see: http://bdml.stanford.edu/twiki/pub/Manufacturing/HaasReferenceInfo/V61_GPost_CD_Manual.pdf

    /// <summary>
    /// Base class for all single commands found in a ACL/BCL file.
    /// </summary>
    public class NclItemBase
    {
        public int LineNo;
        public string RawLine;
        public string Comment;
    }

    /// <summary>
    /// Case, if the command is unknown.
    /// </summary>
    public class NclItemUnknown : NclItemBase
    {
    }

    /// <summary>
    /// Line codes a global constant, which would be available in <c>Globals</c>.
    /// </summary>
    public class NclItemGlobal : NclItemBase
    {
    }

    /// <summary>
    /// Blank line
    /// </summary>
    public class NclItemBlank : NclItemBase
    {
    }

    /// <summary>
    /// Command for loading a tool
    /// </summary>
    public class NclItemLoadTool : NclItemBase
    {
        public int ToolNo;
    }

    /// <summary>
    /// Command for switching spindle to a specific RPM
    /// </summary>
    public class NclItemSpindleRpm : NclItemBase
    {
        public double Rpm;
    }

    /// <summary>
    /// Command for switching spindle off
    /// </summary>
    public class NclItemSpindleOff : NclItemBase
    {
    }

    /// <summary>
    /// Command for setting the feedrate for subsequent commands
    /// </summary>
    public class NclItemFeedRate : NclItemBase
    {
        public double FeedRate;
        public NclUnit? Unit;
    }

    /// <summary>
    /// Command for switch to rapid movement mode for subsequent commands
    /// </summary>
    public class NclItemRapid : NclItemBase
    {
    }

    /// <summary>
    /// End of file.
    /// </summary>
    public class NclItemFini : NclItemBase
    {
    }

    /// <summary>
    /// Move to a 3D coordinate
    /// </summary>
    public class NclItemGoto : NclItemBase
    {
        public double X, Y, Z;
    }

    /// <summary>
    /// Base class for CYCLE command.
    /// </summary>
    public class NclItemCycleBase : NclItemBase
    {
        public NclArgValueList Args = new NclArgValueList();
    }

    /// <summary>
    /// Drilling command with one pass.
    /// </summary>
    public class NclItemCycleDrill : NclItemCycleBase
    {
    }

    /// <summary>
    /// Complex deep drilling command including a lot of sub-movements.
    /// </summary>
    public class NclItemCycleDeep : NclItemCycleBase
    {
    }

    /// <summary>
    /// End of cycle.
    /// </summary>
    public class NclItemCycleOff : NclItemBase
    {
    }

    /// <summary>
    /// Circle/ arc command
    /// </summary>
    public class NclItemCircle : NclItemBase
    {
        // see: http://bdml.stanford.edu/twiki/pub/Manufacturing/HaasReferenceInfo/V61_GPost_CD_Manual.pdf
        // Clause 4.8, pp. 66
        // I/J/K are vector of circular normal
        // R is radius

        public double X, Y, Z, I, J, K, R;
    }

    /// <summary>
    /// Single named argument of a command.
    /// </summary>
    public class NclArgValue
    {
        public string Arg = "";
        public double Value = 0.0;
    }

    /// <summary>
    /// Some commands have named arguments in (arbitrary??) sequence.
    /// This is a help class.
    /// </summary>
    public class NclArgValueList : Dictionary<string, NclArgValue>
    {
        public NclArgValueList Parse(string st)
        {
            var its = st.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            for (int i=0; i<its.Length - 1; i+=2)
            {
                var astr = its[i].ToUpper();
                var vstr = its[i + 1];

                if (double.TryParse(vstr, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out var f))
                    this.Add(astr, new NclArgValue() { 
                        Arg = astr, Value = f 
                    });
            }
            return this;
        }

        public double? GetNamedValue(string argName)
        {
            if (!this.ContainsKey(argName))
                return null;
            return this[argName]?.Value;
        }
    }

    /// <summary>
    /// Some coded units.
    /// Note: currently, unit handling is NOT fully supported.
    /// </summary>
    public enum NclUnit { 
        MM, Inch, 
        InchPerMin, InchPerRev, SurfacefeetPerMin,
        MmPerMin, MmPerRev, SurfaceMeterPerMin
    };

    /// <summary>
    /// Global constants given by various commands.
    /// </summary>
    public class NclGlobals
    {
        public string PartNo;
        public string PostProc;
        public string MachineNo;
        public NclUnit Units = NclUnit.MM;
    }

    /// <summary>
    /// Thsi class read a file/ text and holds parsed line commands.
    /// </summary>
    
      
}
