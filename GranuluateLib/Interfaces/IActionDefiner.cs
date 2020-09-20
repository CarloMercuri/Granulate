using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GranulateLibrary
{
    public interface IActionDefiner
    {
       ActionType ActionCategory { get; set; }


       void RevertAction();

    }
}
