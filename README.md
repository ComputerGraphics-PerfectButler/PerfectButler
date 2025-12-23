# 🐈‍⬛ Perfect Butler

<img width="1920" height="1080" alt="perfectbutler" src="https://github.com/user-attachments/assets/1b4f249c-487b-4804-adb1-2f912aad74f8" />


## 디렉토리 구조 설명

```
PerfectButler/
└── Assets/
    ├── _Project/                # 메인 프로젝트 폴더
    │   ├── Scenes/              # Unity 씬 파일
    │   │   ├── Main/           # 메인 게임 씬
    │   │   │   ├── MainMenu.unity          # 메인 메뉴
    │   │   │   ├── room.unity              # 집 씬 (메인 게임)
    │   │   │   ├── park.unity              # 공원 씬 (고양이 찾기)
    │   │   │   ├── MiniGame_FishingRod.unity  # 미니게임: 낚싯대 흔들기
    │   │   │   └── MiniGame_WhackAMole.unity  # 미니게임: 두더지잡기
    │   │   └── Test/           # 테스트 씬
    │   │       ├── CatMovement.unity       # 고양이 모션 테스트
    │   │       ├── CharacterMovement.unity # 캐릭터 모션 테스트
    │   │       ├── Home UI.unity           # 홈 UI 테스트
    │   │       └── room_test_auto_cat.unity # 고양이 자동 이동 테스트
    │   │
    │   ├── Scripts/             # C# 스크립트
    │   │   ├── GameSystem/     # 게임 시스템
    │   │   │   ├── GameManager.cs              # 게임 매니저 (싱글톤)
    │   │   │   ├── CatStats.cs                 # 고양이 스탯 시스템
    │   │   │   ├── StatType.cs                 # 스탯 타입 정의
    │   │   │   ├── LevelData.cs                # 레벨 데이터
    │   │   │   ├── GameLevelManager.cs         # 레벨 관리 시스템
    │   │   │   ├── RoomDecoManager.cs          # 방 장식 관리 (레벨별 가구)
    │   │   │   ├── SaveLoadManager.cs          # 저장/불러오기
    │   │   │   ├── CatInteractionManager.cs    # 고양이 상호작용
    │   │   │   ├── PlayerInteractionController.cs  # 플레이어 상호작용 입력
    │   │   │   ├── VacuumCleanerInteraction.cs # 청소기 상호작용
    │   │   │   ├── MiniGameBase.cs             # 미니게임 기본 클래스
    │   │   │   ├── MiniGameResultManager.cs    # 미니게임 결과 관리
    │   │   │   ├── FishingRodGame.cs           # 낚싯대 미니게임
    │   │   │   ├── WhackAMoleGame.cs           # 두더지잡기 미니게임
    │   │   │   └── CaughtCatLoader.cs          # 공원에서 잡은 고양이 로드
    │   │   ├── Controller/      # 캐릭터 컨트롤러
    │   │   │   ├── PlayerMovement.cs           # 플레이어 이동 (Room)
    │   │   │   ├── ParkPlayerMovement.cs       # 플레이어 이동 (Park)
    │   │   │   ├── PlayerInventory.cs          # 플레이어 인벤토리
    │   │   │   ├── CatController.cs            # 고양이 컨트롤러
    │   │   │   ├── CatWanderAI.cs              # 고양이 배회 AI (Park)
    │   │   │   ├── AutoCatController_Fixed.cs  # 자동 고양이 컨트롤러
    │   │   │   ├── FollowCamera.cs             # 카메라 팔로우
    │   │   │   ├── ItemBoxController.cs        # 아이템 박스 컨트롤러
    │   │   │   ├── PatternRecorder.cs          # 패턴 녹화
    │   │   │   ├── PatternPlayer.cs            # 패턴 재생
    │   │   │   ├── PatternDataManager.cs       # 패턴 데이터 관리
    │   │   │   └── AutoRotate.cs               # 자동 회전
    │   │   ├── UI/             # UI 관련
    │   │   │   ├── MainGameUI.cs               # 메인 게임 UI
    │   │   │   ├── MainMenuManager.cs          # 메인 메뉴 매니저
    │   │   │   ├── InteractionUIManager.cs     # 상호작용 UI 관리
    │   │   │   ├── InteractionHintUI.cs        # 상호작용 힌트 UI (E키)
    │   │   │   ├── MiniGameSelectionPopup.cs   # 미니게임 선택 팝업
    │   │   │   ├── MiniGameResultHandler.cs    # 미니게임 결과 처리
    │   │   │   ├── GameOverPopup.cs            # 게임 오버 팝업
    │   │   │   ├── GameCompletePopup.cs        # 게임 완료 팝업
    │   │   │   └── ButtonHoverEffect.cs        # 버튼 호버 효과
    │   │   └── Audio/          # 오디오 관련
    │   │       ├── BGMManager.cs               # BGM 매니저 (싱글톤)
    │   │       ├── SFXManager.cs               # SFX 매니저 (싱글톤)
    │   │       └── SceneBGMConroller.cs        # 씬별 BGM 컨트롤러
    │   │
    │   └── Audio/              # 오디오 파일
    │       ├── BGM/            # 배경음악
    │       └── SFX/            # 효과음
    │
    ├── Art/                     # 아트 리소스
    │   └── UI/                 # UI 이미지
    │
    ├── Fonts/                   # 폰트
    │
    ├── 98-3d-cats-tree/        # 외부 에셋: 고양이 트리
    ├── Cats/                    # 외부 에셋: 고양이 모델
    ├── Fries and Seagull/       # 외부 에셋: 프라이스와 갈매기
    ├── Furniture Mega Pack/     # 외부 에셋: 가구
    ├── Hyper Casual Characters/ # 외부 에셋: 캐릭터
    ├── ithappy/                 # 외부 에셋
    ├── Palmov Island/           # 외부 에셋: 섬 배경
    ├── Settings/                # Unity 설정
    └── TextMesh Pro/            # TextMesh Pro 에셋
```

