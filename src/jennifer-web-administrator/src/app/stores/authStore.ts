import { create } from 'zustand'

type AuthStore = {
  token: string | null
  setToken: (token: string) => void
  setRefreshToken: (token: string | null) => void
  clear: () => void
  isAuthenticated: () => boolean
}

export const useAuthStore = create<AuthStore>((set, get) => ({
  token: null,
  setToken: (token) => set({ token }),
  setRefreshToken: (token) => set({ token }),
  clear: () => set({ token: null }),
  isAuthenticated: () => !!get().token,
}))