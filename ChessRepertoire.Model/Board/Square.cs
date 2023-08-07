using System.ComponentModel.DataAnnotations;

namespace ChessRepertoire.Model.Board;

public record Square([Range(0, 7)] int File, [Range(0, 7)] int Rank)
{
    public bool IsValid()
    {
        return File is >= 0 and < 8 && Rank is >= 0 and < 8;
    }

    public override string ToString() {
        return $"{(char)('A' + File)}{Rank + 1}";
    }
}
