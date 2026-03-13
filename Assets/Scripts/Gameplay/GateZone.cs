using SpectrumDrift.Core;
using UnityEngine;

namespace SpectrumDrift.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class GateZone : MonoBehaviour
    {
        public enum GateType
        {
            Calm,
            Heat,
            Recover,
            Overcharge,
            Split,
            Magnet
        }

        [SerializeField] private GateType gateType;
        [SerializeField] private bool singleUse = true;

        private bool _used;

        private void Reset()
        {
            Collider col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_used && singleUse)
            {
                return;
            }

            PlayerStateController player = other.GetComponent<PlayerStateController>();
            if (player == null)
            {
                return;
            }

            switch (gateType)
            {
                case GateType.Calm:
                    player.ChangeState(ElementState.BlueCalm);
                    break;
                case GateType.Heat:
                    player.ChangeState(ElementState.RedHeat);
                    break;
                case GateType.Recover:
                    player.ChangeState(ElementState.GreenRecover);
                    break;
                case GateType.Overcharge:
                    player.ChangeState(ElementState.YellowOvercharge);
                    break;
                case GateType.Split:
                    player.ChangeState(player.CurrentState);
                    // TODO: 분신 스폰 / 이중 흡수 로직 추가
                    break;
                case GateType.Magnet:
                    player.ChangeState(player.CurrentState);
                    // TODO: 주변 동일 속성 자동 흡수 효과 추가
                    break;
            }

            _used = true;
        }
    }
}
