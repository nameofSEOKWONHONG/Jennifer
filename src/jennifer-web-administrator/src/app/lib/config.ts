export const config = {
  apiBaseUrl: process.env.NEXT_PUBLIC_API_DOMAIN ?? 'https://localhost:7288',
  authApi: {
    signin: '/api/v1/auth/SignIn',
    register: '/api/v1/auth/register',
    logout: '/api/v1/auth/logout',
    refresh: '/api/v1/auth/refresh',
  }
}