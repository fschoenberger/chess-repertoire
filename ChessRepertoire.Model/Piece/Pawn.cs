﻿namespace ChessRepertoire.Model.Piece;

public class Pawn : Piece {
    public Pawn(Color color) {
        Color = color;
    }

    public override void Accept(IPieceVisitor visitor) {
        visitor.Visit(this);
    }

    public override T Accept<T>(IPieceVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}