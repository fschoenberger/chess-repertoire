using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace ChessRepertoire.ViewModel.EvaluationBar {
    public interface IEvaluationBarViewModel : IReactiveObject {
        string TopText { get; }

        string BottomText { get; }

        double Percentage { get; }
    }

    public class DesignTimeEvaluationBarViewModel : ReactiveObject, IEvaluationBarViewModel {
        public string TopText => "10.3";
        public string BottomText => "";
        public double Percentage => 0.9;
    }
}
