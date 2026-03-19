using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure;

public class AssemblyReference
{
    public static readonly System.Reflection.Assembly Assembly = typeof(AssemblyReference).Assembly;
}
