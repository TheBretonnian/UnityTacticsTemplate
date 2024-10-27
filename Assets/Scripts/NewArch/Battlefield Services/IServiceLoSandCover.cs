using UnityEngine;

public interface IServiceLoSandCover
{
    Range GetRangeWithLoS(ITile origin, int distance);

    //Cover
    bool HasCover(IUnit attacker, IUnit defender);
    float GetNormalizedCover(IUnit attacker, IUnit defender); //Returns [0..1]
    bool GetNormalizedCover(IUnit attacker, IUnit defender, out float cover);

    //Line of Sight
    bool HasLos(IUnit attacker, IUnit defender);
    bool HasLos(ITile orig, ITile dest);
}