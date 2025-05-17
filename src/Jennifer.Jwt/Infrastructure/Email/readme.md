# 📧 Channel 기반 Email Queue 처리 및 실패 대응 전략

## 개요

이 문서는 .NET에서 `Channel<T>`을 기반으로 이메일 전송 큐를 처리할 때, **메일 전송 실패 상황에 대한 안전한 재처리 전략**을 설계하는 방법을 다룹니다.  
`Channel<T>`는 비동기, 스레드 안전 큐를 제공하지만 **Dequeue(읽기) 즉시 데이터가 삭제**되기 때문에 실패 처리 로직이 없으면 데이터 손실이 발생할 수 있습니다.  
따라서, 유효성 보증을 위해 Kafka, Rabbitmq 사용을 권장 합니다.
---

## 기본 구조

```csharp
var email = await _emailQueue.DequeueAsync(stoppingToken);
await SendEmailAsync(email);
```

- `DequeueAsync()` 호출 시 메시지는 메모리 큐에서 제거됨
- `SendEmailAsync()`에서 예외 발생 시 해당 메시지는 손실됨
- 재전송 또는 기록 없으면 **데이터 무손실 처리 불가능**

---

## 실패 대응 전략

### ✅ 전략 1: 재시도 큐 삽입

```csharp
catch (Exception ex)
{
    _logger.LogError(ex, "이메일 전송 실패: {To}", email.To);
    await Task.Delay(5000, stoppingToken);
    await _emailQueue.EnqueueAsync(email, stoppingToken);
}
```

**장점**:
- 구현 간단  
  **단점**:
- 실패한 메시지가 무한 재시도될 수 있음

---

### ✅ 전략 2: 최대 재시도 횟수 제한

#### 모델 확장

```csharp
public class EmailMessage
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public int RetryCount { get; set; } = 0;
}
```

#### 큐 처리 로직

```csharp
if (email.RetryCount < 3)
{
    email.RetryCount++;
    await _emailQueue.EnqueueAsync(email, stoppingToken);
}
else
{
    _logger.LogWarning("재시도 초과, 로그 저장: {To}", email.To);
    // 실패 이메일 DB 저장 등
}
```

**장점**:
- 무한 루프 방지  
  **단점**:
- 상태 저장이 필요함

---

### ✅ 전략 3: 실패한 메시지 로그/보관소로 이동

```csharp
// 예: 실패 메시지를 DB나 파일 시스템에 저장
await _failedEmailLogService.SaveAsync(email);
```

**장점**:
- 재처리 전용 시스템 분리 가능
- 장애 후 수동 복구 가능  
  **단점**:
- 시스템 복잡도 증가

---

## Channel<T> 특성 요약

| 특성 | 설명 |
|------|------|
| 읽기 시 삭제 | `DequeueAsync`는 읽은 데이터를 메모리에서 제거 |
| 안전성 | Lock 없이 멀티스레드 안전 |
| 메시지 유지 | 별도 재삽입 로직 없으면 메시지 손실 발생 가능 |

---

## 결론

Channel 기반 이메일 전송 구조는 **비동기 처리와 성능에 유리**하지만, 실패한 메시지를 안전하게 처리하려면 반드시 다음 전략 중 하나 이상을 포함해야 합니다:

- ✅ 재시도 횟수 제한과 큐 재삽입
- ✅ 실패 메시지 로그 저장
- ✅ 고급 시나리오에서는 Kafka, Redis, DB 큐 등 Durable 메시지 큐 고려

이러한 설계를 통해 이메일 처리 시스템의 **신뢰성과 데이터 무손실성**을 확보할 수 있습니다.
