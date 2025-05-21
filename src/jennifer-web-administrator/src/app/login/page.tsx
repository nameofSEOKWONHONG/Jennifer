'use client'

import { useState } from 'react'
import { useRouter } from 'next/navigation'
import { signIn } from '@/app/lib/api/authApi'
import { SignInModel } from '@/app/lib/models/siginModel'
import { useAuthStore } from '../stores/authStore'

export default function LoginPage() {
  const [model, setModel] = useState<SignInModel>({
    email: '',
    password: '',
    rememberMe: false
  })
  const [loading, setLoading] = useState(false)
  const [message, setMessage] = useState<string | null>(null)

  const router = useRouter()
  const setToken = useAuthStore((state) => state.setToken)  

  const handleChange = (field: keyof SignInModel, value: string | boolean) => {
    setModel(prev => ({ ...prev, [field]: value }))
  }

  const handleLogin = async () => {
    setLoading(true)
    setMessage(null)

    try {
      const result = await signIn(model.email, model.password)
      if (result.isSuccess) {
        useAuthStore.getState().setToken(result.data?.accessToken || '')
        useAuthStore.getState().setRefreshToken(result.data?.refreshToken || null)
        setMessage(`로그인 성공! 토큰: ${result.data?.accessToken}`)
        router.push('/dashboard')
      } else {
        setMessage(`로그인 실패: ${result.message}`)
      }
    } catch (err: any) {
      setMessage(err.message || '예상치 못한 오류')
    } finally {
      setLoading(false)
    }
  }

  return (
    <main className="flex min-h-screen flex-col items-center justify-center p-8">
      <h1 className="text-3xl font-bold mb-6">로그인</h1>

      <div className="w-full max-w-sm space-y-4">
        <input
          type="email"
          placeholder="이메일"
          value={model.email}
          onChange={(e) => handleChange('email', e.target.value)}
          className="w-full p-3 border border-gray-300 rounded"
        />
        <input
          type="password"
          placeholder="비밀번호"
          value={model.password}
          onChange={(e) => handleChange('password', e.target.value)}
          className="w-full p-3 border border-gray-300 rounded"
        />
        <label className="flex items-center space-x-2 text-sm">
          <input
            type="checkbox"
            checked={model.rememberMe}
            onChange={(e) => handleChange('rememberMe', e.target.checked)}
          />
          <span>로그인 상태 유지</span>
        </label>
        <button
          onClick={handleLogin}
          disabled={loading}
          className="w-full bg-blue-600 text-white py-3 rounded hover:bg-blue-700 disabled:bg-gray-400"
        >
          {loading ? '로그인 중...' : '로그인'}
        </button>
        {message && <p className="text-center mt-4 text-sm text-red-600">{message}</p>}
      </div>
    </main>
  )
}