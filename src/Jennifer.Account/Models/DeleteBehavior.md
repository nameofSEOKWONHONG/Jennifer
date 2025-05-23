# EF Core DeleteBehavior 환경설정(관계 삭제 동작) 정보

Entity Framework Core에서 관계 삭제 동작을 제어하는 `DeleteBehavior` 역가함(값) 옵션은
구체 에너티가 삭제될 때, 관련된 부분 에너티에 어떤 동작을 적용할지 결정합니다.

---

## 패턴 정보

### 1. `Cascade`

* 구체 에너티가 삭제되면 관련된 부분 에너티도 자동으로 삭제
* 대본 1\:N 관계에서 가장 자주 사용
* DB FK에 `ON DELETE CASCADE`
* 예) User 삭제시 UserClaim, UserRole 도 함꿈 삭제

### 2. `Restrict`

* 부분 관계가 존재하면 삭제 불가 (예외 발생)
* FK 문제가 발생해서 삭제가 중지됨
* 결항 무가정성 보장에 적합

### 3. `SetNull`

* 부분이 삭제되면 FK 값을 null 로 변경
* FK 키가 nullable이어야 함
* FK 관계가 발견이 없어도 각 키는 남죠

### 4. `NoAction`

* EF Core가 아무 동작도 지시하지 않음
* 데이터베이스 드론에 따라 가능 (SQL Server에서는 Restrict와 같이 동작)
* DB가 결정하는 방식을 원한다면 사용

### 5. `ClientSetNull`

* EF Core가 메모리에서 FK를 null 로 변경
* 직접 DB에는 동작 없음
* `SaveChanges()` 전에 수동 체크를 필요로 함

### 6. `ClientCascade` (EF Core 8+)

* EF 메모리에서 부분 삭제시 부분도 함꿈 삭제 (DB에가서 반영X)
* FK 문제 없고 다른 구성이 필요할 때 해결책
* Tracked 된 에너티에만 적용

---

## 매기 DeleteBehavior 보고기

| 옵션            | 부모 삭제 시 동작     | DB 반영 | FK nullable | 특징                     |
| ------------- | -------------- | ----- | ----------- | ---------------------- |
| Cascade       | 자신도 삭제         | ✔ Yes | ✖ No        | 일반\uuc801 관계에 많이 사용    |
| Restrict      | FK 있으면 삭제 금지   | ✔ Yes | ✖ No        | 무가정성 가장 강하게 보장         |
| SetNull       | FK 값 null 처리   | ✔ Yes | ✔ Yes       | FK가 선호적이면 적합           |
| NoAction      | 가능한 행동은 디비가 결정 | ✔ Yes | ✖ No        | DB FK 관련을 역할가지고 있음     |
| ClientSetNull | EF에서 null      | ✖ No  | ✔ Yes       | EF 메모리 제어에 해당          |
| ClientCascade | EF에서 자신 삭제     | ✖ No  | ✖ No        | EF8+ 메모리 cascade 시리케이션 |

---

## 결단

* 복잡 1\:N 관계는 **Cascade** 또는 **Restrict** 가 기본
* FK가 nullable 일 경우에는 **SetNull** 도 가능
* **ClientSetNull**, **ClientCascade**는 테스트, offline mode 등 개발 특수 형태에서만 포함
