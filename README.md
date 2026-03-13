# Spectrum Drift (Unity Playable Prototype)

`Spectrum Drift`는 **상태 기반 흡수 + 루트 선택 + 콤보 유지**를 중심으로 한 3D 러너 프로토타입입니다.

## 구현된 플레이 루프
1. 플레이어 자동 전진
2. 좌우 이동으로 흡수 오브젝트 라인 선택
3. 현재 상태와 같은 자원 흡수 시 질량/콤보/점수 증가
4. 상태 불일치 흡수 또는 장애물 접촉 시 질량/점수 감소
5. 게이트 통과로 상태 전환
6. 제한 시간 종료 또는 피니시 존 도달 시 정산

---

## 포함된 스크립트

### Core
- `ElementState`: BlueCalm / RedHeat / GreenRecover / YellowOvercharge

### Data
- `StageBalanceConfig`: 질량, 콤보, 점수, 상태별 보정치 데이터

### Gameplay
- `RunnerController`: 자동 전진 + 모바일 드래그/키보드 좌우 이동
- `PlayerStateController`: 상태/질량/점수/콤보/붕괴(패배) 처리
- `AbsorbableObject`: 상태 자원 흡수 트리거
- `GateZone`: 상태 전환 게이트(Calm/Heat/Recover/Overcharge + 확장 슬롯)
- `HazardZone`: 중성 장애물(질량/점수 감소)
- `FinishZone`: 피니시 트리거
- `StageGameManager`: 스테이지 타이머, 클리어/실패 판정, 최종 점수 산출
- `LaneSpawnController`: 러너 전방에 오브젝트 라인 자동 생성
- `AutoDestroyBehindRunner`: 러너 뒤 오브젝트 자동 제거

---

## 빠른 씬 구성 (10분 세팅)

### 1) 플레이어
- Capsule(또는 Sphere) 생성 후 이름 `Player`
- `RunnerController`, `PlayerStateController` 추가
- Collider + Rigidbody(kinematic 권장) 구성
- `PlayerStateController.runner`에 같은 오브젝트의 `RunnerController` 연결

### 2) 밸런스 데이터
- `Create > Spectrum Drift > Stage Balance Config` 생성
- `PlayerStateController.balance` 슬롯에 연결

### 3) 자원 프리팹 4종
- Blue/Red/Green/Yellow 오브젝트 프리팹 생성
- 각 프리팹에 `AbsorbableObject` 추가 후 `Resource State` 지정
- 공통으로 `AutoDestroyBehindRunner` 추가

### 4) 장애물 프리팹
- 중성 장애물 프리팹 생성
- `HazardZone` + `AutoDestroyBehindRunner` 추가

### 5) 스포너
- 빈 오브젝트 `Spawner` 생성 후 `LaneSpawnController` 추가
- absorbPrefabs 배열에 4종 자원 프리팹 연결
- hazardPrefab에 장애물 프리팹 연결
- runnerTarget에 Player Transform 연결

### 6) 게이트
- Trigger Collider 오브젝트 생성 후 `GateZone` 추가
- GateType 설정(Calm/Heat/Recover/Overcharge)

### 7) 게임 매니저
- 빈 오브젝트 `GameManager` 생성 후 `StageGameManager` 추가
- runner/player 연결
- 필요 시 `FinishZone.onReachedFinish -> StageGameManager.FinishStage` 바인딩

---

## 점수/상태 규칙 요약
- 상태 일치 흡수: 질량 증가 + 콤보 증가 + 점수 증가
- 상태 불일치 흡수: 질량 감소 + 점수 패널티 + 콤보 초기화
- RedHeat: 전진 속도 증가 + 시간당 질량 감소
- GreenRecover: 상태 전환 즉시 소량 질량 회복
- YellowOvercharge: 흡수 점수 배수 추가
- 질량이 최소 임계치 이하: 플레이어 붕괴(실패)

---

## 다음 확장 추천
- Split Gate 실제 분신 로직
- Magnet Gate 자동 흡수 반경 시스템
- UI/HUD(TextMeshPro): 상태 아이콘, 콤보 타이머, 남은 시간
- 스테이지 데이터 기반 연출 패턴
