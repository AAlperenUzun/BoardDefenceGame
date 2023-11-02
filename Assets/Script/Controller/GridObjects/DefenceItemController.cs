using System.Linq;
using UnityEngine;

public class DefenceItemController : BaseGridObjectController
{
    public DefenceItem DefenceItem=new DefenceItem();
    public float CurrentIntervalTime;
    public DefenceItemController(GridObjectData data, GridObjectInstanceId instanceId) : base(data, instanceId)
    {
    }
    public void Initialize()
    {
        DefenceItemData defenceItemData = GameController.Instance.defenceItemData;
        DefenceItem = defenceItemData.defenceItems.FirstOrDefault(item => item.defenceItemType == Data.TypeContainer.GridDefenceItemType);
    }

    public override bool Interact()
    {
        return false;
    }

    public void Attack()
    {
        for (int i = 1; i <= DefenceItem.range; i++)
        {
            var pos = new Vector2Int(Data.Position.x,
                Data.Position.y + i);
            GameController.Instance.BoardController.AttackToEnemy(pos, DefenceItem.damage);
        }

        if (DefenceItem.direction == DefenceItem.Direction.All)
        {
            for (int i = 1; i <= DefenceItem.range; i++)
            {
                var pos = new Vector2Int(Data.Position.x - i,
                    Data.Position.y);
                GameController.Instance.BoardController.AttackToEnemy(pos, DefenceItem.damage);
            }

            for (int i = 1; i <= DefenceItem.range; i++)
            {
                var pos = new Vector2Int(Data.Position.x + i,
                    Data.Position.y);
                GameController.Instance.BoardController.AttackToEnemy(pos, DefenceItem.damage);
            }
        }
    }
    protected override void FallCompleted()
    {
        base.FallCompleted();
        
        Vector2Int position = GetPosition();
        if (position.y == 0)
            GameController.Instance.BoardController.TryDestroyGridObject(position);
    }
}