using Cysharp.Threading.Tasks;

namespace Characters
{
    public class Boss2Script : Enemy
    {
        protected override void Awake()
        {
            base.Awake();
            PerformActionsAsync().Forget();
        }
        
        private async UniTask PerformActionsAsync()
        {
            await UniTask.Delay(1000);
            await FireActionsAsync();
        }
        
        private async UniTask FireActionsAsync()
        {
            if (!this || !gameObject.activeInHierarchy)
            {
                return;
            }
            ShootingController.FireBullet(PlayerDirection);
            await UniTask.Delay(1000);
            await PerformActionsAsync();
        }
    }
}
