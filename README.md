# Spectrum Drift (Unity Prototype)

`Spectrum Drift`는 자동 전진 러너 장르에 **상태(State) 기반 흡수 시스템**을 결합한 게임 프로토타입입니다.

## 핵심 플레이
- 플레이어는 자동 전진
- 좌우 이동으로 흡수 루트 선택
- 현재 상태와 맞는 자원을 흡수해 콤보 유지
- 상태 전환 게이트를 통해 플레이스타일 전환
- 질량(몸집), 콤보, 상태 선택으로 점수 극대화

## 현재 구현 범위
이 저장소는 빠른 프로토타이핑을 위한 핵심 시스템 스크립트를 포함합니다.

- `RunnerController` : 자동 전진 + 드래그/키보드 기반 좌우 이동
- `PlayerStateController` : 상태/질량/콤보/점수 계산
- `AbsorbableObject` : 상태 속성 오브젝트 충돌 처리
- `GateZone` : 상태 전환 게이트 + 보조 효과
- `StageBalanceConfig` : 밸런스 조정용 ScriptableObject

## Unity 구성 가이드
1. 빈 씬 생성 후 Player 오브젝트에 다음 컴포넌트 부착:
   - `RunnerController`
   - `PlayerStateController`
2. 흡수 오브젝트 프리팹에 `AbsorbableObject` 추가 후 `Resource State` 지정.
3. 게이트 오브젝트(Trigger Collider)에 `GateZone` 추가.
4. `StageBalanceConfig` 에셋 생성 후 `PlayerStateController`의 `Balance` 슬롯 연결.

## 조작
- 모바일: 드래그 좌우
- PC 테스트: `A/D` 또는 `←/→`

## 확장 추천
- Split Gate 시 분신 시스템 추가
- Magnet Gate 시 범위 내 자동 흡수
- 스테이지 데이터 기반 스폰러너 제작
- UI: 상태 아이콘, 콤보 타이머, 게이트 예고
