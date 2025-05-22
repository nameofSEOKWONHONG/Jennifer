'use client'

import LoginPage from './login/page'
import { Container } from '@mui/material'

export default function HOME() {
  return (
    <Container maxWidth="sm" sx={{ mt: 8 }}>
      <LoginPage />
    </Container>
  )
}