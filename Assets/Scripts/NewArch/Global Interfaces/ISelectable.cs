//ITarget is used to abstract a game object that can be selected: 
//typically either a tile or a unit (preferred)
public interface ISelectable  
{
	void Selected();
	void Deselected();
	void OnHoverEnter();
	void OnHoverExit();
}//end ISelectable