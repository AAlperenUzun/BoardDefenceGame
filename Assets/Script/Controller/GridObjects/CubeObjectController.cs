public class CubeObjectController : BaseGridObjectController
{
    public CubeObjectController(GridObjectData data, GridObjectInstanceId instanceId) : base(data, instanceId)
    {
    }

    public override bool Interact()
    {
        // return GameController.Instance.BoardController.TryDestroyConnectedObjectsAtPosition(Data.Position);
        if (Data.Position.y<4)
        {
            GameController.Instance.BoardController.TryAddDefenceItem(Data.Position, Data);
            return true;
        }
        else
        {
            return false;
        }
    }
}