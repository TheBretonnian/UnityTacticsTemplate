
//ITarget is used to abstract a target for an ability: 
//typically either a tile or a unit
public interface ITarget  
{
	bool IsUnit {get;}
	IUnit GetUnit{get;}
}