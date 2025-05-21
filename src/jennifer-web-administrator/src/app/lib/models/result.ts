export type Result<T> = {
  isSuccess: boolean
  message?: string
  error?: any
  data?: T
}

export type TokenResponse = {
  accessToken: string
  refreshToken: string
}