## 개발 현황

### 완료된 기능

#### 핵심 게임 시스템
- [x] 게임 매니저 (싱글톤 패턴)
- [x] 고양이 스탯 시스템
  - [x] 배고픔, 재미, 위생, 건강 스탯 관리
  - [x] 경험치 & 레벨 시스템 (0~5 레벨)
  - [x] 스탯별 쿨타임 관리
  - [x] 게임 오버/완료 이벤트 시스템
- [x] 저장/불러오기 시스템 (PlayerPrefs)
- [x] 레벨별 방 장식 시스템 (가구 자동 표시/숨김)

#### 씬 구성
- [x] MainMenu 씬 - 메인 메뉴
- [x] room 씬 - 집 맵 (메인 게임)
- [x] park 씬 - 공원 맵 (고양이 찾기)
- [x] MiniGame_FishingRod 씬 - 낚싯대 흔들기 미니게임
- [x] MiniGame_WhackAMole 씬 - 두더지잡기 미니게임
- [x] Test 씬들 (모션 테스트, UI 테스트)

#### 플레이어 시스템
- [x] Room 씬 플레이어 이동 (WASD, Y축 고정)
- [x] Park 씬 플레이어 이동 (발자국 SFX 포함)
- [x] 플레이어 인벤토리 (아이템 선택)
- [x] 상호작용 시스템 (E키)
- [x] 상호작용 힌트 UI (E키 표시)
- [x] 카메라 팔로우 시스템

#### 고양이 시스템
- [x] 고양이 컨트롤러 (애니메이션)
- [x] Park 씬 고양이 배회 AI
  - [x] 아이템 좋아함/싫어함 반응
  - [x] 아이템에 따른 호감도 시스템
- [x] Room 씬 고양이 자동 이동
- [x] 고양이 상호작용 (밥주기, 놀아주기, 병원, 청소)

#### UI 시스템
- [x] 메인 메뉴 (시작하기, 이어하기, 설정, 종료)
- [x] 메인 게임 UI (스탯 표시, 레벨 표시)
- [x] 상호작용 UI (밥주기, 놀아주기, 병원, 청소 버튼)
- [x] 미니게임 선택 팝업
- [x] 미니게임 결과 처리
- [x] 게임 오버 팝업 (스탯 0 도달 시)
- [x] 게임 완료 팝업 (최고 레벨 도달 시)
- [x] 버튼 호버 효과

#### 미니게임 시스템
- [x] 미니게임 기본 프레임워크
- [x] 낚싯대 흔들기 게임 (스페이스바 연타)
- [x] 두더지잡기 게임
- [x] 미니게임 결과 등급 (Perfect/Normal/Fail)
- [x] 미니게임 완료 후 보상 시스템

#### 오디오 시스템
- [x] BGM 매니저 (씬별 자동 재생, 볼륨 조절)
- [x] SFX 매니저 (효과음 재생, 볼륨 조절)
- [x] 씬별 BGM 설정
- [x] 효과음 적용
  - [x] 메인 메뉴 버튼 클릭
  - [x] Room 상호작용 버튼 클릭
  - [x] Park 플레이어 발자국
  - [x] Room 플레이어 발자국
  - [x] Park 고양이 반응 (좋아함/싫어함)
  - [x] Park 아이템 획득
  - [x] Room 고양이 밥 먹기
  - [x] 레벨업
  - [x] 미니게임 완료 후 복귀
  - [x] 게임 오버
  - [x] 게임 완료

### 향후 계획
- [ ] 튜토리얼 시스템
- [ ] 업적 시스템
- [ ] 추가 미니게임
- [ ] 스토리 요소 추가
- [ ] 그래픽 최적화


## 🤝 협업 규칙

### Git 워크플로우
- `main`: 배포용 안정 브랜치
- `develop`: 개발 통합 브랜치  
- `feature/기능명`: 개별 기능 개발

### 브랜치 명명 규칙
- `feature/player-controller`
- `feature/ui-system` 
- `feature/3d-models`
- `feature/minigames` 

### 커밋 메시지 규칙
- `feat: 새 기능 추가`
- `fix: 버그 수정`
- `docs: 문서 수정`
- `style: 코드 포맷팅`

### Unity 작업 규칙
- 씬 저장 후 반드시 커밋
- 개인 테스트는 Test 폴더에서
- 메인 씬 수정 전 팀원과 상의
