const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5080'

export type HealthStatus = 'healthy' | 'unreachable'

export async function getHealth(): Promise<HealthStatus> {
  try {
    const response = await fetch(`${apiBaseUrl}/health`)
    return response.ok ? 'healthy' : 'unreachable'
  } catch {
    return 'unreachable'
  }
}