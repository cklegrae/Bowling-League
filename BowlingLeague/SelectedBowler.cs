using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BowlingLeague
{
    // Represents the currently selected Bowler and his accompanying label.

    static class SelectedBowler
    {
        private static Bowler _bowler;
        private static Label _label;

        public static Bowler bowler { set { _bowler = value; } get { return _bowler; } }
        public static Label label { set { _label = value; } get { return _label; } }
    }
}
