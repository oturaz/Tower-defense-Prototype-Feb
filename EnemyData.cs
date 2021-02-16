public class EnemyData
{
    public int enemyType;
    public int waveIndex;
    public int pathIndex;
    public int quantity;

    public EnemyData(int _waveIndex = 0, int _enemyType = 0, int _pathIndex = 0, int _quantity = 1)
    {
        waveIndex = _waveIndex;
        enemyType = _enemyType;
        pathIndex = _pathIndex;
        quantity = _quantity;
    }
}