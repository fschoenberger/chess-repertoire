using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessRepertoire.Model.Variation {
    public interface IVariationGraphRepository {
        VariationGraph GetGraph();
    }
}
