# Spectrum Drift (Unity Full Prototype)

`Spectrum Drift`는 자동 전진 러너 위에 **상태 전환 + 루트 선택 + 콤보 유지**를 얹은 프로토타입입니다.
이 버전은 단순 시스템 샘플이 아니라, 씬에 붙이면 한 판이 끝까지 도는 **풀 플레이 루프**를 제공합니다.

## 구현된 게임 루프
1. 자동 전진 + 좌우 조작
2. 상태별 자원 흡수 / 불일치 패널티
3. 콤보 기반 점수 배수
4. 게이트로 상태 전환 또는 특수 능력 획득
5. 장애물 회피 및 질량 유지
6. 타이머 종료(또는 피니시존)로 클리어, 질량 붕괴 시 실패
7. 최종 점수 계산 + 재시작

---

## 스크립트 구성

### Core
- `ElementState` : BlueCalm / RedHeat / GreenRecover / YellowOvercharge

### Data
- `StageBalanceConfig` : 질량/점수/콤보/상태 효과/게이트 능력 수치
- `StageLayoutConfig` : 세그먼트 기반 스폰 패턴(선호 상태, 위험도, 게이트 빈도)

### Gameplay
- `RunnerController` : 자동 전진 + 터치/마우스 드래그/키보드 좌우 이동 + Run/Stop
- `PlayerStateController` : 상태/질량/콤보/점수 + 붕괴(패배) + Split/Magnet 능력
- `AbsorbableObject` : 흡수 처리 + 자기장 자동 흡수 대응
- `GateZone` : 상태 게이트 + Split/Magnet 능력 게이트
- `HazardZone` : 중성 장애물 페널티
- `FinishZone` : 피니시 트리거
- `LaneSpawnController` : 전방 세그먼트 동적 생성 + 게이트 랜덤 배치
- `AutoDestroyBehindRunner` : 지나간 오브젝트 정리
- `StageGameManager` : 타이머, 클리어/실패, 최종 점수, R키 재시작
- `SimpleHudPresenter` : 상태/질량/콤보/점수/시간/능력/UI 결과 표시
- `WebGameBootstrap` : WebGL 프레임/백그라운드 실행 기본 세팅

---

## 15분 씬 세팅

### 1) Player
- Sphere/Capsule 생성 (`Player`)
- 컴포넌트: `Rigidbody(kinematic)`, `Collider`, `RunnerController`, `PlayerStateController`

### 2) 데이터
- `Create > Spectrum Drift > Stage Balance Config`
- `Create > Spectrum Drift > Stage Layout Config`
  - 예시 세그먼트:
    - 0~8: Blue 위주, hazard 0.1
    - 9~16: Red 위주, hazard 0.25
    - 17~24: Green 위주, gate 증가
    - 25+: mixed

### 3) 흡수체 프리팹 4종
- Blue/Red/Green/Yellow 프리팹 생성
- 각 프리팹: `Collider(isTrigger)`, `AbsorbableObject`, `AutoDestroyBehindRunner`
- `AbsorbableObject.ResourceState` 지정

### 4) 장애물 프리팹
- 장애물 프리팹: `Collider(isTrigger)`, `HazardZone`, `AutoDestroyBehindRunner`

### 5) 게이트 프리팹
- Trigger Collider 오브젝트 + `GateZone`
- gateType별로 프리팹 여러 개 준비:
  - Calm / Heat / Recover / Overcharge / Split / Magnet

### 6) Spawner
- 빈 오브젝트 `Spawner` + `LaneSpawnController`
- absorbPrefabs: 4색 프리팹 연결
- hazardPrefab 연결
- gatePrefabs 배열에 게이트 프리팹 연결
- stageLayout 연결

### 7) Stage Manager
- 빈 오브젝트 `GameManager` + `StageGameManager`
- player/runner 연결
- 선택: 피니시 오브젝트에 `FinishZone`, `onReachedFinish -> StageGameManager.FinishStage`

### 8) HUD (선택)
- Canvas + Text 7개 생성
  - state/mass/combo/score/time/ability/result
- `SimpleHudPresenter` 추가 후 Text 슬롯 연결

### 9) WebGL 부트스트랩
- 빈 오브젝트 `Bootstrap` 생성
- `WebGameBootstrap` 컴포넌트 추가

---

## 처음 해보는 사람용: 5분 실행 가이드

유니티가 설치되어 있다면 아래만 따라하면 바로 플레이 테스트할 수 있습니다.

1. Unity Hub에서 이 폴더(`codex_unity`)를 프로젝트로 열기
2. 빈 씬 생성 후 README의 **15분 씬 세팅** 중 아래 최소 항목만 먼저 적용
   - Player (`RunnerController`, `PlayerStateController`)
   - Stage Balance Config / Stage Layout Config 생성 후 연결
   - 자원 프리팹 4종 + 장애물 프리팹 1종
   - `LaneSpawnController` 붙은 Spawner
   - `StageGameManager`
3. Play 버튼 눌러 에디터에서 바로 테스트
4. 조작
   - PC: 마우스 드래그 또는 `A/D`, `←/→`
   - 모바일: 터치 드래그

> 막히는 포인트: `PlayerStateController`의 `balance`가 비어 있으면 점수/질량 로직이 동작하지 않습니다.

---

## WebGL 빌드 가이드 (웹에서 실행)
1. `File > Build Settings` 열기
2. `WebGL` 플랫폼으로 `Switch Platform`
3. `Player Settings`에서 권장:
   - Resolution and Presentation:
     - Run In Background: On
     - Default Canvas Width/Height: 1280x720
   - Publishing Settings:
     - Compression Format: Gzip (서버에서 gzip 지원 시)
4. `Build` 또는 `Build And Run`
5. 결과 폴더를 정적 호스팅에 업로드 (예: itch.io, Netlify, GitHub Pages)

로컬에서 바로 확인하려면 `Build` 결과 폴더에서 아래처럼 실행:
```bash
python -m http.server 8080
```
그 다음 브라우저에서 `http://localhost:8080` 접속.

---

## 현재 규칙 핵심
- **일치 흡수**: 질량↑ 콤보↑ 점수↑
- **불일치 흡수**: 질량↓ 콤보 리셋 점수 패널티
- **RedHeat**: 이동속도↑ + 지속 질량 소모
- **GreenRecover**: 전환 시 즉시 회복
- **YellowOvercharge**: 점수 배수 추가
- **Split 게이트**: 일정 시간 흡수값 증폭
- **Magnet 게이트**: 일정 시간 동일 상태 자원 자동 흡수
- 질량이 `minMass` 이하가 되면 붕괴로 실패

---

## 다음 확장 포인트
- 모바일 UI 입력(스와이프 영역 분리)
- 스킨/업그레이드 메타 저장
- 사운드 큐(상태별 SFX + 콤보 피치)
- 보스 구간(연속 게이트 판단)
