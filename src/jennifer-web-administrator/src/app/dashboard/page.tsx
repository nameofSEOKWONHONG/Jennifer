'use client'

import RequireAuth from '@/app/components/RequireAuth'

export default function DashboardPage() {
  return (
    <RequireAuth>
      <div className="p-4">대시보드 접근 성공 (인증 됨)</div>
    </RequireAuth>
  )
}