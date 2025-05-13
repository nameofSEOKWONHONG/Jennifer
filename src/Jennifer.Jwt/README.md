# Jennifer.Jwt

`Jennifer.Jwt`는 ASP.NET Core 환경에서 JWT(JSON WEB TOKEN)를 사용한 
single tenant 인증/인가 처리를 담당하는 모듈입니다.

## 주요 목적

- 사용자 인증 상태 관리
- 사용자 관리
- 권한 관리
- 각종 계정 관련 유틸리티 구현

## 구성요소

- EntityFrameworkCore
- EFCore.Identity
- Scalar
- SqlServer
- Jwt

## 사용예

```csharp
// Identity 기반 Database 설정 및 관련 Service 등록합니다. IdentityOption에 대한 설정을 할 수 있습니다.
builder.AddJennifer("account", 
    (provider, optionsBuilder) =>
    {
        var con = builder.Configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseSqlServer(con);
        if (builder.Environment.IsDevelopment())
        {
            optionsBuilder.EnableSensitiveDataLogging()
                .EnableThreadSafetyChecks()
                .EnableDetailedErrors();
        }
    }, null);

// HybridCache 기반 서비스 등록, Hybrid 캐시 및 Redis 캐시 설정을 등록합니다.
builder.Services.AddJenniferHybridCache(null, null);

// 계정 갱신에 대한 SignalR 서비스를 등록합니다. Redis Backplane에 대한 설정을 할 수 있습니다.
builder.Services.AddJenniferHub(null);

// 이메일 전송에 대한 설정을 등록합니다. 이메일 전송을 위한 큐 설정과 백그라운드 서비스를 등록합니다.
builder.Services.AddJenniferEmail();
```

## 작업

- [X] 로그인 관련 작업
- [X] 사용자 관리 작업
- [ ] 역활 권한 관리 작업
- [X] 이메일 전송
- [X] JWT 발급 및 갱신 작업
