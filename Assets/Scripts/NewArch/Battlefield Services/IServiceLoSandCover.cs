using System.Collections.Generic;
using UnityEngine;

public interface IServiceLoSandCover
{
    Range GetRangeWithLoS(ITile origin, int distance);

    //Line of Sight
    bool HasLos(IUnit attacker, IUnit defender);
    bool HasLos(IUnit attacker, IUnit defender, out List<ICover> interferingCovers, out List<IUnit> interferingUnits);
    //Methods with tiles are more suitable to get ranges and visualization jobs, therefore interfering objects may be interesting for player feedback
    bool HasLos(ITile orig, ITile dest);
    bool HasLos(ITile orig, ITile dest, out List<ICover> interferingCovers, out List<IUnit> interferingUnits);

    //Cover (TO DO: with unit or tiles?)
    bool HasCover(IUnit attacker, IUnit defender); 
    //Can this method use other strategies different of Raycasting to complement it and consider edge cases? (Also to be more performant)
    //This method can be called from the other cover method as well as second check: Raycast + rules

    //Cover calculator from information from HasLos (Pre-requisite)
    float GetNormalizedCover(List<ICover> interferingCovers);

    //Compact methods which removes need of calling HasLos (to be used directly in Abilities)
    bool GetNormalizedCoverIfLos(IUnit attacker, IUnit defender, out float cover);
    //Most Compact: return value = HasLos, Cover in out parameters together with list of interfering objects
    bool GetNormalizedCoverIfLoS(IUnit attacker, IUnit defender, out float cover, out List<ICover> interferingCovers, out List<IUnit> interfingUnits);

    //RULES:

    //All mentions object in LoS trayectory (Obstacle = Cover which blocks line of sight)
    //1. Cover adjacent to the attacker does not count for cover purpose AT ALL (not consider in multiple covers rule)
    //2. Interference of units is controlled in concrete class (use case)
    //3. Cover adjacent to the defender AND Distance(Attacker,Cover) < Distance(Attacker, Defender) counts as COVER even if not interfering in LoS 
    //   (EdgeCase: LoS in corner of square tile)
    //4. For multiple covers:
    //    a) Combine Cover Value (Simple addition or reduced weight for covers not adjacent to the defender)
    //    b) Max Value
    //    c) Ignore covers in between (not recommended at all => very unrealistc)
    //    d) Force LoS = false (not recommended => not intuitive for player unless a lot of visual indicators => rise complexity without added value)
    //5. Interfering units: Ignore ally if adjacent to the attacker => Kind of Universal rule, can be placed on concrete Service
    //6. Interfering units: Reduce accuracy and assign random probability to become target of the ability instead of defender 
    //    => Use case logic so it must happen in Ability since it's more volatile. It can change from ability to ability.


}

public interface ICover{}