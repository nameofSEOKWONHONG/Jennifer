# CQRS와 Service - Repository

CQRS는 Command와 Query로 분리된 구조로 Command는 CUD를 Query는 R을 처리한다.
Service는 Domain을 생성 처리하는 Domain Service를 의미한다.
Repository는 EF CORE를 사용한다는 전제로 만들 필요는 없지만 특정 쿼리가 반복 사용되는 경우 Repository로 만들 수 있다.

따라서 흐름은 아래와 같다.

1. COMMAND -> SERVICE -> REPOSITORY -> DB
2. QUERY -> SERVICE -> REPOSITORY -> DB
3. COMMAND -> SERVICE -> DB
4. QUERY -> SERVICE -> DB
5. COMMAND -> DB
6. QUERY -> DB

SERVICE와 REPOSITORY는 반복되거나 재사용되는 코드를 목표로 한다.

CQRS의 Handler는 Endpoint API에 노출되지 않으며 샌드박스 형태로 Application 내부의 독립적인 프로세스로 만든다.
따라서, Request 객체는 별도로 필요하며 경우에 따라 Request 객체가 Endpoint와 Service에서 사용될 수 있다.