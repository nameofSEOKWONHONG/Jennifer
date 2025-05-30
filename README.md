# Jennifer 프로젝트 구조 개요

.net account management library

## 📌 프로젝트 개요
Jennifer는 ASP.NET Core Minimal API 기반의 계정 관리 시스템으로, EF Core Identity를 기반으로 사용자, 역할, 클레임 등의 인증/인가 기능을 제공합니다.  
모놀리식 배포 방식이지만 계층형 클린 아키텍처를 기반으로 향후 마이크로서비스 전환에 대비된 구조를 채택하였습니다.

---

## 📁 주요 프로젝트 구성

```
src/
├── Jennifer.Api                      # Minimal API 진입점
├── Jennifer.Account                  # CQRS 기반 계정 Application 계층
├── Jennifer.Todo                     # CQRS 기반 예졔 Aplpication 계층
├── Jennifer.Domain                   # 도메인 모델, 도메인 이벤트
├── Jennifer.Infrastructure           # Kafka/Redis/EF 연동 구현, 공통 메시징/캐시 인프라 서비스
├── Jennifer.SharedKernel             # 공용 Result, Enum, 인터페이스
├── Jennifer.External.OAuth           # OAuth 로그인 인증 지원
```

---

## 🏗️ 아키텍처 계층 구성

### ✅ Presentation Layer
- **Jennifer.Api**  
  Minimal API 구성, 사용자 요청 수신 → Application 계층 위임

### ✅ Application Layer
- **Jennifer.Account**  
  CQRS 구조 (Command/Query + Handler), Mediator를 통해 분기  
  Domain과 Infrastructure를 호출하며 흐름 조정 역할  
  계정계 시스템 구현

### ✅ Domain Layer
- **Jennifer.Domain**  
  도메인 엔티티 (User, Role, Claim 등)  
  도메인 이벤트 (예: `UserCompleteDomainEvent`)

- **Jennifer.SharedKernel**  
  `Result<T>`, `IAuditable`, `ENUM_XXX` 등 공용 요소

### ✅ Infrastructure Layer
- **Jennifer.Infrastructure**  
  Kafka를 통한 비동기 이메일 인증 코드 발송  
  Redis 기반 HybridCache  
  EF Core Identity 연동
  Redis, Kafka 공통 구성 캡슐화

### ✅ External Systems
- **Kafka**: 이메일 인증 이벤트 처리
- **Redis**: 캐시 및 세션 상태 저장
- **EF Core Identity**: 사용자/권한/토큰 저장

---

## 🔁 기술 스택 및 패턴

| 항목 | 기술 |
|------|------|
| 언어/프레임워크 | .NET 9 (Preview), ASP.NET Core Minimal API |
| 인증/인가 | EF Core Identity 기반, JWT + RefreshToken, 2FA 일부 |
| 구조 | CQRS + Mediator 패턴 + DomainEvent |
| 비동기 메시징 | Apache Kafka |
| 캐시 | Redis (HybridCache) |
| 감사 로그 | IAuditable + SaveChangesInterceptor 구현 (→ Kafka 연동 확장 가능) |

---

## 🚀 구조적 특성

- ✅ **계층형 클린 아키텍처** 기반의 모놀리식 애플리케이션
- ✅ **CQRS 및 도메인 이벤트 구조**를 적용하여 책임 분리
- ✅ **Kafka/Redis 연동**을 통해 메시징 및 캐시 구조 반영

---

## 🧩 참조 관계 (요약)

- `Jennifer.Api` → Application, Mediator, Identity
- `Application` → Domain, Infrastructure, SharedKernel
- `Infrastructure` → Redis, Kafka, EF Core, SharedKernel

---

## 🔒 보안 및 감사 처리
- `IAuditable` 인터페이스 구현
- `AuditableInterceptor`를 통해 Entity 상태 추적
- Kafka Producer 연동 시 실시간 감사 로그 전송 가능

---

## 📦 배포 형태

- **배포 단위**: 단일 모놀리식 애플리케이션
- **인프라 구성**: Kafka, Redis, PostgreSQL/SQL Server (가정)

---

## 📝 향후 확장 고려 사항
- Kafka 기반 감사 로그 Consumer 구성 (ELK 연동)
- 마이크로서비스로 인증/사용자 분리

[architecture image](./doc/jennifer-architecure.png)