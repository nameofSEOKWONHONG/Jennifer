'use client'

import { useState } from 'react'
import { useRouter } from 'next/navigation'
import { signIn } from '@/app/lib/api/authApi'
import { SignInModel } from '@/app/lib/models/siginModel'
import { useAuthStore } from '../stores/authStore'

import {
  Box,
  Button,
  Checkbox,
  CircularProgress,
  FormControlLabel,
  TextField,
  Typography,
  Alert,
  Stack
} from '@mui/material'



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
    <Box
      display="flex"
      flexDirection="column"
      alignItems="center"
      justifyContent="center"
      minHeight="100vh"
      px={2}
    >
      <Typography variant="h4" gutterBottom>로그인</Typography>

      <Box width="100%" maxWidth="400px">
        <Stack spacing={2}>
          <TextField
            label="이메일"
            type="email"
            value={model.email}
            onChange={(e) => handleChange('email', e.target.value)}
            fullWidth
          />
          <TextField
            label="비밀번호"
            type="password"
            value={model.password}
            onChange={(e) => handleChange('password', e.target.value)}
            fullWidth
          />
          <FormControlLabel
            control={
              <Checkbox
                checked={model.rememberMe}
                onChange={(e) => handleChange('rememberMe', e.target.checked)}
              />
            }
            label="로그인 상태 유지"
          />
          <Button
            variant="contained"
            color="primary"
            fullWidth
            onClick={handleLogin}
            disabled={loading}
          >
            {loading ? <CircularProgress size={24} /> : '로그인'}
          </Button>
          {message && (
            <Alert severity={message.includes('성공') ? 'success' : 'error'}>
              {message}
            </Alert>
          )}
        </Stack>
      </Box>
    </Box>
  )
}