const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5080'

export class ApiError extends Error {
  readonly status: number

  constructor(message: string, status: number) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}

export async function request<T>(path: string, init: RequestInit = {}): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    ...init,
    headers: {'Content-Type': 'application/json', ...init.headers},
  })

  if (!response.ok) {
    const problem = (await response.json().catch(() => null)) as { detail?: string; title?: string } | null
    throw new ApiError(problem?.detail ?? problem?.title ?? `Request failed (${response.status}).`, response.status)
  }

  return (await response.json()) as T
}