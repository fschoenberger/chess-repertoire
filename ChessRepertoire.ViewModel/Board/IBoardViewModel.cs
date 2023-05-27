﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using ChessRepertoire.Model.Board;
using ReactiveUI;

namespace ChessRepertoire.ViewModel.Board;

public interface IBoardViewModel : IReactiveObject
{
    public IEnumerable<IFieldViewModel> Fields { get; }

    public ReadOnlyObservableCollection<IPieceViewModel> Pieces { get; }

    public Color Orientation { get; set; }

    public ICommand FlipBoard { get; }
}