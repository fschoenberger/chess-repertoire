﻿namespace ChessRepertoire.Model.Board;

public record Move(Square From, Square To, ChessPiece? PromotedPiece = null, bool IsCastling = false);