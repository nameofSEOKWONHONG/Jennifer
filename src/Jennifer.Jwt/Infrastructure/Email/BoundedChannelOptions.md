## 📦 BoundedChannelOptions 전체 설정 항목 목록

| 속성 이름 | 타입 | 기본값 | 설명 |
|-----------|------|--------|------|
| `Capacity` | `int` | 없음 (필수) | 채널이 수용할 수 있는 최대 항목 수 |
| `FullMode` | `BoundedChannelFullMode` | `Wait` | 채널이 가득 찼을 때 쓰기 시도에 대한 동작 방식 |
| `AllowSynchronousContinuations` | `bool` | `false` | true일 경우 WriteAsync/ReadAsync 이후 continuation이 동기적으로 실행될 수 있음 |
| `SingleReader` | `bool` | `false` | true이면 읽기 작업이 단일 쓰레드/컨슈머로 제한되어 최적화 |
| `SingleWriter` | `bool` | `false` | true이면 쓰기 작업이 단일 쓰레드/프로듀서로 제한되어 최적화 |

---

✅ 예시 전체 설정 설명

```csharp
var options = new BoundedChannelOptions(100)
{
FullMode = BoundedChannelFullMode.Wait,
SingleReader = true,
SingleWriter = false,
AllowSynchronousContinuations = false // 명시적으로도 설정 가능
};
```

| 설정 항목                                   | 의미                                              |
| --------------------------------------- | ----------------------------------------------- |
| `Capacity = 100`                        | 최대 100개의 이메일 메시지를 큐에 보관 가능                      |
| `FullMode = Wait`                       | 큐가 가득 차면 Enqueue 요청이 대기 상태로 들어감 (손실 없음)         |
| `SingleReader = true`                   | EmailSenderService 한 개만 큐를 소비하므로 최적화 가능         |
| `SingleWriter = false`                  | 여러 스레드에서 Enqueue 가능 (예: API 요청 병렬 처리)           |
| `AllowSynchronousContinuations = false` | Task continuation이 동기 실행되지 않도록 보장 (Deadlock 방지) |

---

### 🔄 BoundedChannelFullMode 값 설명

| 값 | 설명 |
|-----|------|
| `Wait` | 버퍼가 가득 차면 쓰기 호출이 대기 상태에 들어감 |
| `DropWrite` | 새로 쓰려는 항목은 무시됨 (유실 가능) |
| `DropOldest` | 가장 오래된 항목을 제거하고 새 항목을 삽입 |
| `DropNewest` | 가장 최근 항목을 제거하고 새 항목을 삽입 |
