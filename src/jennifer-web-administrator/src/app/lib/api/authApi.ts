import { config } from '../config'
import { Result, TokenResponse } from '../models/result'

async function post<T>(url: string, payload: any): Promise<T> {
  const res = await fetch(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Accept: 'application/json'
    },
    body: JSON.stringify(payload)
  })

  if (!res.ok) {
    const errorBody = await res.text()
    throw new Error(`Request failed: ${res.status} - ${errorBody}`)
  }

  return res.json()
}

export async function signIn(email: string, password: string): Promise<Result<TokenResponse>> {
  const url = `${config.apiBaseUrl}${config.authApi.signin}`
  return post(url, { email, password })
}
