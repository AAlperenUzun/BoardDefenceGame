using System.Linq;
using System.Reflection;
using UnityEngine;

public class EnemyObjectController : BaseGridObjectController
{
    public EnemyObjectController(GridObjectData data, GridObjectInstanceId instanceId) : base(data, instanceId)
    {
    }
    public Enemy _enemy=new Enemy();
    public float currentBlockTime;
    
    public void Initialize()
    {
        EnemyData enemyData = GameController.Instance.enemyData;
        Enemy originalEnemy =
            enemyData.enemies.FirstOrDefault(item => item.enemyType == Data.TypeContainer.GridEnemyType);
        _enemy = CopyEnemy(originalEnemy);
    }
    public Enemy CopyEnemy(Enemy originalEnemy)
    {
        Enemy copy = new Enemy();
        foreach (PropertyInfo prop in originalEnemy.GetType().GetProperties())
        {
            if (prop.CanRead && prop.CanWrite)
            {
                prop.SetValue(copy, prop.GetValue(originalEnemy, null), null);
            }
        }
        foreach (FieldInfo field in originalEnemy.GetType().GetFields())
        {
            field.SetValue(copy, field.GetValue(originalEnemy));
        }
        return copy;
    }

    public override bool Interact()
    {
        StartFall();
        return false;
    }

    public bool TakeDamage(float damage)
    {
        Debug.Log("health"+_enemy.health+ "damage:"+damage);
        _enemy.health -= damage;
     
        if (_enemy.health<1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StartFall()
    {
        GameController.Instance.BoardController.TryDestroyGridObject(new Vector2Int(Data.Position.x,
            Data.Position.y - 1));
        StartFalling(Data.Position);
    }

    protected override void FallCompleted()
    {
        base.FallCompleted();
        
        Vector2Int position = GetPosition();
        if (position.y == 0)
        {
            GameController.Instance.BoardController.TryDestroyGridObject(position);
            GameController.Instance.EndLevel(false);
        }

    }
}