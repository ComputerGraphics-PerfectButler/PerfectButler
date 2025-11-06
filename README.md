# 🐈‍⬛ Perfect Butler

## 디렉토리 구조 설명

```
PerfectButler/
└── Assets/
    ├── _Project/                # 메인 프로젝트 폴더
    │   ├── Scenes/              # Unity 씬 파일
    │   │   ├── Main/           # 메인 게임 씬
    │   │   │   ├── room.unity     # 집 씬
    │   │   │   ├── park1.unity     # 공원 씬
    │   │   │   └── MainMenu.unity # 메인 메뉴
    │   │   └── Test/           # 테스트 씬
    │   │       ├── CatMovement.unity       # 고양이 모션 테스트
    │   │       └── CharacterMovement.unity # 캐릭터 모션 테스트
    │   │
    │   ├── Scripts/             # C# 스크립트
    │   │   ├── GameSystem/     # 게임 시스템
    │   │   │   ├── GameManager.cs    # 게임 매니저 (싱글톤)
    │   │   │   ├── CatStats.cs       # 고양이 스탯 시스템
    │   │   │   ├── StatType.cs       # 스탯 타입 정의
    │   │   │   └── LevelData.cs      # 레벨 데이터
    │   │   ├── Controller/      # 캐릭터 컨트롤러
    │   │   │   ├── PlayerMovement.cs # 플레이어 이동 컨트롤러
    │   │   │   ├── CatController.cs  # 고양이 컨트롤러
    │   │   │   └── FollowCamera.cs   # 카메라 팔로우
    │   │   ├── UI/             # UI 관련
    │   │   │   └── MainGameUI.cs     # 메인 게임 UI
    │   │   ├── Player/         # 플레이어 관련
    │   │   └── Audio/          # 오디오 관련
    │   │
    │   ├── Prefabs/            # 프리팹
    │   ├── Models/             # 3D 모델
    │   ├── Animations/         # 애니메이션
    │   ├── Materials/          # 머티리얼
    │   ├── Textures/           # 텍스처
    │   └── Audio/              # 오디오 파일
    │
    ├── 98-3d-cats-tree/        # 외부 에셋: 고양이 트리
    ├── Art/                     # 외부 에셋: 아트 리소스
    ├── Cats/                    # 외부 에셋: 고양이 모델
    ├── Fries and Seagull/       # 외부 에셋: 프라이스와 갈매기
    ├── Furniture Mega Pack/     # 외부 에셋: 가구
    ├── Hyper Casual Characters/ # 외부 에셋: 캐릭터
    ├── ithappy/                 # 외부 에셋
    ├── Palmov Island/           # 외부 에셋: 섬 배경
    ├── Settings/                # Unity 설정
    └── Textures/                # 공용 텍스처
```

## 개발 현황

### 완료된 기능
- [x] 기본 게임 시스템 구조 (GameManager 싱글톤)
- [x] 고양이 스탯 시스템 (CatStats, StatType)
- [x] 씬 구성
  - [x] MainMenu 씬 (메인 메뉴)
  - [x] Home 씬 (집 맵)
  - [x] Park 씬 (공원 맵)
  - [x] Test 씬 (CatMovement, CharacterMovement)
- [x] 캐릭터 모션 시스템
  - [x] Stickman 캐릭터 모션 (PlayerMovement.cs)
  - [x] 고양이 모션 (CatController.cs)
  - [x] 카메라 팔로우 시스템 (FollowCamera.cs)
- [x] 메인 게임 UI 기본 구조

### 진행 중인 작업
- [ ] 플레이어 컨트롤러 고도화
- [ ] 고양이 상호작용
- [ ] 미니게임 시스템
- [ ] 오디오 시스템

### 향후 계획
- [ ] 스토리 시스템
- [ ] 세이브/로드 시스템
- [ ] 업적 시스템
- [ ] 추가 맵 구성


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
