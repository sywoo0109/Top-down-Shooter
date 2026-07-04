# Top-down Shooter

Unity로 제작한 2D 탑다운 슈팅 게임입니다. 이동 방향과 조준 방향이 독립적인 탑다운 슈팅 장르의 기본기를 다지고, 여기에 오브젝트 풀링, 이벤트 기반 아키텍처, 신버전 Input System 등 실무에서 쓰이는 구조를 적용해보는 것을 목표로 제작했습니다.

## 🎮 플레이 방법

| 조작 | 키 |
|---|---|
| 이동 | `W` `A` `S` `D` 또는 방향키 |
| 조준 & 발사 | 마우스 이동 & 좌클릭 |

시간이 지날수록 적의 스폰 주기가 짧아지며 난이도가 상승합니다. 최대한 오래 생존하며 점수를 쌓아보세요.

## ✨ 주요 기능

- **독립적인 이동/조준 시스템**: 이동 방향과 무관하게 마우스 방향으로 조준 및 사격
- **오브젝트 풀링(Object Pooling)**: 총알과 적 오브젝트를 매번 생성/파괴하지 않고 재사용하여 성능 최적화
- **점진적 난이도 상승**: 시간이 지날수록 적 스폰 주기가 짧아짐
- **체력 및 피격 반응 시스템**: 피격 시 사운드, 스프라이트 색상 변화(Flash) 등의 시각·청각적 피드백 제공

## 🛠 기술 스택

- **Engine**: Unity 6.3 LTS
- **Language**: C#
- **Input**: Unity Input System (신버전)

## 🏗 아키텍처 설계 포인트

- **Character.cs**: 체력 관리, 피격 반응(사운드/Flash), 사망 처리를 플레이어와 적이 공통으로 사용하는 컴포넌트로 분리. `OnDeathStart`, `OnDeathComplete` 이벤트를 통해 "무엇을 할지"는 각 오브젝트(PlayerController, EnemyController)가 스스로 구독해 처리하도록 설계하여, 재사용 가능한 범용 컴포넌트로 유지
- **GameManager.cs**: 씬 전환, 적 스폰, 점수 관리 등 게임 전체의 흐름을 담당. 플레이어의 사망 이벤트를 구독해 게임오버 씬으로 전환하는 로직을 플레이어 스크립트가 아닌 이곳에 위치시켜, 각 컴포넌트의 책임 범위를 명확히 분리
- **ObjectPool.cs**: 총알과 적 오브젝트를 사전에 생성해두고 활성화/비활성화로 재사용하여 `Instantiate`/`Destroy` 반복 호출로 인한 성능 저하 방지
- **Input System (신버전)**: 키 재설정, 다중 디바이스 지원, 입력 로직과 게임 로직의 분리를 위해 구버전 Input Manager 대신 채택

## 📦 프로젝트 구조

```
Assets/
 └─ Scripts/
     ├─ PlayerController.cs   # 플레이어 이동, 조준, 사격
     ├─ EnemyController.cs    # 적 이동 및 상태(스폰/이동/사망) 관리
     ├─ Character.cs          # 체력, 피격 반응, 사망 처리 공통 로직
     ├─ Bullet.cs              # 투사체 이동 및 충돌 처리
     ├─ ObjectPool.cs          # 오브젝트 풀링
     ├─ GameManager.cs         # 게임 흐름, 적 스폰, 점수 관리
     ├─ GameOverManager.cs     # 게임오버 화면 UI 처리
     └─ MenuManager.cs         # 메인 메뉴 UI 처리
```

## 🚀 실행 방법

1. 이 저장소를 클론합니다.
2. Unity Hub에서 Unity 6.3 LTS 버전으로 프로젝트를 엽니다.
3. `Assets/Scenes/MainMenuScene`을 열고 Play 버튼을 눌러 실행합니다.
혹은
https://sywoo0109.itch.io/top-down-shooter-toy-project 에서 플레이 가능합니다.

## 📝 제작 배경

Unity 기초 강좌를 참고해 처음 만든 토이 프로젝트를 바탕으로, 실무에서 흔히 쓰이는 구조와 패턴(캐싱, 이벤트 기반 설계, 신버전 Input System 등)을 스스로 학습하며 리팩토링하며 발전시킨 프로젝트입니다.
